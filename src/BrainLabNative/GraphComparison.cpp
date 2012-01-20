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
		: _subjectEdges(boost::extents[edges][subjectCount]), _lu(verts), _edgeTStats(edges), _grpStats(edges)
	{
		_subjectCount = subjectCount;
		_vertCount = verts;
		_edgeCount = edges;
		_currentSubjectIdx = 0;
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

	void GraphComparison::GetEdgeTStats(vector<int>& idxs, int szGrp1, vector<EdgeValue>& edgeStats)
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

	void GraphComparison::Permute(vector<int>& idxs, int szGrp1, double tStatThreshold, Graph &graph)
	{
		// Create somewhere to store our t stats
		vector<EdgeValue> edgeStats(_edgeCount);

		// Calculate t stats for this random subject labeling
		GetEdgeTStats(idxs, szGrp1, edgeStats);

		int size = (int)edgeStats.size();

		// Loop through edge stats and calc	our measures
		for(auto idx=0; idx<edgeStats.size(); ++idx)
		//parallel_for(0, size, [=, &idxs, &edgeStats, &graph] (int idx)
		{
			std::pair<int, int> edge = _lu.GetEdge(idx);

			// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
			if(abs(edgeStats[idx].TStat) >= _grpStats[idx].TStat)
				_grpEdgeCounts[idx]++;

			// Save this t stat in the distro store
			//_edgeTStats[idx].push_back(edgeStats[idx].TStat);

			// NBS level testing - Add this to the NBS graph if above our threshold, we will calc the size of the cmp soon
			if(abs(edgeStats[idx].TStat) > tStatThreshold)
				graph.AddEdge(edge.first, edge.second, edgeStats[idx]);
		}//);
			
		// Calculate component count and max topological extent
		graph.ComputeComponents();

		// Get the index of the largest cmp
		int id = graph.GetLargestComponentId();

		// Keep this max for the NBS distribution
		_componentSizes.push_back(graph.GetComponentExtent(id));
	}

	void GraphComparison::CompareGroups(vector<int>& idxs, int szGrp1, Graph& graph, double tStatThreshold)
	{				
		// Create somewhere to store our t stats
		//vector<EdgeValue> edgeStats(_edgeCount);

		// Calculate t stats for this subject labeling
		GetEdgeTStats(idxs, szGrp1, _grpStats);

		// Load graph with thresholded t stats
		for(auto idx=0; idx<_grpStats.size(); ++idx)
		{
			std::pair<int, int> edge = _lu.GetEdge(idx);

			//_grpStats[idx].PValue = GetEdgePVal(idx, _grpStats[idx].TStat);

			if(abs(_grpStats[idx].TStat) > tStatThreshold)
				graph.AddEdge(edge.first, edge.second, _grpStats[idx]);
		}
	}

	double GraphComparison::GetComponentSizePVal(int cmpSize)
	{
		int count = 0;
		for(auto it=_componentSizes.begin(); it<_componentSizes.end(); ++it)
		{
			if(*it > cmpSize)
				++count;
		}

		return ((double)count) / ((double)_componentSizes.size());
	}

	double GraphComparison::GetEdgePVal(int edgeIdx, int tstat)
	{
		int count = 0;
		for(auto it=_edgeTStats[edgeIdx].begin(); it<_edgeTStats[edgeIdx].end(); ++it)
		{
			if(*it > tstat)
				++count;
		}

		return ((double)count) / ((double)_edgeTStats[edgeIdx].size());
	}
}
