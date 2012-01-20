#include "stdafx.h"
#include "BrainLabManager.h"

namespace BrainLabLibrary
{
	BrainLabManager::BrainLabManager()
	{
		//_graphStatsNative = new GraphStatsNative();
	}

	BrainLabManager::~BrainLabManager()
	{
		//if(_graphStatsNative)
		//	delete _graphStatsNative;
	}

	void BrainLabManager::Permute(array<double, 2>^ data)
	{}

	void BrainLabManager::AddEdge(int i, int j)
	{
		//_graphStatsNative->AddEdge(i, j);
	}

	int BrainLabManager::GetLargestComponentSize ()
	{
		return 0;
		/*return _graphStatsNative->GetLargestComponentSize();*/
	}

	System::Collections::Generic::List<int>^ BrainLabManager::GetComponentList()
	{
		System::Collections::Generic::List<int>^ components = gcnew System::Collections::Generic::List<int>();

		//vector<int>::iterator ci;
		//for(ci = _graphStatsNative->Components.begin(); ci < _graphStatsNative->Components.end(); ci++)
		//{
		//	components->Add(*ci);
		//}

		return components;
	}

	void BrainLabManager::Cleanup()
	{
		//if(_graphStatsNative)
		//	delete _graphStatsNative;
	}
}