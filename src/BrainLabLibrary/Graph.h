#pragma once
#include "EdgeValue.h"
#include "GraphLookup.h"
#include <vector>
#include <map>
#include <boost/graph/adjacency_matrix.hpp>
#include <boost/graph/graph_utility.hpp>
#include <boost/graph/connected_components.hpp>
#include <boost/graph/iteration_macros.hpp>

namespace BrainLabLibrary
{
	using namespace std;

	struct EdgeValue
	{
		EdgeValue()
		{
			Idx = 0;
			Value = 0;
			M1 = 0;
			M2 = 0;
			V1 = 0;
			V2 = 0;
			TStat = 0;
			PValue = 0;
			RightTailCount = 0;
		}

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

	typedef std::pair<int, int> Edge;
	//struct Edge
	//{
	//	int EdgeIndex;
	//	pair<int, int> Vertices;

	//	float Value;
	//};

	struct ComponentEdge
	{
		int ComponentIndex;

		Edge Edge;
		int EdgeIndex;

		EdgeValue EdgeValue;
	};

	struct Component
	{
		int Identifier;
		vector<ComponentEdge> Edges;
		vector<int> Vertices;

		int RightTailExtent;
	};

	class Graph
	{
	public:
		typedef vector<Edge> EdgeCollection;
		typedef vector<EdgeValue> EdgeValueCollection;

		typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;
		typedef map<int, vector<int>> ComponentVertexCollection;   // Component id to component edge mapping
		typedef map<int, vector<ComponentEdge>> ComponentEdgeCollection;

		Graph(int nVerts, GraphLookup* lu);
		~Graph(void);

		void AddEdge(int m, int n, EdgeValue val);
		EdgeValue GetEdge(int m, int n);

		float GlobalStrength();
		void GetMeanVtxStrength(vector<float> &meanVtxStr);

		void ComputeComponents();
		void GetComponents(vector<Component> &components);
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

