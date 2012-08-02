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

			Items.Add(IoC.Get<BrainLab.Sections.Setup.ExperimentViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.Groups.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.Simple.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.NBSm.MainViewModel>());
		}

		public LiveTilesViewModel LiveTiles { get; set; }

        private void Load()
        {
            //string regionFile = _txtRegionFile.Text;
            //string subjectFile = _txtSubjectFile.Text;
            //string dataFolder = _txtDataFolder.Text;
            //string outputDir = _txtOutputFolder.Text;

            //if (string.IsNullOrEmpty(regionFile))
            //{
            //    MessageBox.Show("Please set region file location.");
            //    return;
            //}
            //if (string.IsNullOrEmpty(subjectFile))
            //{
            //    MessageBox.Show("Please set subject file location.");
            //    return;
            //}
            //if (string.IsNullOrEmpty(dataFolder))
            //{
            //    MessageBox.Show("Please set data folder location.");
            //    return;
            //}
            //if (string.IsNullOrEmpty(outputDir))
            //{
            //    MessageBox.Show("Please set output folder location.");
            //    return;
            //}

            //_btnData.IsEnabled = false;
            //await _viewModel.Load(regionFile, subjectFile, dataFolder, outputDir);
            //_btnPermute.IsEnabled = true;
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

        protected void OnClosing()
        {
            //base.OnClosing(e);

            //DataStore.AppPrefs.RoiFilePath = _txtRegionFile.Text;
            //DataStore.AppPrefs.SubjectfilePath = _txtSubjectFile.Text;
            //DataStore.AppPrefs.DataFileDir = _txtDataFolder.Text;
            //DataStore.AppPrefs.OutputDir = _txtOutputFolder.Text;

            //DataStore.AppPrefs.WindowLocation.X = this.Left;
            //DataStore.AppPrefs.WindowLocation.Y = this.Top;
            //DataStore.AppPrefs.WindowLocation.Width = this.Width;
            //DataStore.AppPrefs.WindowLocation.Height = this.Height;

            //DataStore.Save();
            //DataStore.Close();
        }

        private void Button_Click_Region()
        {
            // Configure open file dialog box
            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = "AAL"; // Default file name
            //dlg.DefaultExt = ".txt"; // Default file extension
            //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            //if (_txtRegionFile.Text != "")
            //    dlg.InitialDirectory = System.IO.Path.GetFullPath(_txtRegionFile.Text);

            //// Show open file dialog box
            //Nullable<bool> result = dlg.ShowDialog();

            //// Process open file dialog box results
            //if (result == true)
            //{
            //    // Open document
            //    _txtRegionFile.Text = dlg.FileName;
            //}
        }

        private void Button_Click_Output()
        {
            //// Configure open file dialog box
            //var dlg = new System.Windows.Forms.FolderBrowserDialog();

            //if (_txtOutputFolder.Text != "")
            //    dlg.SelectedPath = System.IO.Path.GetFullPath(_txtOutputFolder.Text);

            //// Show open file dialog box
            //var result = dlg.ShowDialog();

            //// Process open file dialog box results
            //if (result == System.Windows.Forms.DialogResult.OK)
            //{
            //    // Open document
            //    _txtOutputFolder.Text = dlg.SelectedPath;
            //}
        }

        private void Button_Click_SaveReport()
        {
            //_viewModel.Save(oComponents, _wrkSpaceComponents);
        }
	}
}
