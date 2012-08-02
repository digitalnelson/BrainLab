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
	public class LiveTilesViewModel : Screen, IHandle<DataTypeSelection>, IHandle<GroupAssignment>
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IComputeService _computeService;

		public int DataTypeCount { get { return _inlDataTypeCount; } set { _inlDataTypeCount = value; NotifyOfPropertyChange(() => DataTypeCount); } } private int _inlDataTypeCount;
		public string Groups { get { return _inlGroups; } set { _inlGroups = value; NotifyOfPropertyChange(() => Groups); } } private string _inlGroups;

		public LiveTilesViewModel(IEventAggregator eventAggregator, IComputeService computeService)
		{
			_eventAggregator = eventAggregator;
			_computeService = computeService;

			_eventAggregator.Subscribe(this);
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
	}
}
