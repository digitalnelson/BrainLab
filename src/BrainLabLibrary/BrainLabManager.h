#pragma once
#include <vector>

using namespace System;
using namespace System::Collections::Generic;
using namespace BrainLabStorage;

namespace BrainLabLibrary
{
	public ref class BrainLabManager
	{
	private:
		//GraphStatsNative* _graphStatsNative;

	public:
		BrainLabManager();
		~BrainLabManager();

		void Permute(array<double, 2>^ data);
		void AddEdge(int i, int j);
		int GetLargestComponentSize ();
		List<int>^ GetComponentList();
		void Cleanup();		
	};
}