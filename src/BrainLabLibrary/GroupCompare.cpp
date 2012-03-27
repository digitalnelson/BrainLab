#include "stdafx.h"
#include "GroupCompare.h"
//#include "Graph.h"
//#include <vector>
//
//namespace BrainLabLibrary
//{
//	GroupCompare::GroupCompare(List<SubjectGraphItem^>^ itms, int vertices)
//	{
//		_groupLists = gcnew Dictionary<String^, List<int>^>();
//
//		int upperTriangleEdgeCount = ( vertices * (vertices - 1) ) / 2;
//
//		//_graphComparison = new GraphComparison(itms->Count, vertices, upperTriangleEdgeCount);
//
//		this->AddSubjectGraphItems(itms);
//	}
//
//	void GroupCompare::AddSubjectGraphItems(List<SubjectGraphItem^>^ itms)
//	{
//		//for(int idx=0; idx<itms->Count; idx++)
//		//{
//		//	// Get our subject
//		//	SubjectGraphItem^ itm = itms[idx];
//
//		//	// Add the raw subject graph to the graph comparison system
//		//	_graphComparison->AddGraph(*itm->RawGraph);
//
//		//	// Make sure this group exists in the cache
//		//	if(!_groupLists->ContainsKey(itm->GroupId))
//		//		_groupLists[itm->GroupId] = gcnew List<int>();
//
//		//	// Keep track of our groups by the order in which they were added
//		//	// To the graph comparison
//		//	_groupLists[itm->GroupId]->Add(idx);
//		//}
//	}
//
//	void GroupCompare::Permute(int permutations, int group1Size, double tStatThreshold)
//	{
//		//_graphComparison->GenDistros(permutations, group1Size, tStatThreshold);
//	}
//
//	void GroupCompare::CompareGroups(String^ group1, String^ group2, double tStatThreshold)
//	{
//		/*List<int>^ idxs = gcnew List<int>();
//
//		idxs->AddRange(_groupLists[group1]);
//		idxs->AddRange(_groupLists[group2]);
//
//		std::vector<int> nativeIdxs;
//		for each(int itm in _groupLists[group1])
//			nativeIdxs.push_back(itm);
//		for each(int itm in _groupLists[group2])
//			nativeIdxs.push_back(itm);
//		
//		BrainLabNative::Graph g(90);
//		_graphComparison->CompareGroups(nativeIdxs, _groupLists[group1]->Count, g, tStatThreshold);
//		g.ComputeComponents();
//		int max = g.GetLargestComponentSize();
//		
//		std::vector<BrainLabNative::Graph::Component> cmps;
//		g.GetComponents(cmps);
//		
//		_components = gcnew List<GraphComponent^>();
//		for(auto vi=cmps.begin(); vi<cmps.end(); ++vi)
//		{
//			GraphComponent^ gc = gcnew GraphComponent();
//
//			auto ces = vi->Edges;
//
//			for(int i=0; i<ces.size(); ++i)
//			{
//				GraphEdge ge(ces[i].Edge.first, ces[i].Edge.second);
//				
//				ge.Value = ces[i].EdgeValue.Value;
//				ge.M1 = ces[i].EdgeValue.M1;
//				ge.M2 = ces[i].EdgeValue.M2;
//				ge.Var1 = ces[i].EdgeValue.V1;
//				ge.Var2 = ces[i].EdgeValue.V2;
//				ge.TStat = ces[i].EdgeValue.TStat;
//				ge.PValue = ces[i].EdgeValue.PValue;
//
//				gc->Edges->Add(ge);
//			}
//
//			int edgeCount = gc->Edges->Count;
//
//			gc->PValue = _graphComparison->GetComponentSizePVal(edgeCount);
//
//			_components->Add(gc);
//		}*/
//	}
//
//	List<GraphComponent^>^ GroupCompare::GetGraphComponents()
//	{
//		return _components;
//	}
//}
