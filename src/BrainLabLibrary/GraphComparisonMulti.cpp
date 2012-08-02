#include "GraphComparisonMulti.h"
#include "GraphComparison.h"
#include "Stats.h"

namespace BrainLabLibrary
{
	GraphComparisonMulti::GraphComparisonMulti(int subjectCount, int vertices, int edges, std::vector<std::string> dataTypes)
	{
		_subjectCount = subjectCount;
		_vertices = vertices;
		_edges = edges;
		_dataTypes = dataTypes;
		_subCounter = 0;

		srand(time(0));

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

	void GraphComparisonMulti::AddSubject(SubjectMarshal *itm)
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

		// Keep track of our group 1 size for the permutation step
		_group1Count = _subIdxsByGroup[group1].size();

		// Temporary map to hold our node counts
		std::vector<int> nodeCounts(_vertices);

		// Loop through our comparisons and call compare group passing our actual subject labels
		for(auto it=_dataByType.begin(); it!=_dataByType.end(); ++it)
		{
			std::vector<int> vertList;

			// Grab the graph comparison for this data type
			auto compare = it->second;

			// Run the permutation for this index arrangement
			compare->CompareGroups(idxs, _group1Count, threshes[it->first].Value, vertList);

			// Pull out the vertices and store then in our counting map
			for(auto cv=0; cv<vertList.size(); ++cv)
			{
				if(vertList[cv] == 1)
					++nodeCounts[cv];
			}
		}

		// Calculate how many nodes overlap between all of the nodes
		int maxOverlap = _dataByType.size();
		for(auto nc=0; nc<nodeCounts.size();++nc)
		{
			if(nodeCounts[nc] == maxOverlap)
			{
				++_realOverlap;
				_overlapVertices.push_back(nc);
			}
		}
	}

	void GraphComparisonMulti::Permute(int permutations, std::map<std::string, Threshold> threshes)
	{
		// Create a simple index vector
		std::vector<int> idxs(_subjectCount);
		
		// Fill with values 0..subjectCount
		Stats::FillVectorInc(idxs);

		for(int i=0; i<permutations; i++)
		//parallel_for(0, permutations, [=, &idxs, &threshes] (int i)
		{	
			// Shuffle the indexes
			random_shuffle(idxs.begin(), idxs.end());

			std::vector<int> nodeCounts(_vertices);

			// Loop through our comparisons and call permute on them passing our new random
			// subject assortement
			for(auto it=_dataByType.begin(); it!=_dataByType.end(); ++it)
			{
				std::vector<int> vertList;

				// Grab the graph comparison for this data type
				auto compare = it->second;

				// Run the permutation for this index arrangement
				compare->Permute(idxs, _group1Count, threshes[it->first].Value, vertList);

				// Pull out the vertices and store then in our counting map
				for(auto cv=0; cv<vertList.size(); ++cv)
				{
					if(vertList[cv] == 1)
						++nodeCounts[cv];
				}
			}

			// Calculate how many nodes overlap between all of the nodes
			int permOverlap = 0, maxOverlap = _dataByType.size();
			for(auto nc=0; nc<nodeCounts.size();++nc)
			{
				if(nodeCounts[nc] == maxOverlap)
					++permOverlap;
			}
			
			// NBS multimodal compare
			if(permOverlap >= _realOverlap)
				++_rightTailOverlapCount;
		}//);
	}

	Overlap GraphComparisonMulti::GetOverlapResult()
	{
		Overlap overlap;

		for(auto it=_dataByType.begin(); it!=_dataByType.end(); ++it)
			it->second->GetComponents(overlap.Components[it->first]); // Ask the graph for the components

		for(auto it=_overlapVertices.begin(); it<_overlapVertices.end(); it++)
			overlap.Vertices.push_back(*it);

		overlap.RightTailOverlapCount = _rightTailOverlapCount;

		return overlap;
	}
}