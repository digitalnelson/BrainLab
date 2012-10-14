#pragma once
#include <memory>
#include <vector>
#include <map>
#include <boost/graph/adjacency_matrix.hpp>
#include <boost/graph/graph_utility.hpp>
#include <boost/graph/connected_components.hpp>
#include <boost/graph/iteration_macros.hpp>

namespace BrainLabLibrary
{
	using namespace std;

	//typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;
	//typedef map<int, vector<int>> ComponentVertexCollection;   // Component id to component edge mapping
	//typedef map<int, vector<Edge>> ComponentEdgeCollection;

	//struct Edge
	//{
	//	int EdgeIndex;
	//	pair<int, int> Vertices;

	//	float Value;
	//};

	//struct Component
	//{
	//	int Identifier;
	//	vector<Edge> Edges;
	//	vector<int> Vertices;

	//	int RightTailExtent;
	//};

	//struct Connection
	//{
	//	int FlatIndex;
	//	pair<int, int> Indices;

	//	float Value;
	//};

	//class ConnData
	//{
	//public:
	//	ConnData(int indices);

	//	void AddConnection(int i, int j, float value);
	//	//shared_ptr<Edge> GetEdge(int edgeIndex);
	//	//shared_ptr<Edge> GetEdge(int i, int j);

	//	float GlobalStrength();
	//	void GetMeanVtxStrength(vector<float> &meanVtxStr);

	//	void ComputeComponents();
	//	void GetComponents(vector<Component> &components);
	//	int GetLargestComponentId();
	//	int GetComponentExtent(int id);

	//private:
	//	int _indices;

	//	// List of all the edges
	//	vector<shared_ptr<Connection>> _connList;

	//	// Boost adj mtx for calcs
	//	boost::adjacency_matrix<boost::undirectedS> _adjMtx;
	//};

	//ConnData::ConnData(int indices) : _adjMtx(indices)
	//{
	//	_indices = indices;
	//	_connList.resize( (_indices * (_indices - 1)) / 2 );  // Only need to store upper triangle
	//}

	//void ConnData::AddConnection(int i, int j, float value)
	//{
	//	shared_ptr<Connection> conn(new Connection());
	//	conn->Indices.first = i;
	//	conn->Indices.second = j;
	//	conn->Value = value;

	//	// Add to our connection list
	//	_connList.push_back(conn);
	//	// Add to our boost graph
	//	boost::add_edge(i, j, _adjMtx);
	//}

	////EdgeValue ConnData::GetEdge(int i, int j)
	////{
	////	int idx = _lu->GetEdge(i, j);
	////	return EdgeValues[idx];
	////}

	//float ConnData::GlobalStrength()
	//{
	//	/*std::vector<float> roiStrs;

	//	for(int vert=0; vert<_nVerts; vert++)
	//	{
	//		float gsRoi = 0;
	//		for(int overt=0; overt<_nVerts; overt++)
	//		{
	//			if(vert != overt)
	//			{
	//				int idx = _lu->GetEdge(vert, overt);
	//				gsRoi += EdgeValues[idx].Value;
	//			}
	//		}

	//		roiStrs.push_back(gsRoi / (_nVerts - 1));
	//	}

	//	float gs = 0;

	//	for (auto i : roiStrs)
	//	{
	//		gs += i;
	//	}

	//	return gs / _nVerts;*/
	//}

	//void ConnData::GetMeanVtxStrength(std::vector<float> &meanVtxStr)
	//{
	//	/*for(int vert=0; vert<_nVerts; vert++)
	//	{
	//		float gsRoi = 0;
	//		for(int overt=0; overt<_nVerts; overt++)
	//		{
	//			if(vert != overt)
	//			{
	//				int idx = _lu->GetEdge(vert, overt);
	//				gsRoi += EdgeValues[idx].Value;
	//			}
	//		}

	//		meanVtxStr.push_back(gsRoi / (_nVerts - 1));
	//	}*/
	//}


	//void ConnData::ComputeComponents()
	//{
	//	//using namespace std;

	//	//// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
	//	//vector<int> cmpRawListingByVertex(boost::num_vertices(_graph));
	//	//ComponentCount = boost::connected_components(_graph, &cmpRawListingByVertex[0]);
	//	//
	//	//map<int, int> componentVertexLookup;

	//	//// Loop through and gather all of the vertex ids for each component
	//	//for(int i=0; i<cmpRawListingByVertex.size(); ++i)
	//	//	ComponentVertices[cmpRawListingByVertex[i]].push_back(i);

	//	//// Store the edges by component
	//	//for(int i=0; i<Edges.size(); ++i)
	//	//{
	//	//	ComponentEdge ce;
	//	//	ce.Edge = Edges[i];
	//	//	ce.EdgeValue = EdgeValues[i];

	//	//	// Look up the first edge vertex
	//	//	int cmpId = cmpRawListingByVertex[ce.Edge.first];
	//	//	
	//	//	// Add edge to component edge map
	//	//	ComponentEdges[cmpId].push_back(ce);
	//	//}
	//}

	//void ConnData::GetComponents(std::vector<Component> &components)
	//{
	//	/*components.resize(ComponentEdges.size());

	//	int idx = 0;
	//	for(auto it=ComponentEdges.begin(); it != ComponentEdges.end(); ++it, ++idx)
	//	{
	//		components[idx].Identifier = it->first;
	//		
	//		for(auto ei=it->second.begin(); ei < it->second.end(); ++ei)
	//		{
	//			components[idx].Edges.push_back(*ei);
	//		}
	//	}*/
	//}

	//int ConnData::GetLargestComponentId()
	//{
	//	//int maxEdges = 0, maxId = 0;
	//	//for(auto it=ComponentEdges.begin(); it != ComponentEdges.end(); ++it)
	//	//{
	//	//	int edges = it->second.size();

	//	//	if(edges > maxEdges)
	//	//	{
	//	//		maxEdges = edges;
	//	//		maxId = it->first;
	//	//	}
	//	//}

	//	return 0; //maxId;
	//}

	//int ConnData::GetComponentExtent(int id)
	//{
	//	return 0; //ComponentEdges[id].size();
	//}
}

