#pragma once
#include "GraphLookup.h"
#include "SubjectGraphItem.h"

namespace BrainLabLibrary
{
	public ref class SubjectGraphFactory
	{
	public:
		SubjectGraphFactory(int vertexCount);
		SubjectGraphItem^ CreateSubject();

	private:
		int _vertexCount;
		BrainLabNative::GraphLookup *_lu;
	};
}

