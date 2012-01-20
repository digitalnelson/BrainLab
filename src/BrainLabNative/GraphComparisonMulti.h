#pragma once

#include <vector>
#include <map>
#include "Graph.h"
#include "GraphComparison.h"

namespace BrainLabNative
{
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
		typedef std::map<std::string, std::vector<BrainLabNative::Graph::Component>> ComponentByTypeCollection;

		std::vector<int> Vertices;
		double PValue;
		ComponentByTypeCollection Components;
	};

	class GraphComparisonMulti
	{
	public:
		GraphComparisonMulti(int subjectCount, int verts, int edges, std::vector<std::string> dataTypes);
		~GraphComparisonMulti(void);

		typedef std::map<std::string, std::vector<BrainLabNative::Graph::Component>> ComponentByTypeCollection;
		
		void AddSubject(Subject *itm);
		void Compare(std::string group1, std::string group2, std::map<std::string, Threshold> threshes);
		void Permute(int permutations, int group1Size, std::map<std::string, Threshold> threshes);

		Overlap GetOverlapResult();
		double GetComponentSizePVal(std::string);
		double GetOverlapSizePVal();

	private:
		int _subjectCount;
		int _vertices;
		int _edges;
		int _subCounter;

		int _realOverlap;
		int _rightTailOverlapCount;
		int _permutations;

		std::vector<std::string> _dataTypes;
		std::map<std::string, GraphComparison*> _dataByType;
		std::map<std::string, std::vector<int>> _subIdxsByGroup;

		Overlap _overlap;
	};
}

