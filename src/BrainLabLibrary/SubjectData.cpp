#include "stdafx.h"
#include "SubjectData.h"

namespace BrainLabLibrary
{
	SubjectData::SubjectData(void)
	{
		Graphs = gcnew Dictionary<String^, SubjectGraphItem^>();
	}
}
