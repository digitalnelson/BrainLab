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
		
		void GetEdgeTStats(std::vector<int>& idxs, int szGrp1, std::vector<EdgeValue>& edgeStats);
		
		void Permute(std::vector<int>& idxs, int szGrp1, double tStatThreshold, Graph &graph);
		void CompareGroups(std::vector<int>& idxs, int szGrp1, Graph& graph, double tStatThreshold);
		
		double GetComponentSizePVal(int cmpSize);
		double GetEdgePVal(int edgeIdx, int tstat);
	
	private:
		typedef boost::multi_array<double, 2> EdgesBySubject;
		typedef boost::multi_array<double, 2>::array_view<1>::type SingleEdgeBySubject;
		typedef boost::multi_array_types::index_range range;
		typedef std::vector<int> PermCollection;
		typedef std::vector<std::vector<double> > PermEdgeCollection;
		typedef std::vector<int> CmpEdgeCollection;

		int _subjectCount;
		int _currentSubjectIdx;
		int _vertCount;
		int _edgeCount;

		GraphLookup _lu;
		EdgesBySubject _subjectEdges;  // Mtx of edge vs subject  e.g. 4005x58
		PermCollection _componentSizes;
		PermEdgeCollection _edgeTStats;

		CmpEdgeCollection _grpEdgeCounts;
		vector<EdgeValue> _grpStats;
	};

}

