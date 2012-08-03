using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Events;
using Caliburn.Micro;
using Ninject;

namespace BrainLab.Sections.Filters
{
	public class GroupViewModel : Screen
	{
		readonly IEventAggregator _eventAggregator;

		[Inject]
		public GroupViewModel(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
		}

		public string Title { get { return _title; } set { _title = value; NotifyOfPropertyChange(() => Title); } } private string _title;

		public string[] ExperimentGroups { get { return new string[] { "Group 1", "Group 2" }; } }
		
		public string SelectedExperimentGroup 
		{ 
			get { return _inlSelectedExperimentGroup; } 
			set 
			{ 
				_inlSelectedExperimentGroup = value; 
				NotifyOfPropertyChange(() => SelectedExperimentGroup);
				_eventAggregator.Publish(new GroupAssignment { GroupId = Title, ComputeGroup = _inlSelectedExperimentGroup }); 
			} 
		} 
		private string _inlSelectedExperimentGroup;
	}
}
