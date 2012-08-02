using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabLibrary;

namespace BrainLab.Services
{
	public interface IComputeService
	{
		void SetDataTypes(Dictionary<string, double> dataTypes);
		void SetGroups(List<string> group1Idents, List<string> group2Idents);

		void FilterSubjects();
		Dictionary<string, int> GetFilteredSubjectCounts();

		void LoadSubjects();
		void CompareGroups();
		void PermuteGroups(int permutations);
	}

	public class ComputeService : IComputeService
	{
		private readonly ISubjectService _subjectService;
		private readonly IRegionService _regionService;

		private Dictionary<string, double> _dataTypes;
		private List<string> _group1Idents;
		private List<string> _group2Idents;

		private List<Subject> _filteredSubjectData;
		private Dictionary<string, List<Subject>> _filteredSubjectDataByGroup;

		private MultiModalCompare _compare;

		public ComputeService(ISubjectService subjectService, IRegionService regionService)
		{
			_subjectService = subjectService;
			_regionService = regionService;
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

		public void FilterSubjects()
		{
			// Loop through our subject data and get rid of the ones without complete data based on user selection
			_filteredSubjectData = new List<Subject>();
			_filteredSubjectDataByGroup = new Dictionary<string, List<Subject>>();

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

					if (!_filteredSubjectDataByGroup.ContainsKey(subject.GroupId))
						_filteredSubjectDataByGroup[subject.GroupId] = new List<Subject>();

					_filteredSubjectDataByGroup[subject.GroupId].Add(subject);
				}
			}
		}

		public Dictionary<string, int> GetFilteredSubjectCounts()
		{
			Dictionary<string, int> counts = new Dictionary<string, int>();
			foreach (var itm in _filteredSubjectDataByGroup)
			{
				counts[itm.Key] = itm.Value.Count;
			}

			return counts;
		}

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
	}
}
