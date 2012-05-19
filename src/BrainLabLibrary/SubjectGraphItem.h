#pragma once
#include "GraphLookup.h"

using namespace System;

namespace BrainLabLibrary
{
	class Graph;

	public ref class SubjectGraphItem
	{
	public:
		property String^ DataSource;
		Graph* RawGraph;

		SubjectGraphItem(int m, GraphLookup* lu);
		~SubjectGraphItem();

		void AddEdge(int i, int j, double val);
		double GetEdge(int i, int j);

		float GlobalStrength();
		System::Collections::Generic::List<float>^ MeanVtxStrength();

	private:
		GraphLookup* _lu;
	};
}