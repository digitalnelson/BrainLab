#pragma once
#include <vector>
#include <map>

namespace BrainLabLibrary
{
	class GraphEdgeMapper
	{
	};

	class GraphLookup
	{
	public:
		GraphLookup()
		{}

		GraphLookup(int m) : _idxToEdge( ( m * (m-1) ) / 2)
		{
			_size = m;

			int idx=0;
			for(int i=0; i<m; ++i)
			{
				for(int j=0; j<m; ++j)
				{
					// Make sure this is only the upper triangle
					if(j > i)
					{
						std::pair<int, int> edge = std::pair<int, int>(i, j);

						_idxToEdge[idx] = edge;
						_edgeToIdx[edge] = idx;
						++idx;
					}
				}
			}
		}

		std::pair<int, int> GetEdge(int idx)
		{
			return _idxToEdge[idx];
		}

		int GetEdge(int i, int j)
		{
			if(i < j)
				return _edgeToIdx[std::pair<int, int>(i, j)];
			else
				return _edgeToIdx[std::pair<int, int>(j, i)];
		}

	private:
		std::vector<std::pair<int, int>> _idxToEdge;
		std::map<std::pair<int, int>, int> _edgeToIdx;
		int _size;
	};
}