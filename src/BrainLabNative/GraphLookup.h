#pragma once
#include <vector>

namespace BrainLabNative
{
	class GraphLookup
	{
	public:
		GraphLookup()
		{}

		GraphLookup(int m) : _idxToEdge( ( m * (m-1) ) / 2)
		{
			int idx=0;
			for(int i=0; i<m; ++i)
			{
				for(int j=0; j<m; ++j)
				{
					// Make sure this is only the upper triangle
					if(j > i)
					{
						_idxToEdge[idx] = std::pair<int, int>(i, j);
						++idx;
					}
				}
			}
		}

		std::pair<int, int> GetEdge(int idx)
		{
			return _idxToEdge[idx];
		}

	private:
		std::vector<std::pair<int, int>> _idxToEdge;
	};
}