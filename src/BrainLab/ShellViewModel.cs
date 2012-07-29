using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Ninject;

namespace BrainLab
{
	public class ShellViewModel : Screen, IShell
	{
		[Inject]
		public ShellViewModel()
		{
			this.DisplayName = "BRAINLAB";

            //// Set up user prefs for file locations 
            var file = DataStore.AppPrefs.RoiFilePath;
            //_txtSubjectFile.Text = DataStore.AppPrefs.SubjectfilePath;
            //_txtDataFolder.Text = DataStore.AppPrefs.DataFileDir;
            //_txtOutputFolder.Text = DataStore.AppPrefs.OutputDir;

            //// Set up last window location and size
            //if (DataStore.AppPrefs.WindowLocation != null &&  DataStore.AppPrefs.WindowLocation.Width != 0.0d && DataStore.AppPrefs.WindowLocation.Height != 0.0d)
            //{
            //    this.Left = DataStore.AppPrefs.WindowLocation.X;
            //    this.Top = DataStore.AppPrefs.WindowLocation.Y;
            //    this.Width = DataStore.AppPrefs.WindowLocation.Width;
            //    this.Height = DataStore.AppPrefs.WindowLocation.Height;
            //}

            //// Create and connect our modules to our data manager
            //_dataManager = new DataManager();
            //oComponents.SetDataManager(_dataManager);

            //_viewModel = new MainWindowViewModel(_dataManager);
            //this.DataContext = _viewModel;
		}

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

        private void Button_Click_Subject()
        {
            //// Configure open file dialog box
            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = "Subjects"; // Default file name
            //dlg.DefaultExt = ".txt"; // Default file extension
            //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            //if (_txtSubjectFile.Text != "")
            //    dlg.InitialDirectory = System.IO.Path.GetFullPath(_txtSubjectFile.Text);

            //// Show open file dialog box
            //Nullable<bool> result = dlg.ShowDialog();

            //// Process open file dialog box results
            //if (result == true)
            //{
            //    // Open document
            //    _txtSubjectFile.Text = dlg.FileName;
            //}
        }

        private void Button_Click_Data()
        {
            //// Configure open file dialog box
            //var dlg = new System.Windows.Forms.FolderBrowserDialog();

            //if (_txtDataFolder.Text != "")
            //    dlg.SelectedPath = System.IO.Path.GetFullPath(_txtDataFolder.Text);

            //// Show open file dialog box
            //var result = dlg.ShowDialog();

            //// Process open file dialog box results
            //if (result == System.Windows.Forms.DialogResult.OK)
            //{
            //    // Open document
            //    _txtDataFolder.Text = dlg.SelectedPath;
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
