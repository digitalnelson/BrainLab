using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrainLab.Events;
using Caliburn.Micro;
using Ninject;

namespace BrainLab.Sections.Filters
{
	public class DataTypeViewModel : Screen
	{
		readonly IEventAggregator _eventAggregator;

		[Inject]
		public DataTypeViewModel(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Threshold { get { return _inlThreshold; } set { _inlThreshold = value; NotifyOfPropertyChange(() => Threshold); _eventAggregator.Publish(new DataTypeSelection { DataType = Title, Threshold = Double.Parse(Threshold), Selected = _inlSelected }); } } private string _inlThreshold;

		public bool Selected 
		{ 
			get { return _inlSelected; } 
			set 
			{ 
				_inlSelected = value; 
				NotifyOfPropertyChange(() => Selected);

				var dt = new DataTypeSelection
				{
					DataType = Title,
					Selected = _inlSelected
				};

				if (!string.IsNullOrEmpty(Threshold))
					dt.Threshold = Double.Parse(Threshold);
				
				_eventAggregator.Publish(dt); 
			} 
		} 
		private bool _inlSelected;
	}
}
