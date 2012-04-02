#include "stdafx.h"
#include "SubjectGraphItem.h"
#include "Graph.h"
#include "EdgeValue.h"

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
		edgeVal.Value = val;

		RawGraph->AddEdge(i, j, edgeVal);
	}

	double  SubjectGraphItem::GetEdge(int i, int j)
	{
		EdgeValue edgeVal = RawGraph->GetEdge(i, j);
		return edgeVal.Value;
	}
}