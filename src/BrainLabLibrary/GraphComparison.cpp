#include "GraphComparison.h"
#include "Graph.h"
#include "Stats.h"
#include <amp_math.h>
#include <vector>
#include <algorithm>
#include <ppl.h>
#include <concurrent_vector.h>
#include <math.h>
#include <iostream>

using namespace std;
using namespace concurrency;

namespace BrainLabLibrary
{
	GraphComparison::GraphComparison(int subjectCount, int verts, int edges) 
		: _subjectEdges(boost::extents[edges][subjectCount]), _lu(verts), _grpStats(edges), _graph(verts) //, _g(verts, &_lu)
	{
		_subjectCount = subjectCount;
		_vertCount = verts;
		_edgeCount = edges;
		_currentSubjectIdx = 0;

		_permutations = 0;
		_largestComponentSize = 0;
		_rightTailComponentSizeCount = 0;
	}

	GraphComparison::~GraphComparison(void)
	{}

	void GraphComparison::AddGraph(Graph* graph)
	{
		int edge = 0;
		for(Graph::EdgeValueCollection::iterator iter = graph->EdgeValues.begin(); iter < graph->EdgeValues.end(); ++iter)
		{
			_subjectEdges[edge][_currentSubjectIdx] = iter->Value;
			//_subjectEdgesView(edge, _currentSubjectIdx) = iter->Value;
			_allEdges.push_back(iter->Value);
			++edge;
		}
		++_currentSubjectIdx;
	}

	// Calculate a T stat for the two groups based on the indexes passed in
	void GraphComparison::CalcEdgeTStats(const vector<int> &idxs, int szGrp1, vector<EdgeValue> &edgeStats)
	{
		// TODO: Probably need to make this thread safe
		//for(int i=0; i<_edgeCount; i++)
		parallel_for(0, _edgeCount, [=, &idxs, &edgeStats] (int i)
		{
			// Pull out a view of the subject values for a single edge
			SingleEdgeBySubject edgeValues = _subjectEdges[ boost::indices[i][range(0, _subjectCount)] ];
			
			int n1 = 0, n2 = 0;
			float m1 = 0, m2 = 0;
			float dv1 = 0, dv2 = 0;

			// Loop through the vals we were passed
			for (int idx = 0; idx < _subjectCount; ++idx)
			{
				float edgeVal = edgeValues[idxs[idx]];

				if (idx < szGrp1)
				{
					n1++;

					float delta = edgeVal - m1;
					m1 += delta / n1;

					if(n1 > 1)
						dv1 = dv1 + delta * (edgeVal - m1);
				}
				else
				{
					n2++;

					float delta = edgeVal - m2;
					m2 += delta / n2;

					if(n2 > 1)
						dv2 = dv2 + delta * (edgeVal - m2);
				}
			}

			float v1 = abs(dv1) / ( n1 - 1 );
			float v2 = abs(dv2) / ( n2 - 1 );
			
			float tstat = 0;
			if(v1 < 0.00000001f && v2 < 0.00000001f)
				tstat = 0;
			else
				tstat = (m1 - m2) / sqrt( ( v1 / (float)n1 ) + ( v2 / (float)n2 ) );

			edgeStats[i].V1 = v1;
			edgeStats[i].V2 = v2;
			edgeStats[i].M1 = m1;
			edgeStats[i].M2 = m2;
			edgeStats[i].TStat = tstat;
		});
	}

	void GraphComparison::CompareGroups(vector<int> &idxs, int szGrp1, double tStatThreshold, vector<int> &vertexList)
	{
		// Calculate t stats for this subject labeling
		CalcEdgeTStats(idxs, szGrp1, _grpStats);

		// Load graph with thresholded t stats
		for(vector<EdgeValue>::size_type idx=0; idx<_grpStats.size(); ++idx)
		{
			// Lookup our edge
			pair<int, int> edge = _lu.GetEdge(idx);

			// If our edge tstat is larger than our threshold keep it for NBS
			if(abs(_grpStats[idx].TStat) > tStatThreshold)
			{
				// Add the edge to our bgl graph
				boost::add_edge(edge.first, edge.second, _graph);

				// Put the edge idx in our vector
				_grpSupraThreshEdgeIdxs.push_back(idx);
			}
		}

		// Calculate our largest component
		ComputeComponents(_graph, _grpSupraThreshEdgeIdxs, _grpComponent);

		// Find the biggest component
		int maxEdgeCount = 0, maxId = 0;
		for(auto &cmp : _grpComponent)
		{
			int cmpEdgeCount = cmp.Edges.size();
			if(cmpEdgeCount > maxEdgeCount)
			{
				maxEdgeCount = cmpEdgeCount;
				maxId = cmp.Identifier;
			}
		}

		// Keep the extent size of the largest component
		_largestComponentSize = maxEdgeCount;
	}

