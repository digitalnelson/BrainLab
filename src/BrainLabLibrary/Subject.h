#pragma once
#include "SubjectGraphItem.h"

using namespace System;
using namespace System::Collections::Generic;

namespace BrainLabLibrary
{
	public ref class Subject
	{
	public:
		Subject(void);
		void AddAttribute(String^ name, String^ value);

		property String^ SubjectId;
		property String^ GroupId;
		property String^ Age;
		property String^ Sex;

		property List<String^>^ EventIds;
		property Dictionary<String^, String^>^ Attributes;
		property Dictionary<String^, SubjectGraphItem^>^ Graphs;
	};
}

