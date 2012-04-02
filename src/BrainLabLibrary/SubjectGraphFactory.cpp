#include "stdafx.h"
#include "SubjectGraphFactory.h"

namespace BrainLabLibrary
{
	SubjectGraphFactory::SubjectGraphFactory(int vertexCount)
	{
		_vertexCount  = vertexCount;
		_lu = new GraphLookup(vertexCount);
	};

	SubjectGraphItem^ SubjectGraphFactory::CreateSubject()
	{
		return gcnew SubjectGraphItem(_vertexCount, _lu);
	}
}
