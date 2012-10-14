#pragma once
#include "EdgeValue.h"
#include "Graph.h"
#include "GraphLookup.h"
#include <vector>
#include <amp.h>
#include <boost/multi_array.hpp>
#include <boost/graph/adjacency_matrix.hpp>
#include <boost/graph/graph_utility.hpp>
#include <boost/graph/connected_components.hpp>
#include <boost/graph/iteration_macros.hpp>

namespace BrainLabLibrary
{	
	using namespace std;

	class GraphComparison
	{
	public:
		GraphComparison(int subjectCount, int verts, int edges);
		~GraphComparison(void);

		void AddGraph(Graph* graph);
		
		void CalcEdgeTStats(const vector<int> &idxs, int szGrp1, vector<EdgeValue> &edgeStats);
		void CompareGroups(vector<int> &idxs, int szGrp1, double tStatThreshold, vector<int> &vertexList);
		void Permute(const vector<int> &idxs, int szGrp1, double tStatThreshold, vector<int> &vertexList);
		void GetComponents(vector<Component> &components);
	
	private:
		typedef boost::multi_array<float, 2> EdgesBySubject;
		typedef boost::multi_array<float, 2>::array_view<1>::type SingleEdgeBySubject;
		typedef boost::multi_array_types::index_range range;
		typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;

		void ComputeComponents(UDGraph &graph, vector<int> &edgeIdxs, vector<Component> &components);

		int _subjectCount;
		int _currentSubjectIdx;
		int _vertCount;
		int _edgeCount;
		int _permutations;

		GraphLookup _lu;
		EdgesBySubject _subjectEdges;  // Mtx of edge vs subject  e.g. 4005x58

		vector<float> _allEdges;

		UDGraph _graph;
		//Graph _g;
		
		vector<EdgeValue> _grpStats;
		vector<int> _grpSupraThreshEdgeIdxs;
		vector<Component> _grpComponent;

		int _largestComponentSize;
		int _rightTailComponentSizeCount;
	};

	struct ConnectionStats
	{
		int Idx;

		float Value;

		float M1;
		float M2;

		float V1;
		float V2;
	
		float TStat;
	
		float PValue;
		int RightTailCount;
	};
}

