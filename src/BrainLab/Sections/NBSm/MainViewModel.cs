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

			OverlapComponent = IoC.Get<OverlapViewModel>();
			ModalityComponents = new BindableCollection<GraphViewModel>();

			_eventAggregator.Subscribe(this);
		}

		public OverlapViewModel OverlapComponent { get; set; }
		public BindableCollection<GraphViewModel> ModalityComponents { get; set; }

		public void Handle(SubjectsFilteredEvent message)
		{
			ModalityComponents.Clear();

			var dataTypes = _computeService.GetDataTypes();

			if (dataTypes != null)
			{
				foreach (var itm in dataTypes)
				{
					var gvm = IoC.Get<GraphViewModel>();
					gvm.DataType = itm.Key;

					ModalityComponents.Add(gvm);
				}
			}
		}

		//private List<GraphView> _wrkSpaceComponents = new List<GraphView>();

		//private Color[] NBSmColors = new Color[] { 
		//    Color.FromRgb(0, 255, 0),
		//    Colors.Blue,
		//    Colors.Purple,
		//    Colors.Orange,
		//};

		private void Permute()
		{
			// Clear our workspace
			//oComponents.Clear();
			//_wrkSpace.Children.Clear();

			//await _viewModel.Permute();

			//Overlap overlap = _dataManager.GetOverlap();
			//if (overlap != null)
			//{
			//    int idx = 0;
			//    foreach (var dataType in _viewModel.DataTypes)
			//    {
			//        if (dataType.Selected)
			//        {
			//            var cmpView = new GraphView();
			//            cmpView.Width = 600;  // TODO: Make this dynamic and stretchy
			//            cmpView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
			//            _wrkSpace.Children.Add(cmpView);
			//            _wrkSpaceComponents.Add(cmpView);

			//            cmpView.SetDataManager(_dataManager);

			//            // TODO: Make this user settable and not blow up
			//            cmpView.LoadGraphComponents(overlap, dataType.Tag, NBSmColors[idx]);

			//            idx++;
			//        }
			//    }

			//    oComponents.LoadGraphComponents(overlap);
			//}

			//_btnSave.IsEnabled = true;
		}
	}
}
