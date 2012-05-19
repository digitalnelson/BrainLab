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
using System.ComponentModel;

namespace BrainLab.Studio
{
	public class DataManager : INotifyPropertyChanged
	{
		public DataManager()
		{
			_regionsOfInterest = new List<ROI>();
			_regionsOfInterestByIndex = new Dictionary<int, ROI>();

			_subjectsByGroup = new Dictionary<string, List<Subject>>();
			_subjectsById = new Dictionary<string, Subject>();
			
			_graphsBySource = new Dictionary<string, List<SubjectGraphItem>>();
			//_analysisBySource = new Dictionary<string, GroupCompare>();

			_subjectData = new List<SubjectData>();

			FilteredSubjectData = new List<SubjectData>();
			FilteredSubjectDataByGroup = new Dictionary<string, List<SubjectData>>();
		}

		public int SubjectCount
		{
			get
			{
				return _subjectsById.Keys.Count;
			}
		}

		public int RegionCount
		{
			get
			{
				return _regionsOfInterest.Count;
			}
		}

		public int AdjCount
		{
			get
			{
				return _adjCount;
			}
			set
			{
				_adjCount = value;
				RaisePropertyChanged("AdjCount");
			}
		}
		private int _adjCount = 0;

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
			this.ZMin = _roiLoader.ZMin;
			this.ZMax = _roiLoader.ZMax;

			RaisePropertyChanged("RegionCount");
		}

		public void LoadSubjectFile(string fullPath)
		{
			_subjectLoader = new SubjectCSVLoader(fullPath);
			List<Subject> subjects = _subjectLoader.LoadSubjectFile();

			Groups = _subjectLoader.Groups.Keys.ToList();

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

			RaisePropertyChanged("SubjectCount");
		}

		protected void RaisePropertyChanged(string property)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(property));
			}
		}

		public void LoadAdjFiles(string fullPath)
		{
            _vertexCount = _regionsOfInterest.Count;
			_adjLoader = new AdjCSVLoader(fullPath, _vertexCount, _subjectsById);
			int count = 0;
			_subjectData = _adjLoader.Load(out count);

			AdjCount = count;

			DataTypes = _adjLoader.DataTypes.Keys.ToList();

			// TODO: Keep track of data types and data type counts
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

		public List<SubjectData> FilteredSubjectData { get; set; }
		public Dictionary<string, List<SubjectData>> FilteredSubjectDataByGroup { get; set; }

		public void LoadComparisons(Dictionary<string, double> thresholds)
		{
			List<string> selectedDataTypes = thresholds.Keys.ToList();

			// Loop through our subject data and get rid of the ones without complete data based on user selection
			FilteredSubjectData = new List<SubjectData>();
			FilteredSubjectDataByGroup = new Dictionary<string, List<SubjectData>>();
			foreach(var sd in _subjectData)
			{
				bool bHasData = true;

				foreach (var dt in selectedDataTypes)
				{
					if (!sd.Graphs.ContainsKey(dt))
					{
						bHasData = false;
						break;
					}
				}

				if (bHasData)
				{
					FilteredSubjectData.Add(sd);

					if (!FilteredSubjectDataByGroup.ContainsKey(sd.GroupId))
						FilteredSubjectDataByGroup[sd.GroupId] = new List<SubjectData>();

					FilteredSubjectDataByGroup[sd.GroupId].Add(sd);
				}
			}

			_edgeCount = (_vertexCount * (_vertexCount - 1)) / 2;
			_compare = new MultiModalCompare(FilteredSubjectData.Count, _vertexCount, _edgeCount, selectedDataTypes);
			_compare.LoadSubjects(FilteredSubjectData);
		}

		public Dictionary<string, SubjectData> SubjectsByGroup { get; set; }

		public void CalculateGroupDifferences(string group1Id, string group2Id, Dictionary<string, double> thresholds)
		{
			_compare.CompareGroups(group1Id, group2Id, thresholds);
		}

		public void PermuteComparisons(int permutations, Dictionary<string, double> thresholds)
		{
			_compare.Permute(permutations, thresholds);

			_overlap = _compare.GetResult();
			foreach (var v in _overlap.Vertices)
			{
				var r = _regionsOfInterestByIndex[v];
				r.Special = true;
			}
		}

		public ROI GetROI(int roiIdx)
		{
			return _regionsOfInterestByIndex[roiIdx];
		}

		public Overlap GetOverlap()
		{
			return _overlap;
		}

		public Dictionary<string, List<GraphComponent>> GetGraphComponents()
		{
			return _overlap.Components;
		}

		public class EdgeStats
		{
			public string Group;
			public string Measure;

			public double Corr;
			
			public double Both;
			public double Left;
			public double Right;
		}

		class CorrVals
		{
			public List<double> EdgeValues = new List<double>();
			public List<double> MeasureValues = new List<double>();
		}

		public List<EdgeStats> CorrelateEdgeAndMeasure(GraphEdge edge, string dataType, string measure)
		{
			List<double> edgeValues1 = new List<double>();
			List<double> edgeValues2 = new List<double>();
			List<double> measureValues1 = new List<double>();
			List<double> measureValues2 = new List<double>();

			Dictionary<string, CorrVals> vals = new Dictionary<string, CorrVals>();

			foreach(var sd in _subjectData)
			{
				Subject s = _subjectsById[sd.SubjectId];
				var val = s[measure];

				if (val == "None")
					continue;

				if (!vals.ContainsKey(s.Group))
					vals[s.Group] = new CorrVals();

				vals[s.Group].MeasureValues.Add(Double.Parse(s[measure]));
				vals[s.Group].EdgeValues.Add(sd.Graphs[dataType].GetEdge(edge.V1, edge.V2));
			}

			List<EdgeStats> stats = new List<EdgeStats>();
			foreach(var itm in vals)
			{
				var ev = itm.Value.EdgeValues;
				var mv = itm.Value.MeasureValues;

				if (ev.Count > 5 && mv.Count > 5)
				{
					EdgeStats es = new EdgeStats();
					es.Group = itm.Key;
					es.Measure = measure;
					es.Corr = alglib.spearmancorr2(ev.ToArray(), mv.ToArray());
					alglib.spearmanrankcorrelationsignificance(es.Corr, ev.Count, out es.Both, out es.Left, out es.Right);

					stats.Add(es);
				}
			}

			return stats;
		}

		public double XMin;
		public double XMax;
		public double YMin;
		public double YMax;
		public double ZMin;
		public double ZMax;

		public List<string> Groups { get; set; }
		public List<string> DataTypes { get; set; }

		private ROILoader _roiLoader;
		private SubjectCSVLoader _subjectLoader;
		private AdjCSVLoader _adjLoader;

		private int _vertexCount;
		private int _edgeCount;

		private List<ROI> _regionsOfInterest;
		private Dictionary<int, ROI> _regionsOfInterestByIndex;
		
		private Dictionary<string, List<Subject>> _subjectsByGroup;
		private Dictionary<string, Subject> _subjectsById;
		private List<SubjectData> _subjectData;
		
		private Dictionary<string, List<SubjectGraphItem>> _graphsBySource;
		//private Dictionary<string, GroupCompare> _analysisBySource;

		private MultiModalCompare _compare;
		private Overlap _overlap;

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
