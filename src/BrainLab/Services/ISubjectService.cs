using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Services.Loaders;
using BrainLabLibrary;
using BrainLabStorage;

namespace BrainLab.Services
{
	public interface ISubjectService
	{
		void LoadSubjectFile(string fullPath);
		void LoadSubjectData(string fullPath, int limit);

		List<Subject> GetSubjects();

		List<string> GetGroups();
		List<string> GetDataTypes();
	}

	public class SubjectService : ISubjectService
	{
		private List<Subject> _subjects;
		private Dictionary<string, List<Subject>> _subjectsByGroup;
		private Dictionary<string, Subject> _subjectsByEventId;

		private List<string> _dataTypes;
		
		public SubjectService()
		{
			_subjects = new List<Subject>();
			_subjectsByGroup = new Dictionary<string, List<Subject>>();
			_subjectsByEventId = new Dictionary<string, Subject>();
			_dataTypes = new List<string>();
		}

		public void LoadSubjectFile(string fullPath)
		{
			var subjectLoader = new SubjectCSVLoader(fullPath);
			_subjects = subjectLoader.LoadSubjectFile();

			foreach (var sub in _subjects)
			{
				if (!_subjectsByGroup.ContainsKey(sub.GroupId))
					_subjectsByGroup[sub.GroupId] = new List<Subject>();
					
				_subjectsByGroup[sub.GroupId].Add(sub);

				foreach(var eventId in sub.EventIds)
					_subjectsByEventId[eventId] = sub;
			}
		}

		public void LoadSubjectData(string fullPath, int limit)
		{
			var adjLoader = new AdjCSVLoader(fullPath, limit);  // TODO: Fix, feels yucky. :-P
			adjLoader.Load(_subjectsByEventId);

			_dataTypes.Clear();
			foreach (var subject in _subjects)
			{
				foreach (var graph in subject.Graphs)
				{
					if (!_dataTypes.Contains(graph.Value.DataSource))
						_dataTypes.Add(graph.Value.DataSource);
				}
			}
		}

		public List<Subject> GetSubjects()
		{
			return _subjects;
		}

		public List<string> GetGroups()
		{
			return _subjectsByGroup.Keys.ToList();
		}

		public List<string> GetDataTypes()
		{
			return _dataTypes;
		}
	}
}
