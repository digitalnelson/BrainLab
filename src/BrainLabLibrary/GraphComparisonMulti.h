#pragma once
#include <vector>
#include <map>
#include "Graph.h"
//#include "Subject.h"

namespace BrainLabLibrary
{
	using namespace std;

	class GraphComparison;

	struct SubjectMarshal
	{
		string SubjectId;
		string GroupId;

		map<string, Graph*> Graphs;
	};

	struct Threshold
	{
		string DataType;
		double Value;
	};

	struct Overlap
	{
		typedef map<string, vector<Component>> ComponentByTypeCollection;

		vector<int> Vertices;
		int RightTailOverlapCount;
		ComponentByTypeCollection Components;
	};

	class GraphComparisonMulti
	{
	public:
		GraphComparisonMulti(int subjectCount, int verts, int edges, vector<string> &dataTypes);
		~GraphComparisonMulti(void);

		typedef map<string, vector<Component>> ComponentByTypeCollection;
		
		void AddSubject(SubjectMarshal *itm);
		void Compare(string group1, string group2, map<string, Threshold> threshes);
		void Permute(const vector<vector<int>> &permutations, map<string, Threshold> &threshes);

		unique_ptr<Overlap> GetOverlapResult();

	private:
		int _subjectCount;
		int _vertices;
		int _edges;
		int _subCounter;

		int _realOverlap;
		int _rightTailOverlapCount;
		int _permutations;

		int _group1Count;

		vector<string> _dataTypes;
		map<string, unique_ptr<GraphComparison>> _dataByType;
		map<string, std::vector<int>> _subIdxsByGroup;
		vector<int> _overlapVertices;
	};
}

