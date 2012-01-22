using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabStorage;
using BrainLabLibrary;
using System.Diagnostics;
using BrainLab.Studio.Loaders;

namespace BrainLab.Studio
{
	public class DataManager
	{
		public DataManager()
		{
			_regionsOfInterest = new List<ROI>();
			_regionsOfInterestByIndex = new Dictionary<int, ROI>();

			_subjectsByGroup = new Dictionary<string, List<Subject>>();
			_subjectsById = new Dictionary<string, Subject>();
			
			_graphsBySource = new Dictionary<string, List<SubjectGraphItem>>();
			_analysisBySource = new Dictionary<string, GroupCompare>();

			_subjectData = new List<SubjectData>();
		}

		#region Data Loading and Cache

		public void LoadROIFile(string fullPath)
		{
			_roiLoader = new ROILoader(fullPath);
			var rois = _roiLoader.Load();

			foreach (var roi in rois)
			{
				_regionsOfInterest.Add(roi);
				_regionsOfInterestByIndex[roi.Index] = roi;
			}

			this.XMin = _roiLoader.XMin;
			this.XMax = _roiLoader.XMax;
			this.YMin = _roiLoader.YMin;
			this.YMax = _roiLoader.YMax;
		}

		public void LoadSubjectFile(string fullPath)
		{
			_subjectLoader = new SubjectCSVLoader(fullPath);
			List<Subject> subjects = _subjectLoader.LoadSubjectFile();

			foreach (var sub in subjects)
			{
				List<Subject> subs = null;
				if (!_subjectsByGroup.ContainsKey(sub.Group))
				{
					subs = new List<Subject>();
					subs.Add(sub);
					_subjectsByGroup[sub.Group] = subs;
				}
				else
					_subjectsByGroup[sub.Group].Add(sub);

				_subjectsById[sub.Id] = sub;
			}
		}

		public void LoadAdjFiles(string fullPath, int vertexCount)
		{
			_adjLoader = new AdjCSVLoader(fullPath);
			_subjectData = _adjLoader.Load(vertexCount);

			foreach (var itm in _subjectData)
			{
				Subject subj = null;
				if (!_subjectsById.TryGetValue(itm.SubjectId, out subj))
					continue;
				else
					itm.GroupId = subj.Group;
			}
		}

		#endregion

		public void LoadComparisons()
		{
			_compare = new MultiModalCompare(58, 90, 4005, new List<string>() { "DTI", "fMRI" });
			_compare.LoadSubjects(_subjectData);
		}

		public ROI GetROI(int roiIdx)
		{
			return _regionsOfInterestByIndex[roiIdx];
		}

		public void CalculateGroupDifferences(string group1Id, string group2Id, Dictionary<string, double> thresholds)
		{
			_compare.CompareGroups(group1Id, group2Id, thresholds);
		}

		public void PermuteComparisons(int permutations, int group1Size, Dictionary<string, double> thresholds)
		{
			_compare.Permute(permutations, group1Size, thresholds);

			_overlap = _compare.GetResult();
			foreach (var v in _overlap.Vertices)
			{
				var r = _regionsOfInterestByIndex[v];
				r.Special = true;
			}
		}

		public Overlap GetOverlap()
		{
			return _overlap;
		}

		public Dictionary<string, List<GraphComponent>> GetGraphComponents()
		{
			return _overlap.Components;
		}

		public double XMin;
		public double XMax;
		public double YMin;
		public double YMax;

		private ROILoader _roiLoader;
		private SubjectCSVLoader _subjectLoader;
		private AdjCSVLoader _adjLoader;

		private List<ROI> _regionsOfInterest;
		private Dictionary<string, List<Subject>> _subjectsByGroup;
		private Dictionary<string, Subject> _subjectsById;
		private List<SubjectData> _subjectData;
		private Dictionary<int, ROI> _regionsOfInterestByIndex;

		private Dictionary<string, List<SubjectGraphItem>> _graphsBySource;
		private Dictionary<string, GroupCompare> _analysisBySource;

		private MultiModalCompare _compare;
		private Overlap _overlap;
	}
}
