#include "stdafx.h"
#include "SubjectGraphItem.h"
#include "Graph.h"
#include "EdgeValue.h"

using namespace System;

namespace BrainLabLibrary
{
	SubjectGraphItem::SubjectGraphItem(int m, GraphLookup *lu)
	{
		RawGraph = new Graph(m, lu);
		_lu = lu;
	}

	SubjectGraphItem::~SubjectGraphItem(void)
	{
		if(RawGraph)
		{
			delete RawGraph;
			RawGraph = nullptr;
		}
	}

	void SubjectGraphItem::AddEdge(int i, int j, double val)
	{
		EdgeValue edgeVal;
		memset(&edgeVal, 0, sizeof(EdgeValue));

		edgeVal.Value = val;

		RawGraph->AddEdge(i, j, edgeVal);
	}

	double  SubjectGraphItem::GetEdge(int i, int j)
	{
		EdgeValue edgeVal = RawGraph->GetEdge(i, j);
		return edgeVal.Value;
	}

	float SubjectGraphItem::GlobalStrength()
	{
		return RawGraph->GlobalStrength();
	}

	System::Collections::Generic::List<float>^ SubjectGraphItem::MeanVtxStrength()
	{
		// get unmanaged values
		std::vector<float> values; 
		RawGraph->GetMeanVtxStrength(values);

		cli::array<float>^ managedValues = gcnew cli::array<float>(values.size());

		// cast to managed object type IntPtr representing an object pointer.
		System::IntPtr ptr = (System::IntPtr)&values[0];

		// copy data to managed array using System::Runtime::Interopservices namespace
		System::Runtime::InteropServices::Marshal::Copy(ptr, managedValues, 0, values.size());

		return gcnew System::Collections::Generic::List<float>(managedValues);
	}
}