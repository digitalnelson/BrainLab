#include "stdafx.h"
#include "SubjectGraphItem.h"
#include "EdgeValue.h"

namespace BrainLabLibrary
{
	SubjectGraphItem::SubjectGraphItem(int m)
	{
		RawGraph = new Graph(m);
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
		edgeVal.Value = val;

		RawGraph->AddEdge(i, j, edgeVal);
	}
}