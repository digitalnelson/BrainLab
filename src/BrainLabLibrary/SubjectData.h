#pragma once
#include "SubjectGraphItem.h"

using namespace System;
using namespace System::Collections::Generic;

namespace BrainLabLibrary
{
	public ref class SubjectData
	{
	public:
		SubjectData(void);

		property String^ SubjectId;
		property String^ Descriptor;
		property String^ GroupId;

		Dictionary<String^, SubjectGraphItem^>^ Graphs;
	};

}

