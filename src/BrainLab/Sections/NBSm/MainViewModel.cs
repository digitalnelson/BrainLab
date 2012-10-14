using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Ninject;
using BrainLab.Events;
using BrainLab.Services;

namespace BrainLab.Sections.NBSm
{
	public class MainViewModel : Screen, IHandle<SubjectsFilteredEvent>
	{
		#region Private Service Vars
		private readonly IEventAggregator _eventAggregator = IoC.Get<IEventAggregator>();
		private readonly IComputeService _computeService = IoC.Get<IComputeService>();
		#endregion

		[Inject]
		public MainViewModel()
		{
			this.DisplayName = "NBSm";

			ModalityComponents = new BindableCollection<GraphViewModel>();

			_eventAggregator.Subscribe(this);
		}

		public BindableCollection<GraphViewModel> ModalityComponents { get; set; }

		public void Handle(SubjectsFilteredEvent message)
		{
			ModalityComponents.Clear();

			var overlapComponent = IoC.Get<GraphViewModel>();
			overlapComponent.DataType = "overlap";

			ModalityComponents.Add(overlapComponent);

			var dataTypes = _computeService.GetDataTypes();
			if (dataTypes != null)
			{
				foreach (var itm in dataTypes)
				{
					var gvm = IoC.Get<GraphViewModel>();
					gvm.DataType = itm.Key;
					gvm.ShowEdges = true;

					ModalityComponents.Add(gvm);
				}
			}
		}
	}
}
