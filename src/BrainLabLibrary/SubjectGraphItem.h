#pragma once
#include "Graph.h"
#include "GraphLookup.h"

using namespace System;
using namespace BrainLabNative;

namespace BrainLabLibrary
{
	public ref class SubjectGraphItem
	{
	public:
		property String^ DataSource;
		Graph* RawGraph;

		SubjectGraphItem(int m, GraphLookup* lu);
		~SubjectGraphItem();

		void AddEdge(int i, int j, double val);
		double GetEdge(int i, int j);

	private:
		GraphLookup* _lu;
	};
}