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
		: _subjectEdges(boost::extents[edges][subjectCount]), _lu(verts), _grpStats(edges)
	{
		_subjectCount = subjectCount;
		_vertCount = verts;
		_edgeCount = edges;
		_currentSubjectIdx = 0;
		_permutations = 0;
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

	void GraphComparison::CompareGroups(vector<int>& idxs, int szGrp1, double tStatThreshold, Graph& graph)
	{				
		// Calculate t stats for this subject labeling
		CalcEdgeTStats(idxs, szGrp1, _grpStats);

		// Load graph with thresholded t stats
		for(auto idx=0; idx<_grpStats.size(); ++idx)
		{
			std::pair<int, int> edge = _lu.GetEdge(idx);

			if(abs(_grpStats[idx].TStat) > tStatThreshold)
				graph.AddEdge(edge.first, edge.second, _grpStats[idx]);
		}

		// Calculate component count and max topological extent
		graph.ComputeComponents();

		// Get the index of the largest cmp
		int id = graph.GetLargestComponentId();

		// Keep this
		_largestComponentSize = graph.GetComponentExtent(id);
	}

	void GraphComparison::Permute(vector<int>& idxs, int szGrp1, double tStatThreshold, Graph &graph)
	{
		// Create somewhere to store our t stats
		vector<EdgeValue> permEdgeStats(_edgeCount);

		// Calculate t stats for this random subject labeling
		CalcEdgeTStats(idxs, szGrp1, permEdgeStats);

		// Loop through edge stats and calc	our measures
		for(auto idx=0; idx<permEdgeStats.size(); ++idx)
		{
			// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
			if(abs(permEdgeStats[idx].TStat) >= abs(_grpStats[idx].TStat))
				_grpEdgeCounts[idx]++;

			// TODO: Eventually store this as a graph tag so we can have multiple pieces of info stored with our edges
			// NBS level testing - Add this to the NBS graph if above our threshold, we will calc the size of the cmp soon
			if(abs(permEdgeStats[idx].TStat) > tStatThreshold)
			{
				// Lookup our edge
				std::pair<int, int> edge = _lu.GetEdge(idx);
				// Store the edge in our graph
				graph.AddEdge(edge.first, edge.second, permEdgeStats[idx]);
			}
		}
			
		// Calculate component count and max topological extent
		graph.ComputeComponents();

		// Get the index of the largest cmp
		int id = graph.GetLargestComponentId();

		// Keep this max for the NBS distribution
		int cmpSize = graph.GetComponentExtent(id);

		// Increment rt tail if this is larger than the actual component
		if(cmpSize >= _largestComponentSize)
			++_rightTailComponentSizeCount;

		++_permutations;
	}

	double GraphComparison::GetComponentSizePVal()
	{
		if(_permutations > 0)
			return ((double)_rightTailComponentSizeCount) / ((double)_permutations);
		else
			return 1.0;
	}

	double GraphComparison::GetEdgePVal(int edgeIdx)
	{
		if(_permutations > 0)
			return ((double)_grpEdgeCounts[edgeIdx]) / ((double)_permutations);
		else
			return 1.0;
	}
}
