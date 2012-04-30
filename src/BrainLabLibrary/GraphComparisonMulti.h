#pragma once
#include <vector>
#include <map>
#include "Graph.h"

namespace BrainLabLibrary
{
	class GraphComparison;

	struct Subject
	{
		std::string SubjectId;
		std::string GroupId;

		std::map<std::string, Graph*> Graphs;
	};

	struct Threshold
	{
		std::string DataType;
		double Value;
	};

	struct Overlap
	{
		typedef std::map<std::string, std::vector<Component>> ComponentByTypeCollection;

		std::vector<int> Vertices;
		int RightTailOverlapCount;
		ComponentByTypeCollection Components;
	};

	class GraphComparisonMulti
	{
	public:
		GraphComparisonMulti(int subjectCount, int verts, int edges, std::vector<std::string> dataTypes);
		~GraphComparisonMulti(void);

		typedef std::map<std::string, std::vector<Component>> ComponentByTypeCollection;
		
		void AddSubject(Subject *itm);
		void Compare(std::string group1, std::string group2, std::map<std::string, Threshold> threshes);
		void Permute(int permutations, std::map<std::string, Threshold> threshes);

		Overlap GetOverlapResult();

	private:
		int _subjectCount;
		int _vertices;
		int _edges;
		int _subCounter;

		int _realOverlap;
		int _rightTailOverlapCount;
		int _permutations;

		int _group1Count;

		std::vector<std::string> _dataTypes;
		std::map<std::string, GraphComparison*> _dataByType;
		std::map<std::string, std::vector<int>> _subIdxsByGroup;
		std::vector<int> _overlapVertices;
	};
}

