#pragma once
#include "Graph.h"

using namespace System;
using namespace BrainLabNative;

namespace BrainLabLibrary
{
	public ref class SubjectGraphItem
	{
	public:
		property String^ DataSource;
		Graph* RawGraph;

		SubjectGraphItem(int m);
		~SubjectGraphItem();

		void AddEdge(int i, int j, double val);
		double GetEdge(int i, int j);
	};
}