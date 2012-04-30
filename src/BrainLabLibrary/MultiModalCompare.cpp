#include "stdafx.h"
#include "MultiModalCompare.h"
#include "GraphComparisonMulti.h"
#include <vector>
#include <map>

namespace BrainLabLibrary
{
	MultiModalCompare::MultiModalCompare(int subjectCount, int vertices, int edges, List<String^> ^dataTypes)
	{
		std::vector<std::string> dtypes;

		for each(String^ dstr in dataTypes)
			dtypes.push_back(msclr::interop::marshal_as<std::string>(dstr));

		_cmpMulti = new	GraphComparisonMulti(subjectCount, vertices, edges, dtypes);
		_permutations = 0;
	}

	void MultiModalCompare::LoadSubjects(List<SubjectData^>^ itms)
	{
		// Loop through subject data items
		for(int idx=0; idx<itms->Count; ++idx)
		{
			// Pull this subject out of the list
			SubjectData ^subj = itms[idx];

			// Create an unmanaged subject
			Subject s;
			s.GroupId = msclr::interop::marshal_as<std::string>(subj->GroupId);

			// Loop through the graphs for this subject
			for each(KeyValuePair<String^, SubjectGraphItem^>^ itm in subj->Graphs)	
			{
				// Convert our CLR string to a STL string
				std::string dataKey = msclr::interop::marshal_as<std::string>(itm->Key);

				auto graph = itm->Value->RawGraph;

				// Add the graph to the unmanaged subject
				s.Graphs[dataKey] = graph;
			}

			// Add subject to our comparison multi
			_cmpMulti->AddSubject(&s);
		}
	}
		
	void MultiModalCompare::CompareGroups(String^ group1, String^ group2, Dictionary<String^, double>^ thresholds)
	{
		// Convert our parameters to native C++
		std::string strGroup1 = msclr::interop::marshal_as<std::string>(group1);
		std::string strGroup2 = msclr::interop::marshal_as<std::string>(group2);

		std::map<std::string, Threshold> threshes;
		for each(KeyValuePair<String^, double>^ itm in thresholds)	
		{
			Threshold thresh;
			thresh.DataType = msclr::interop::marshal_as<std::string>(itm->Key);
			thresh.Value = itm->Value;

			threshes[thresh.DataType] = thresh;
		}

		_cmpMulti->Compare(strGroup1, strGroup2, threshes);
	}

	void MultiModalCompare::Permute(int permutations, Dictionary<String^, double>^ thresholds)
	{
		std::map<std::string, Threshold> threshes;

		for each(KeyValuePair<String^, double>^ itm in thresholds)	
		{
			Threshold thresh;
			thresh.DataType = msclr::interop::marshal_as<std::string>(itm->Key);
			thresh.Value = itm->Value;

			threshes[thresh.DataType] = thresh;
		}

		_cmpMulti->Permute(permutations, threshes);
		_permutations += permutations;
	}

	BrainLabStorage::Overlap^ MultiModalCompare::GetResult()
	{
		// Pull the results of the comparison
		BrainLabLibrary::Overlap overlapResult = _cmpMulti->GetOverlapResult();
		
		// Convert the NBS computation result back to managed C++
		BrainLabStorage::Overlap^ blsor = gcnew BrainLabStorage::Overlap();
		blsor->Components = gcnew Dictionary<String^, List<GraphComponent^>^>();
		blsor->Permutations = _permutations;
		blsor->RightTailOverlapCount = overlapResult.RightTailOverlapCount;
		
		for(auto cit=overlapResult.Components.begin(); cit!=overlapResult.Components.end();++cit)
		{
			String^ dataType = msclr::interop::marshal_as<String^>(cit->first);
			List<GraphComponent^>^ components = gcnew List<GraphComponent^>();

			auto cmps = cit->second;
			for(auto vi=cmps.begin(); vi<cmps.end(); ++vi)
			{
				GraphComponent^ gc = gcnew GraphComponent();

				auto ces = vi->Edges;

				for(int i=0; i<ces.size(); ++i)
				{
					GraphEdge ge(ces[i].Edge.first, ces[i].Edge.second);
				
					ge.Value = ces[i].EdgeValue.Value;
					ge.M1 = ces[i].EdgeValue.M1;
					ge.M2 = ces[i].EdgeValue.M2;
					ge.Var1 = ces[i].EdgeValue.V1;
					ge.Var2 = ces[i].EdgeValue.V2;
					ge.TStat = ces[i].EdgeValue.TStat;
					ge.RightTailCount = ces[i].EdgeValue.RightTailCount;

					gc->Edges->Add(ge);
				}

				int edgeCount = gc->Edges->Count;
				//gc->VertexCount = vi->Vertices.size();
				gc->RightTailExtentCount = vi->RightTailExtent;

				components->Add(gc);
			}

			blsor->Components[dataType] = components;
		}

		blsor->Vertices = gcnew List<int>();
		for(auto vit=overlapResult.Vertices.begin();vit<overlapResult.Vertices.end();++vit)
			blsor->Vertices->Add(*vit);

		return blsor;
	}
}
