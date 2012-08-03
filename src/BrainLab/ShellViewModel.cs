using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrainLab.Services;
using Caliburn.Micro;
using Ninject;

namespace BrainLab
{
	public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
	{
		[Inject]
		public ShellViewModel()
		{
			this.DisplayName = "BRAINLAB";

			LiveTiles = IoC.Get<LiveTilesViewModel>();

			Items.Add(IoC.Get<BrainLab.Sections.Inputs.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.Filters.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.Graph.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.NBSm.MainViewModel>());
		}

		public LiveTilesViewModel LiveTiles { get; set; }

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
