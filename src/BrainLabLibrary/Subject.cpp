#include "stdafx.h"
#include "Subject.h"

namespace BrainLabLibrary
{
	Subject::Subject(void)
	{
		EventIds = gcnew List<String^>();
		Attributes = gcnew Dictionary<String^, String^>();
		Graphs = gcnew Dictionary<String^, SubjectGraphItem^>();
	}

	void Subject::AddAttribute(String^ name, String^ value)
	{
		Attributes[name] = value;
	}
}
