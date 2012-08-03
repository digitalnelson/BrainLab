using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Events;
using BrainLabLibrary;
using Caliburn.Micro;

namespace BrainLab.Services
{
	public enum ComputeGroup
	{
		GroupOne,
		GroupTwo
	}

	public interface IComputeService
	{
		void SetDataTypes(Dictionary<string, double> dataTypes);
		Dictionary<string, double> GetDataTypes();

		void SetGroups(List<string> group1Idents, List<string> group2Idents);

		void FilterSubjects();
		Dictionary<ComputeGroup, List<Subject>> GetFilteredSubjectsByComputeGroup();
		Dictionary<ComputeGroup, int> GetFilteredSubjectCountsByComputeGroup();

		// Graph


		// NBS
		void LoadSubjects();
		void CompareGroups();
		void PermuteGroups(int permutations);
		void GetResults();
	}

	public class ComputeService : IComputeService
	{
		private readonly ISubjectService _subjectService;
		private readonly IRegionService _regionService;
		private readonly IEventAggregator _eventAggregator;

		private Dictionary<string, double> _dataTypes;
		private List<string> _group1Idents;
		private List<string> _group2Idents;

		private List<Subject> _filteredSubjectData;
		private Dictionary<ComputeGroup, List<Subject>> _filteredSubjectDataByGroup;

		private MultiModalCompare _compare;

		public ComputeService(ISubjectService subjectService, IRegionService regionService, IEventAggregator eventAggregator)
		{
			_subjectService = subjectService;
			_regionService = regionService;
			_eventAggregator = eventAggregator;
		}

		public void SetGroups(List<string> group1Idents, List<string> group2Idents)
		{
			_group1Idents = group1Idents;
			_group2Idents = group2Idents;
		}

		public void SetDataTypes(Dictionary<string, double> dataTypes)
		{
			_dataTypes = dataTypes;
		}

		public Dictionary<string, double> GetDataTypes()
		{
			return _dataTypes;
		}

		public void FilterSubjects()
		{
			// Loop through our subject data and get rid of the ones without complete data based on user selection
			_filteredSubjectData = new List<Subject>();
			_filteredSubjectDataByGroup = new Dictionary<ComputeGroup, List<Subject>>();

			_filteredSubjectDataByGroup[ComputeGroup.GroupOne] = new List<Subject>();
			_filteredSubjectDataByGroup[ComputeGroup.GroupTwo] = new List<Subject>();

			List<Subject> subjects = _subjectService.GetSubjects();

			foreach (var subject in subjects)
			{
				bool bHasData = true;

				foreach (var dt in _dataTypes)
				{
					if (!subject.Graphs.ContainsKey(dt.Key))
					{
						bHasData = false;
						break;
					}
				}

				if (bHasData)
				{
					_filteredSubjectData.Add(subject);

					var computeGrp = ComputeGroup.GroupOne;
					if (_group2Idents.Contains(subject.GroupId))
						computeGrp = ComputeGroup.GroupTwo;

					_filteredSubjectDataByGroup[computeGrp].Add(subject);
				}
			}

			_eventAggregator.Publish(new SubjectsFilteredEvent());
		}

		public Dictionary<ComputeGroup, List<Subject>> GetFilteredSubjectsByComputeGroup()
		{
			return _filteredSubjectDataByGroup;
		}

		public Dictionary<ComputeGroup, int> GetFilteredSubjectCountsByComputeGroup()
		{
			Dictionary<ComputeGroup, int> counts = new Dictionary<ComputeGroup, int>();
			foreach (var itm in _filteredSubjectDataByGroup)
			{
				counts[itm.Key] = itm.Value.Count;
			}

			return counts;
		}

		#region NBSm
		public void LoadSubjects()
		{
			_compare = new MultiModalCompare(_filteredSubjectData.Count, _regionService.GetNodeCount(), _regionService.GetEdgeCount(), _dataTypes.Keys.ToList());
			_compare.LoadSubjects(_filteredSubjectData);
		}

		public void CompareGroups()
		{
			if (_compare != null)
				_compare.CompareGroups(_group1Idents[0], _group2Idents[0], _dataTypes); // TODO: Make this use the groupings
		}

		public void PermuteGroups(int permutations)
		{
			if (_compare != null)
				_compare.Permute(permutations, _dataTypes);
		}

		public void GetResults()
		{
			var overlap = _compare.GetResult();
		}
		#endregion
	}
}
