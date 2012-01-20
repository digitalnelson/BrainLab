using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabLibrary;

namespace BrainLab.Studio.Loaders
{
	class AdjCSVLoader
	{
		public AdjCSVLoader(string fullPath)
		{
			_fullPath = fullPath;
		}

		public List<SubjectData> Load(int vertexCount)
		{
			string[] adjFiles = Directory.GetFiles(_fullPath);
			
			Dictionary<string, SubjectData> subs = new Dictionary<string, SubjectData>();
			List<SubjectData> subList = new List<SubjectData>();

			foreach (var adjFile in adjFiles)
			//Parallel.ForEach(adjFiles, adjFile =>
			{
				var fileName = System.IO.Path.GetFileName(adjFile);
				var fileParts = fileName.Split('_');

				var subjId = fileParts[0];
				var adjType = fileParts[1];
				var desc = fileParts[2];

				SubjectData sd = null;
				if (!subs.ContainsKey(subjId))
				{
					sd = new SubjectData();
					subs[subjId] = sd;
					subList.Add(sd);
				}
				else
					sd = subs[subjId];

				sd.SubjectId = subjId;

				SubjectGraphItem itm = new SubjectGraphItem(vertexCount);
				itm.DataSource = adjType;

				// Read in all the lines
				string[] lines = System.IO.File.ReadAllLines(adjFile);

				for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
				{
					var line = lines[lineIdx];
					var columns = line.TrimEnd().Split(' ');

					for (int colIdx = 0; colIdx < columns.Length; colIdx++)
					{
						if (lineIdx < vertexCount && colIdx < vertexCount)
						{
							// TODO: Only load the upper triangle
							// Parse the value - set the identity to NaN
							//double dVal = colIdx == lineIdx ? Double.NaN : Double.Parse(columns[colIdx]);
							if (colIdx > lineIdx)
								itm.AddEdge(lineIdx, colIdx, Math.Abs(Double.Parse(columns[colIdx])));
						}
					}
				}

				sd.Graphs[adjType] = itm;
			}//);

			return subList;
		}

		private string _fullPath;
	}
}
