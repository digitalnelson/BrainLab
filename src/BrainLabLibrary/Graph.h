#pragma once
#include "EdgeValue.h"
#include "GraphLookup.h"
#include <vector>
#include <map>
#include <boost/graph/adjacency_matrix.hpp>
#include <boost/graph/graph_utility.hpp>
#include <boost/graph/connected_components.hpp>
#include <boost/graph/iteration_macros.hpp>

namespace BrainLabNative
{
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
		int RightTailExtent;
	};

	class Graph
	{
	public:
		Graph(int nVerts, GraphLookup* lu);
		~Graph(void);

		typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;
		typedef std::pair<int, int> Edge;
		typedef std::vector<Edge> EdgeCollection;
		typedef std::vector<EdgeValue> EdgeValueCollection;

		typedef std::map<int, std::vector<int>> ComponentVertexCollection;   // Component id to component edge mapping
		typedef std::map<int, std::vector<ComponentEdge>> ComponentEdgeCollection;

		void AddEdge(int m, int n, EdgeValue val);
		EdgeValue GetEdge(int m, int n);

		void ComputeComponents();
		void GetComponents(std::vector<Component> &components);
		int GetLargestComponentId();
		int GetComponentExtent(int id);

		EdgeCollection Edges;
		EdgeValueCollection EdgeValues;
		
		int ComponentCount;
		ComponentVertexCollection ComponentVertices;
		ComponentEdgeCollection ComponentEdges;

	private:
		UDGraph _graph;
		GraphLookup * _lu;
		int _nVerts;
	};
}

