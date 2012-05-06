#include "Graph.h"
#include <exception>
#include <map>

namespace BrainLabLibrary
{
	Graph::Graph(int nVerts, GraphLookup* lu) : _graph(nVerts)
	{
		_nVerts = nVerts;
		_lu = lu;
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

	EdgeValue Graph::GetEdge(int i, int j)
	{
		int idx = _lu->GetEdge(i, j);
		return EdgeValues[idx];
	}

	float Graph::GlobalStrength()
	{
		std::vector<float> roiStrs;

		for(int vert=0; vert<_nVerts; vert++)
		{
			float gsRoi = 0;
			for(int overt=0; overt<_nVerts; overt++)
			{
				if(vert != overt)
				{
					int idx = _lu->GetEdge(vert, overt);
					gsRoi += EdgeValues[idx].Value;
				}
			}

			roiStrs.push_back(gsRoi / (_nVerts - 1));
		}

		float gs = 0;

		for (auto i : roiStrs)
		{
			gs += i;
		}

		return gs / _nVerts;
	}

	void Graph::ComputeComponents()
	{
		using namespace std;

		// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
		vector<int> cmpRawListingByVertex(boost::num_vertices(_graph));
		ComponentCount = boost::connected_components(_graph, &cmpRawListingByVertex[0]);
		
		map<int, int> componentVertexLookup;

		// Loop through and gather all of the vertex ids for each component
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
			
			// Add edge to component edge map
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
