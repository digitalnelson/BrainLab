#pragma once
#include "Exports.h"
#include "EdgeValue.h"
#include "Graph.h"
#include "GraphLookup.h"
#include <vector>
#include <boost/multi_array.hpp>

namespace BrainLabNative
{	
	class GraphComparison
	{
	public:
		GraphComparison(int subjectCount, int verts, int edges);
		~GraphComparison(void);

		void AddGraph(Graph* graph);
		
		void CalcEdgeTStats(std::vector<int>& idxs, int szGrp1, std::vector<EdgeValue>& edgeStats);
		
		void CompareGroups(std::vector<int>& idxs, int szGrp1, double tStatThreshold, Graph& graph);
		void Permute(std::vector<int>& idxs, int szGrp1, double tStatThreshold, Graph &graph);
		
		double GetComponentSizePVal();
		double GetEdgePVal(int edgeIdx);
	
	private:
		typedef boost::multi_array<double, 2> EdgesBySubject;
		typedef boost::multi_array<double, 2>::array_view<1>::type SingleEdgeBySubject;
		typedef boost::multi_array_types::index_range range;

		int _subjectCount;
		int _currentSubjectIdx;
		int _vertCount;
		int _edgeCount;
		int _permutations;

		GraphLookup _lu;
		EdgesBySubject _subjectEdges;  // Mtx of edge vs subject  e.g. 4005x58

		std::vector<EdgeValue> _grpStats;
		std::vector<int> _grpEdgeCounts;

		int _largestComponentSize;
		int _rightTailComponentSizeCount;
	};
}

