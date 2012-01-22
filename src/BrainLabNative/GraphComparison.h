#pragma once
#include "Exports.h"
#include "EdgeValue.h"
#include "Graph.h"
#include "GraphLookup.h"
#include <vector>
#include <boost/multi_array.hpp>
#include <boost/graph/adjacency_matrix.hpp>
#include <boost/graph/graph_utility.hpp>

#include <boost/graph/connected_components.hpp>
#include <boost/graph/iteration_macros.hpp>

namespace BrainLabNative
{	
	class GraphComparison
	{
	public:
		GraphComparison(int subjectCount, int verts, int edges);
		~GraphComparison(void);

		typedef std::pair<int, int> Edge;
		typedef std::vector<Edge> EdgeCollection;

		struct ComponentEdge
		{
			Edge Edge;
			EdgeValue EdgeValue;
		};

		struct Component
		{
			int Identifier;
			std::vector<ComponentEdge> Edges;
			std::vector<int> Vertices;
		};

		void AddGraph(Graph* graph);
		
		void CalcEdgeTStats(std::vector<int> &idxs, int szGrp1, std::vector<EdgeValue> &edgeStats);
		
		void CompareGroups(std::vector<int> &idxs, int szGrp1, double tStatThreshold, std::vector<int> &vertexList);
		void Permute(std::vector<int> &idxs, int szGrp1, double tStatThreshold, std::vector<int> &vertexList);

		void GetComponents(std::vector<Component> &components);
		
		double GetComponentSizePVal();
		double GetEdgePVal(int edgeIdx);
	
	private:
		typedef boost::multi_array<double, 2> EdgesBySubject;
		typedef boost::multi_array<double, 2>::array_view<1>::type SingleEdgeBySubject;
		typedef boost::multi_array_types::index_range range;

		typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;

		void ComputeComponents(UDGraph &graph, std::vector<int> &edgeIdxs, std::vector<int> &components, std::vector<int> &vertexList);

		int _subjectCount;
		int _currentSubjectIdx;
		int _vertCount;
		int _edgeCount;
		int _permutations;

		GraphLookup _lu;
		EdgesBySubject _subjectEdges;  // Mtx of edge vs subject  e.g. 4005x58

		UDGraph _graph;
		Graph _g;
		std::vector<EdgeValue> _grpStats;
		std::vector<int> _grpSupraThreshEdgeIdxs;
		std::vector<int> _grpComponent;

		int _largestComponentSize;
		int _rightTailComponentSizeCount;
	};
}

