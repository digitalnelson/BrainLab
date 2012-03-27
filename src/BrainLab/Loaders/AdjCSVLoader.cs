using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabLibrary;
using BrainLabStorage;

namespace BrainLab.Studio.Loaders
{
	class AdjCSVLoader
	{
		public AdjCSVLoader(string fullPath, int vertexCount, Dictionary<string, Subject> subjects)
		{
			_fullPath = fullPath;
			_vertexCount = vertexCount;
			_subjects = subjects;
			_sgf = new SubjectGraphFactory(_vertexCount);
		}

		public List<SubjectData> Load()
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

				if (!DataTypes.ContainsKey(adjType))
					DataTypes[adjType] = adjType;

				if (!_subjects.ContainsKey(subjId))
					continue;

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

				SubjectGraphItem itm = _sgf.CreateSubject();
				itm.DataSource = adjType;

				// Read in all the lines
				string[] lines = System.IO.File.ReadAllLines(adjFile);

				for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
				{
					var line = lines[lineIdx];
					var columns = line.TrimEnd().Split('\t');

					for (int colIdx = 0; colIdx < columns.Length; colIdx++)
					{
						if (lineIdx < _vertexCount && colIdx < _vertexCount)
						{
							// Only load the upper triangle
							if (colIdx > lineIdx)
								itm.AddEdge(lineIdx, colIdx, Math.Abs(Double.Parse(columns[colIdx])));
						}
					}
				}

				sd.Graphs[adjType] = itm;
			}//);

			return subList;
		}

		public Dictionary<string, string> DataTypes = new Dictionary<string, string>();

		private string _fullPath;
		private int _vertexCount;
		Dictionary<string, Subject> _subjects;
		private SubjectGraphFactory _sgf;
	}
}
