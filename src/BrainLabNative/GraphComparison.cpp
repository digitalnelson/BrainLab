#include "stdafx.h"
#include "GraphComparison.h"
#include "Graph.h"
#include "Stats.h"
#include <vector>
#include <algorithm>
#include <ppl.h>
#include <concurrent_vector.h>
#include <math.h>

using namespace Concurrency;
using namespace std;

namespace BrainLabNative
{
	GraphComparison::GraphComparison(int subjectCount, int verts, int edges) 
		: _subjectEdges(boost::extents[edges][subjectCount]), _lu(verts), _grpStats(edges), _g(verts), _graph(verts)
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
			++edge;
		}
		++_currentSubjectIdx;
	}

	void GraphComparison::CalcEdgeTStats(vector<int>& idxs, int szGrp1, vector<EdgeValue>& edgeStats)
	{
		//for(int i=0; i<_edgeCount; i++)
		parallel_for(0, _edgeCount, [=, &idxs, &edgeStats] (int i)
		{
			// Pull out a view of the subject values for a single edge
			SingleEdgeBySubject edgeValues = _subjectEdges[ boost::indices[i][range(0, _subjectCount)] ];

			// TODO: Probably need to make this thread safe
			// Calculate a T stat for the two groups based on the indexes passed in
			Stats::TStatByIndexEx(idxs, &edgeValues[0], _subjectCount, szGrp1, edgeStats[i]);
		});
	}

	void GraphComparison::CompareGroups(vector<int>& idxs, int szGrp1, double tStatThreshold, std::vector<int> &vertexList)
	{				
		// Calculate t stats for this subject labeling
		CalcEdgeTStats(idxs, szGrp1, _grpStats);

		// Load graph with thresholded t stats
		for(auto idx=0; idx<_grpStats.size(); ++idx)
		{
			// Lookup our edge
			std::pair<int, int> edge = _lu.GetEdge(idx);

			// If our edge tstat is larger than our threshold keep it for NBS
			if(abs(_grpStats[idx].TStat) > tStatThreshold)
			{
				boost::add_edge(edge.first, edge.second, _graph);
				_grpSupraThreshEdgeIdxs.push_back(idx);
			}
		}

		// Calculate our largest component
		ComputeComponents(_graph, _grpSupraThreshEdgeIdxs, _grpComponent, vertexList);

		// Keep the extent size of the largest component
		_largestComponentSize = _grpComponent.size();
	}

	void GraphComparison::Permute(vector<int>& idxs, int szGrp1, double tStatThreshold, std::vector<int> &vertexList)
	{
		// Make a temp graph for calculating high-level graph stats
		UDGraph g(_vertCount);

		// Create somewhere temp to store our permuted t stats
		vector<EdgeValue> permEdgeStats(_edgeCount);
		vector<int> supraThreshEdgeIdxs;

		// Calculate t stats for this random subject labeling
		CalcEdgeTStats(idxs, szGrp1, permEdgeStats);

		// Loop through edge stats and calc	our measures
		for(auto idx=0; idx<permEdgeStats.size(); ++idx)
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
		ComputeComponents(g, supraThreshEdgeIdxs, cmp, vertexList);

		// Keep this max for the NBS distribution
		int cmpSize = cmp.size();

		// TODO: Loop through all actual components and update rightTailCounts for each
		// Increment rt tail if this is larger than the actual component
		if(cmpSize >= _largestComponentSize)
			++_rightTailComponentSizeCount;

		// Keep track of permutations
		++_permutations;
	}

	void GraphComparison::ComputeComponents(UDGraph &graph, vector<int> &edgeIdxs, vector<int> &components, std::vector<int> &vertexList)
	{
		vector<int> cmpRawListingByVertex(_vertCount);

		// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
		int componentCount = boost::connected_components(graph, &cmpRawListingByVertex[0]);
		
		std::map<int, std::vector<int>> cmpEdge;

		// Store the edges by component
		for(auto it=edgeIdxs.begin(); it<edgeIdxs.end(); ++it)
		{
			// Get our edge index
			int idx = *it;
			// Lookup the edge
			std::pair<int, int> edge = _lu.GetEdge(idx);
			// Look up the first edge vertex
			int cmpId = cmpRawListingByVertex[edge.first];
			// Add edge to component edge map
			cmpEdge[cmpId].push_back(idx);
		}

		// Find the biggest component
		int maxEdges = 0, maxId = 0;
		for(auto it=cmpEdge.begin(); it != cmpEdge.end(); ++it)
		{
			int edges = it->second.size();
			if(edges > maxEdges)
			{
				maxEdges = edges;
				maxId = it->first;
			}
		}

		// Copy the biggest component into our caller's array
		components = cmpEdge[maxId];

		// Save the biggest component into the caller's vertex array
		vertexList.resize(_vertCount);
		for(auto i=0; i<components.size(); ++i)
		{
			int edgeIdx = components[i];
			std::pair<int, int> edge = _lu.GetEdge(edgeIdx);

			vertexList[edge.first] = 1;
			vertexList[edge.second] = 1;
		}
	}

	void GraphComparison::GetComponents(std::vector<Component> &components)
	{
		Component c;

		for(auto ei=_grpComponent.begin(); ei < _grpComponent.end(); ++ei)
		{
			ComponentEdge ce;
			ce.Edge = _lu.GetEdge(*ei);
			ce.EdgeValue = _grpStats[*ei];
			
			c.Edges.push_back(ce);
		}

		components.push_back(c);
	}

	double GraphComparison::GetComponentSizePVal()
	{
		if(_permutations > 0)
			return ((double)_rightTailComponentSizeCount) / ((double)_permutations);
		else
			return 1.0;
	}
}
