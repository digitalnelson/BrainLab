#include "stdafx.h"
#include "GraphComparisonMulti.h"
#include "Stats.h"

namespace BrainLabNative
{
	GraphComparisonMulti::GraphComparisonMulti(int subjectCount, int vertices, int edges, std::vector<std::string> dataTypes)
	{
		_subjectCount = subjectCount;
		_vertices = vertices;
		_edges = edges;
		_dataTypes = dataTypes;
		_subCounter = 0;

		_permutations = 0;
		_realOverlap = 0;
		_rightTailOverlapCount = 0;

		for(auto it=_dataTypes.begin(); it<_dataTypes.end(); ++it)
			_dataByType[*it] = new GraphComparison(subjectCount, vertices, edges);
	}
	
	GraphComparisonMulti::~GraphComparisonMulti(void)
	{
		for(auto it=_dataTypes.begin(); it<_dataTypes.end(); ++it)
			delete _dataByType[*it];
	}

	void GraphComparisonMulti::AddSubject(Subject *itm)
	{
		// Loop through the graphs for this subject
		for(auto it=_dataTypes.begin(); it<_dataTypes.end(); ++it)
			_dataByType[*it]->AddGraph( itm->Graphs[*it] );

		// Add our subject idx to the proper group vector
		_subIdxsByGroup[itm->GroupId].push_back(_subCounter);

		// Make sure to increment the counter
		++_subCounter;
	}
	
	void GraphComparisonMulti::Compare(std::string group1, std::string group2, std::map<std::string, Threshold> threshes)
	{
		// Put together a list of idxs representing our two groups
		std::vector<int> idxs;
		for each(int itm in _subIdxsByGroup[group1])
			idxs.push_back(itm);
		for each(int itm in _subIdxsByGroup[group2])
			idxs.push_back(itm);

		// Temporary map to hold our node counts
		std::map<int, int> nodeCounts;

		// Loop through our comparisons and call compare group passing our actual subject labels
		for(auto it=_dataByType.begin(); it!=_dataByType.end(); ++it)
		{
			Graph g(_vertices);

			// Grab the graph comparison for this data type
			auto compare = it->second;

			// Run the permutation for this index arrangement
			compare->CompareGroups(idxs, _subIdxsByGroup[group1].size(), threshes[it->first].Value, g);

			// Compute our components
			g.ComputeComponents();

			// Get the index of the largest component
			int idx = g.GetLargestComponentId();

			// Get verts
			auto cvV = g.ComponentVertices[idx];

			// Pull out the vertices and store then in our counting map
			for(auto cv=cvV.begin(); cv<cvV.end(); ++cv)
			{
				if(nodeCounts.find(*cv) != nodeCounts.end())
					nodeCounts[*cv]++;
				else
					nodeCounts[*cv] = 1;
			}

			// Ask the graph for the components
			g.GetComponents(_overlap.Components[it->first]);
		}

		// Calculate how many nodes overlap between all of the nodes
		int maxOverlap = _dataByType.size();
		for(auto nc=nodeCounts.begin(); nc!=nodeCounts.end();++nc)
		{
			if(nc->second == maxOverlap)
			{
				++_realOverlap;
				_overlap.Vertices.push_back(nc->first);
			}
		}
	}

	void GraphComparisonMulti::Permute(int permutations, int group1Size, std::map<std::string, Threshold> threshes)
	{
		// Create a simple index vector
		std::vector<int> idxs(_subjectCount);
		
		// Fill with values 0..subjectCount
		BrainLabNative::Stats::FillVectorInc(idxs);

		for(int i=0; i<permutations; i++)
		{	
			// Shuffle the indexes
			random_shuffle(idxs.begin(), idxs.end());

			std::map<int, int> nodeCounts;

			// Loop through our comparisons and call permute on them passing our new random
			// subject assortement
			for(auto it=_dataByType.begin(); it!=_dataByType.end(); ++it)
			{
				Graph g(_vertices);

				// Grab the graph comparison for this data type
				auto compare = it->second;

				// Run the permutation for this index arrangement
				compare->Permute(idxs, group1Size, threshes[it->first].Value, g);

				// Get the index of the largest component
				int id = g.GetLargestComponentId();

				// Pull out the vertices and store then in our counting map
				for(auto it=g.ComponentVertices[id].begin(); it<g.ComponentVertices[id].end(); ++it)
				{
					if(nodeCounts.find(*it) != nodeCounts.end())
						nodeCounts[*it]++;
					else
						nodeCounts[*it] = 1;
				}
			}

			// Calculate how many nodes overlap between all of the nodes
			int permOverlap = 0, maxOverlap = _dataByType.size();
			for(auto it=nodeCounts.begin(); it!=nodeCounts.end();++it)
			{
				if(it->second == maxOverlap)
					++permOverlap;
			}

			if(permOverlap >= _realOverlap)
				++_rightTailOverlapCount;
		}
	}

	Overlap GraphComparisonMulti::GetOverlapResult()
	{
		return _overlap;
	}

	double GraphComparisonMulti::GetComponentSizePVal(std::string dataType)
	{
		return _dataByType[dataType]->GetComponentSizePVal();
	}

	double GraphComparisonMulti::GetOverlapSizePVal()
	{
		if(_permutations > 0)
			return ((double)_rightTailOverlapCount) / ((double)_permutations);
		else
			return 1.0;
	}
}