using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Events;
using BrainLab.Services;
using Caliburn.Micro;

namespace BrainLab
{
	public class LiveTilesViewModel : Screen, 
		IHandle<DataTypeSelection>, IHandle<GroupAssignment>, IHandle<RegionsLoadedEvent>, IHandle<SubjectsLoadedEvent>, IHandle<DataLoadedEvent>, IHandle<SubjectsFilteredEvent>
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IComputeService _computeService;
		private readonly IRegionService _regionService;
		private readonly ISubjectService _subjectService;

		public int DataTypeCount { get { return _inlDataTypeCount; } set { _inlDataTypeCount = value; NotifyOfPropertyChange(() => DataTypeCount); } } private int _inlDataTypeCount;
		public string Groups { get { return _inlGroups; } set { _inlGroups = value; NotifyOfPropertyChange(() => Groups); } } private string _inlGroups;
		
		public int Regions { get { return _inlRegions; } set { _inlRegions = value; NotifyOfPropertyChange(() => Regions); } } private int _inlRegions;
		public int Subjects { get { return _inlSubjects; } set { _inlSubjects = value; NotifyOfPropertyChange(() => Subjects); } } private int _inlSubjects;			
		public int Adjs { get { return _inlAdjs; } set { _inlAdjs = value; NotifyOfPropertyChange(() => Adjs); } } private int _inlAdjs;

		public int FilteredSubjects { get { return _inlFilteredSubjects; } set { _inlFilteredSubjects = value; NotifyOfPropertyChange(() => FilteredSubjects); } } private int _inlFilteredSubjects;

		public int Permutations { get { return _inlPermutations; } set { _inlPermutations = value; NotifyOfPropertyChange(() => Permutations); } } private int _inlPermutations;

		public LiveTilesViewModel(IEventAggregator eventAggregator, IComputeService computeService, IRegionService regionService, ISubjectService subjectService) 
		{
			_eventAggregator = eventAggregator;
			_computeService = computeService;
			_regionService = regionService;
			_subjectService = subjectService;

			Groups = "None";
			Permutations = 500;

			_eventAggregator.Subscribe(this);
		}

		public void Handle(RegionsLoadedEvent message)
		{
			Regions = _regionService.GetNodeCount();
		}

		public void Handle(SubjectsLoadedEvent message)
		{
			Subjects = _subjectService.GetSubjects().Count;
		}

		public void Handle(DataLoadedEvent message)
		{
			Adjs = _subjectService.GetFilesLoadedCount();
		}

		public void Handle(DataTypeSelection message)
		{
			// Added
			if (message.Selected)
				_selectedDataTypes[message.DataType] = message.Threshold;

			// Removed
			if (!message.Selected && _selectedDataTypes.ContainsKey(message.DataType))
				_selectedDataTypes.Remove(message.DataType);

			DataTypeCount = _selectedDataTypes.Keys.Count;

			_computeService.SetDataTypes(_selectedDataTypes);
		}
		private Dictionary<string, double> _selectedDataTypes = new Dictionary<string, double>();

		public void Handle(GroupAssignment message)
		{
			if (message.ComputeGroup == "Group 1")
			{
				_group2Members.Remove(message.GroupId);
				_group1Members.Add(message.GroupId);
			}

			if (message.ComputeGroup == "Group 2")
			{
				_group1Members.Remove(message.GroupId);
				_group2Members.Add(message.GroupId);
			}

			StringBuilder sbGroups = new StringBuilder();
			foreach (var grp in _group1Members)
				sbGroups.Append(grp + ",");
			foreach (var grp in _group2Members)
				sbGroups.Append(grp + ",");
			Groups = sbGroups.ToString();

			_computeService.SetGroups(_group1Members, _group2Members);
		}
		private List<string> _group1Members = new List<string>();
		private List<string> _group2Members = new List<string>();

		public void FilterSubjects()
		{
			_computeService.FilterSubjects();

			var counts = _computeService.GetFilteredSubjectCountsByComputeGroup();
		}

		public void Handle(SubjectsFilteredEvent message)
		{
			var subs = _computeService.GetFilteredSubjectCountsByComputeGroup();

			var count = 0;
			foreach (var itm in subs)
				count += itm.Value;

			FilteredSubjects = count;
		}

		public void Run()
		{
			_computeService.LoadSubjects();
			_computeService.CompareGroups();
			_computeService.PermuteGroups(Permutations);
			_computeService.GetResults();
		}
	}
}
