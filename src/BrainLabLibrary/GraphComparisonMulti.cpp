#include "GraphComparisonMulti.h"
#include "GraphComparison.h"
#include "Stats.h"

namespace BrainLabLibrary
{
	using namespace std;

	GraphComparisonMulti::GraphComparisonMulti(int subjectCount, int vertices, int edges, vector<string> &dataTypes)
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

		for(auto dataType : _dataTypes)
			_dataByType[dataType] = unique_ptr<GraphComparison>(new GraphComparison(subjectCount, vertices, edges));
	}
	
	GraphComparisonMulti::~GraphComparisonMulti(void)
	{}

	void GraphComparisonMulti::AddSubject(SubjectMarshal *itm)
	{
		// Loop through the graphs for this subject
		for(auto dataType : _dataTypes)
			_dataByType[dataType]->AddGraph( itm->Graphs[dataType] );

		// Add our subject idx to the proper group vector
		_subIdxsByGroup[itm->GroupId].push_back(_subCounter);

		// Make sure to increment the counter
		++_subCounter;
	}
	
	void GraphComparisonMulti::Compare(std::string &group1, std::string &group2, std::map<std::string, Threshold> &threshes)
	{
		// Put together a list of idxs representing our two groups
		vector<int> idxs;
		for(auto itm : _subIdxsByGroup[group1])
			idxs.push_back(itm);
		for(auto itm : _subIdxsByGroup[group2])
			idxs.push_back(itm);

		// Keep track of our group 1 size for the permutation step
		_group1Count = _subIdxsByGroup[group1].size();

		// Temporary map to hold our node counts
		vector<int> nodeCounts(_vertices);

		// Loop through our comparisons and call compare group passing our actual subject labels
		for(auto &dataItem : _dataByType)
		{
			// Compare the two groups for this data type
			Component cmp = dataItem.second->CompareGroups(idxs, _group1Count, threshes[dataItem.first].Value);

			// Pull out the vertices and store then in our counting map
			for(auto vert : cmp.Vertices)
				++nodeCounts[vert];
		}

		// Calculate how many nodes overlap between all of the data types
		int maxOverlap = _dataByType.size();
		for(auto nc=0; nc<nodeCounts.size();++nc)
		{
			_verticesById[nc] = shared_ptr<Vertex>(new Vertex());
			_verticesById[nc]->Id = nc;

			if(nodeCounts[nc] == maxOverlap)
			{
				_verticesById[nc]->IsFullOverlap = true;
				++_realOverlap;
			}
		}
	}

	void GraphComparisonMulti::Permute(const vector<vector<int>> &permutations, map<string, Threshold> &threshes)
	{
		//for(int i=0; i<permutations.size(); i++)
		parallel_for_each(begin(permutations), end(permutations), [=, &threshes] (const vector<int> &subIdxs)
		{	
			std::vector<int> nodeCounts(_vertices);

			// Loop through our comparisons and call permute on them passing our new random subject assortement
			//for(auto it=_dataByType.begin(); it!=_dataByType.end(); ++it)
			parallel_for_each(begin(_dataByType), end(_dataByType), [=, &subIdxs, &threshes, &nodeCounts] (pair<const string, unique_ptr<GraphComparison>> &item)
			{
				// Run the permutation for this index arrangement
				Component cmp = item.second->Permute(subIdxs, _group1Count, threshes[item.first].Value);

				// Pull out the vertices and store then in our counting map
				for(auto vert : cmp.Vertices)
					++nodeCounts[vert];
			});

			// Calculate how many nodes overlap between all of the nodes
			int permOverlap = 0, maxOverlap = _dataByType.size();
			for(auto nc=0; nc<nodeCounts.size();++nc)
			{
				if(nodeCounts[nc] == maxOverlap)
				{
					_verticesById[nc]->RandomOverlapCount++;
					++permOverlap;
				}
			}

			if(_overlapDistribution.count(permOverlap) == 0)
				_overlapDistribution[permOverlap] = 0;
			
			_overlapDistribution[permOverlap]++;

			// NBS multimodal compare
			if(permOverlap >= _realOverlap)
				++_rightTailOverlapCount;
		});
	}

	std::unique_ptr<Overlap> GraphComparisonMulti::GetOverlapResult()
	{
		unique_ptr<Overlap> overlap(new Overlap());

		for(auto &dataItem : _dataByType)
			dataItem.second->GetComponents(overlap->Components[dataItem.first]); // Ask the graph for the components

		for(auto vtx : _verticesById)
		{
			if(vtx.second->IsFullOverlap)
				overlap->Vertices.push_back(vtx.second);
		}

		overlap->RightTailOverlapCount = _rightTailOverlapCount;
		overlap->Distribution = _overlapDistribution;

		return overlap;
	}
}