	void GraphComparison::Permute(const vector<int> &idxs, int szGrp1, double tStatThreshold, vector<int> &vertexList)
	{
		// Make a temp graph for calculating high-level graph stats
		UDGraph g(_vertCount);

		// Create somewhere temp to store our permuted t stats
		vector<EdgeValue> permEdgeStats(_edgeCount);
		vector<int> supraThreshEdgeIdxs;

		// Calculate t stats for this random subject labeling
		CalcEdgeTStats(idxs, szGrp1, permEdgeStats);

		// Loop through edge stats and calc	our measures
		for(vector<EdgeValue>::size_type idx=0; idx<permEdgeStats.size(); ++idx)
		{
			// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
			if(abs(permEdgeStats[idx].TStat) >= abs(_grpStats[idx].TStat))
				_grpStats[idx].RightTailCount++;

			// TODO: Eventually store this as a graph tag so we can have multiple pieces of info stored with our edges
			// NBS level testing - Add this to the NBS graph if above our threshold, we will calc the size of the cmp soon
			if(abs(permEdgeStats[idx].TStat) > tStatThreshold)
			{
				// Lookup our edge
				std::pair<int, int> edge = _lu.GetEdge(idx);
				// Store the edge in our graph
				boost::add_edge(edge.first, edge.second, g);
				// Mark this edge as surviving
				supraThreshEdgeIdxs.push_back(idx);
			}
		}

		// NBS calcs
		vector<int> cmp;

		// Calculate our largest component
		//ComputeComponents(g, supraThreshEdgeIdxs, cmp, vertexList);

		// Keep this max for the NBS distribution
		int cmpSize = cmp.size();

		// TODO: Loop through all actual components and update rightTailCounts for each
		// Increment rt tail if this is larger than the actual component
		if(cmpSize >= _largestComponentSize)
			++_rightTailComponentSizeCount;

		// Keep track of permutations
		++_permutations;
	}

	void GraphComparison::ComputeComponents(UDGraph &graph, vector<int> &edgeIdxs, vector<Component> &components)
	{
		vector<int> cmpRawListingByVertex(_vertCount);

		// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
		int componentCount = boost::connected_components(graph, &cmpRawListingByVertex[0]);
		
		components.reserve(componentCount);

		for(auto edgeIdx : edgeIdxs)
		{
			ComponentEdge edgeItm;

			edgeItm.EdgeIndex = edgeIdx;
			// Lookup the edge
			edgeItm.Edge = _lu.GetEdge(edgeIdx);
			// Look up the first edge vertex
			edgeItm.ComponentIndex = cmpRawListingByVertex[edgeItm.Edge.first];
			
			// Store the ident for this cmp
			components[edgeItm.ComponentIndex].Identifier = edgeItm.ComponentIndex;
			// Add edge to component edge map
			components[edgeItm.ComponentIndex].Edges.push_back(edgeItm);
		}

		// Store the edges by component
		//map<int, vector<ComponentEdge>> cmps;
		//for(auto edgeIdx : edgeIdxs)
		//{
		//	ComponentEdge edgeItm;

		//	edgeItm.EdgeIndex = edgeIdx;
		//	// Lookup the edge
		//	edgeItm.Edge = _lu.GetEdge(edgeIdx);
		//	// Look up the first edge vertex
		//	edgeItm.ComponentIndex = cmpRawListingByVertex[edgeItm.Edge.first];
		//	
		//	// Add edge to component edge map
		//	cmps[edgeItm.ComponentIndex].push_back(edgeItm);
		//}

		//// Find the biggest component
		//int maxEdgeCount = 0, maxId = 0;
		//for(auto &cmp : cmps)
		//{
		//	int cmpEdgeCount = cmp.second.size();
		//	if(cmpEdgeCount > maxEdgeCount)
		//	{
		//		maxEdgeCount = cmpEdgeCount;
		//		maxId = cmp.first;
		//	}
		//}

		//// Copy the biggest component into our caller's array
		//components = cmps[maxId];

		//// Save the biggest component into the caller's vertex array
		////vertexList.resize(_vertCount);
		////for(vector<int>::size_type i=0; i<components.size(); ++i)
		////{
		////	int edgeIdx = components[i];
		////	std::pair<int, int> edge = _lu.GetEdge(edgeIdx);

		////	vertexList[edge.first] = 1;
		////	vertexList[edge.second] = 1;
		////}
	}

	void GraphComparison::GetComponents(std::vector<Component> &components)
	{
		for(auto cmp : _grpComponent)
			cmp.RightTailExtent = _rightTailComponentSizeCount;

		components = _grpComponent;
	}
}
