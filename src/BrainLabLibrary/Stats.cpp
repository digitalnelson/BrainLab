#include "Stats.h"
#include <iostream>
#include <math.h>
#include <amp.h>
#include <map>

#include <boost/graph/adjacency_matrix.hpp>
#include <boost/graph/connected_components.hpp>
#include <boost/graph/iteration_macros.hpp>

namespace BrainLabNative
{
	Stats::Stats(void)
	{
	}

	Stats::~Stats(void)
	{
	}
	
	//void Stats::Permute(float* edges, int edgeCount, int subjectCount, float* tstats)
	//{
	//	//using namespace std;
	//	//using namespace concurrency;

	//	//try
	//	//{
	//	//	// Create a view over the data on the CPU
	//	//	array_view<float, 2> edgeView(edgeCount, subjectCount, &edges[0]);
	//	//	array_view<float, 1> tstatView(edgeCount, &tstats[0]);

	//	//	std::vector<float> m1E(edgeCount);
	//	//	array_view<float, 1> m1EV(edgeCount, &m1E[0]);

	//	//	std::vector<float> m2E(edgeCount);
	//	//	array_view<float, 1> m2EV(edgeCount, &m2E[0]);

	//	//	std::vector<float> vc1E(edgeCount);
	//	//	array_view<float, 1> vc1EV(edgeCount, &vc1E[0]);

	//	//	std::vector<float> vc2E(edgeCount);
	//	//	array_view<float, 1> vc2EV(edgeCount, &vc2E[0]);

	//	//	// Run code on the GPU
	//	//	parallel_for_each(tstatView.grid, [=] (index<1> idx) mutable restrict(direct3d)
	//	//	{	
	//	//		float v1 = 0;
	//	//		float v2 = 0;
	//	//		float t1 = 0;
	//	//		float t2 = 0;
	//	//		int j1 = 0;
	//	//		int j2 = 0;
	//	//		float m1 = 0;
	//	//		float m2 = 0;
	//	//		float s1 = 0;
	//	//		float s2 = 0;

	//	//		m1EV(idx[0]) = 5;

	//	//		for (int subjectIdx = 0; subjectIdx < subjectCount; ++subjectIdx)
	//	//		{
	//	//			float edgeVal = edgeView(idx[0], subjectIdx);

	//	//			if (subjectIdx < 29)
	//	//			{
	//	//				if (subjectIdx == 0)
	//	//				{
	//	//					j1++;
	//	//					t1 = edgeVal;
	//	//				}
	//	//				else
	//	//				{
	//	//					j1++;
	//	//					float xi = edgeVal;
	//	//					t1 += xi;
	//	//					float diff = (j1 * xi) - t1;
	//	//					v1 += (diff * diff) / (j1 * (j1 - 1));
	//	//				}

	//	//				m1 += edgeVal;
	//	//			}
	//	//			else
	//	//			{
	//	//				if (subjectIdx == 29)
	//	//				{
	//	//					j2++;
	//	//					t2 = edgeVal;
	//	//				}
	//	//				else
	//	//				{
	//	//					j2++;
	//	//					float xi = edgeVal;
	//	//					t2 += xi;
	//	//					float diff = (j2 * xi) - t2;
	//	//					v2 += (diff * diff) / (j2 * (j2 - 1));
	//	//				}

	//	//				m2 += edgeVal;
	//	//			}
	//	//		}

	//	//		float vc1 = v1 / (j1 - 1);
	//	//		m1 /= 29;

	//	//		float vc2 = v2 / (j2 - 1);
	//	//		m2 /= 29;
	//	//				
	//	//		//s1 = (float)concurrency::sqrt(vc1);
	//	//		//s2 = (float)concurrency::sqrt(vc2);

	//	//		//float tstat = (m1 - m2) / (float)concurrency::sqrt((((float)concurrency::pow(s1, 2)) / 29) + (((float)concurrency::pow(s2, 2)) / 29));

	//	//		vc1EV[idx] = vc1;
	//	//		m1EV[idx] = m1;
	//	//		vc2EV[idx] = vc2;
	//	//		m2EV[idx] = m2;
	//	//	});

	//	//	vc1EV.synchronize();
	//	//	m1EV.synchronize();
	//	//	vc2EV.synchronize();
	//	//	m2EV.synchronize();

	//	//	for(int i=0; i<4005; i++)
	//	//	{
	//	//		float fS1 = sqrt(vc1E[i]);
	//	//		float fS2 = sqrt(vc2E[i]);

	//	//		tstatView(i) = (m1E[i] - m2E[i] / sqrt(pow(fS1, 2) / 29) + (pow(fS2, 2) / 29));
	//	//	}

	//	//	// Copy data from GPU to CPU
	//	//	tstatView.synchronize();
	//	//}
	//	//catch (std::exception& ex)
	//	//{
	//	//	cout << "Caught exception: " << ex.what() << endl;
	//	//}
	//}

	typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;
	typedef UDGraph::vertex_descriptor VDS;
	typedef UDGraph::edge_descriptor EDS;
	typedef boost::graph_traits<UDGraph>::edge_iterator edge_iter;

	GraphStatsNative::GraphStatsNative(void)
	{
	}

	GraphStatsNative::~GraphStatsNative(void)
	{
	}

	void GraphStatsNative::AddEdge(int i, int j)
	{
		_edges.push_back(Edge(i, j));
	}

	int GraphStatsNative::GetLargestComponentSize()
	{
		UDGraph graph(90);
		vector<Edge>::iterator ei;

		for(ei = _edges.begin(); ei < _edges.end(); ei++)
			add_edge(ei->first, ei->second, graph);
			
		map<int, int> componentCounts;
		map<int, int>::iterator mit;

		Components.resize(num_vertices(graph));
		
		int num = connected_components(graph, &Components[0]);

		for(ei = _edges.begin(); ei < _edges.end(); ei++)
		{
			int cmpId = Components[ei->first];

			if (componentCounts.find(cmpId) != componentCounts.end()) 
				componentCounts[cmpId]++;
			else
				componentCounts[cmpId] = 1;
		}

		int maxCount = 0;
		for(mit = componentCounts.begin(); mit != componentCounts.end(); mit++)
		{
			if(mit->second > maxCount)
				maxCount = mit->second;
		}

		return maxCount;
	}

	void GraphStatsNative::GetLargestComponent()
	{}
}
