using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Events;
using BrainLab.Services;
using Caliburn.Micro;

namespace BrainLab.Sections.Filters
{
	public class MainViewModel : Screen, IHandle<SubjectsLoadedEvent>, IHandle<DataLoadedEvent>
	{
		readonly ISubjectService _subjectService;
		readonly IComputeService _computeService;
		readonly IEventAggregator _eventAggregator;

		public MainViewModel(ISubjectService subjectService, IComputeService computeService, IEventAggregator eventAggregator)
		{
			_subjectService = subjectService;
			_computeService = computeService;
			_eventAggregator = eventAggregator;

			this.DisplayName = "FILTERS";

			Groups = new BindableCollection<GroupViewModel>();
			DataTypes = new BindableCollection<DataTypeViewModel>();

			_eventAggregator.Subscribe(this);
		}

		public void Handle(SubjectsLoadedEvent message)
		{
			Groups.Clear();

			var groups = _subjectService.GetGroups();
			foreach (var group in groups)
			{
				var grp = IoC.Get<GroupViewModel>();
				grp.Title = group;

				Groups.Add(grp);
			}
		}

		public void Handle(DataLoadedEvent message)
		{
			DataTypes.Clear();

			var dataTypes = _subjectService.GetDataTypes();
			foreach (var dataType in dataTypes)
			{
				var dt = IoC.Get<DataTypeViewModel>();
				dt.Title = dataType;

				DataTypes.Add(dt);
			}
		}

		public void FilterSubjects()
		{
			_computeService.FilterSubjects();
		}

		public BindableCollection<GroupViewModel> Groups { get; private set; }
		public BindableCollection<DataTypeViewModel> DataTypes { get; private set; }
	}
}
