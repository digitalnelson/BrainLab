#pragma once
#include <msclr\marshal.h>
#include <msclr\marshal_cppstd.h>
#include <map>
#include "SubjectData.h"
#include "SubjectGraphItem.h"
#include "GraphComparisonMulti.h"

using namespace System::Collections::Generic;
using namespace BrainLabStorage;

namespace BrainLabLibrary
{
	public ref class MultiModalCompare
	{
	public:
		MultiModalCompare(int subjectCount, int vertices, int edges, List<String^> ^dataTypes);

		void LoadSubjects(List<SubjectData^>^ itms);
		void Permute(int permutations, Dictionary<String^, double>^ thresholds);
		void CompareGroups(String^ group1, String^ group2, Dictionary<String^, double>^ thresholds);
		
		BrainLabStorage::Overlap^ GetResult();

	private:
		GraphComparisonMulti *_cmpMulti;
		int _permutations;
	};
}

