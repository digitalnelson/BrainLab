#include "stdafx.h"
#include "Graph.h"
#include <exception>
#include <map>

namespace BrainLabNative
{
	Graph::Graph(int nVerts) : _graph(nVerts)
	{
		_nVerts = nVerts;
		ComponentCount = 0;
	}

	Graph::~Graph(void)
	{}

	void Graph::AddEdge(int i, int j, EdgeValue val)
	{
		if(i >= _nVerts)
			throw std::exception("Exceeded matrix bounds.  i >= number of verticies");
		if(j >= _nVerts)
			throw std::exception("Exceeded matrix bounds.  i >= _m");

		Edges.push_back(Edge(i, j));
		EdgeValues.push_back(val);

		boost::add_edge(i, j, _graph);
	}

	void Graph::ComputeComponents()
	{
		using namespace std;

		// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
		vector<int> cmpRawListingByVertex(boost::num_vertices(_graph));
		ComponentCount = boost::connected_components(_graph, &cmpRawListingByVertex[0]);
		
		map<int, int> componentVertexLookup;

		//// Loop through and gather all of the vertex ids for each component
		for(int i=0; i<cmpRawListingByVertex.size(); ++i)
			ComponentVertices[cmpRawListingByVertex[i]].push_back(i);

		// Store the edges by component
		for(int i=0; i<Edges.size(); ++i)
		{
			ComponentEdge ce;
			ce.Edge = Edges[i];
			ce.EdgeValue = EdgeValues[i];

			// Look up the first edge vertex
			int cmpId = cmpRawListingByVertex[ce.Edge.first];
			
			// Add our values to the component maps
			ComponentEdges[cmpId].push_back(ce);
		}
	}

	void Graph::GetComponents(std::vector<Component> &components)
	{
		components.resize(ComponentEdges.size());

		int idx = 0;
		for(auto it=ComponentEdges.begin(); it != ComponentEdges.end(); ++it, ++idx)
		{
			components[idx].Identifier = it->first;
			
			for(auto ei=it->second.begin(); ei < it->second.end(); ++ei)
			{
				components[idx].Edges.push_back(*ei);
			}
		}

		//idx = 0;
		//for(auto it=ComponentVertices.begin(); it != ComponentVertices.end(); ++it, ++idx)
		//{
		//	for(auto ei=it->second.begin(); ei < it->second.end(); ++ei)
		//	{
		//		components[idx].Vertices.push_back(*ei);
		//	}
		//}
	}

	int Graph::GetLargestComponentId()
	{
		int maxEdges = 0, maxId = 0;
		for(auto it=ComponentEdges.begin(); it != ComponentEdges.end(); ++it)
		{
			int edges = it->second.size();

			if(edges > maxEdges)
			{
				maxEdges = edges;
				maxId = it->first;
			}
		}

		return maxId;
	}

	int Graph::GetComponentExtent(int id)
	{
		return ComponentEdges[id].size();
	}
}
