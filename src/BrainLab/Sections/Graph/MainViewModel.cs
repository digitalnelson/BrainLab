using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Events;
using BrainLab.Services;
using Caliburn.Micro;

namespace BrainLab.Sections.Graph
{
	public class MainViewModel : Screen, IHandle<SubjectsFilteredEvent>
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IComputeService _computeService;

		public MainViewModel(IEventAggregator eventAggregator, IComputeService computeService)
		{
			_eventAggregator = eventAggregator;
			_computeService = computeService;

			StrengthByDataType = new BindableCollection<StrengthViewModel>();

			this.DisplayName = "GRAPHS";
			_eventAggregator.Subscribe(this);
		}

		public BindableCollection<StrengthViewModel> StrengthByDataType { get; set; }

		public void LoadCharts()
		{
			StrengthByDataType.Clear();

			var dataTypes = _computeService.GetDataTypes();

			if (dataTypes != null)
			{
				foreach (var itm in dataTypes)
				{
					var svm = IoC.Get<StrengthViewModel>();
					svm.Load(itm.Key);

					StrengthByDataType.Add(svm);
				}
			}
		}

		public void Handle(SubjectsFilteredEvent message)
		{
			LoadCharts();
		}
	}
}
