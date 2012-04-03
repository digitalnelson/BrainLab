using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using System.Diagnostics;
using BrainLabStorage;
using BrainLabLibrary;
using System.Threading.Tasks;

namespace BrainLab.Studio
{
	public partial class MainWindow : Window
	{
		private MainWindowViewModel _viewModel;
		private DataManager _dataManager;
		Dictionary<string, double> _thresholds;

		public MainWindow()
		{
			InitializeComponent();

			// Set up user prefs for file locations
			_txtRegionFile.Text = DataStore.AppPrefs.RoiFilePath;
			_txtSubjectFile.Text = DataStore.AppPrefs.SubjectfilePath;
			_txtDataFolder.Text = DataStore.AppPrefs.DataFileDir;

			// Set up last window location and size
			if (DataStore.AppPrefs.WindowLocation != null &&  DataStore.AppPrefs.WindowLocation.Width != 0.0d && DataStore.AppPrefs.WindowLocation.Height != 0.0d)
			{
				this.Left = DataStore.AppPrefs.WindowLocation.X;
				this.Top = DataStore.AppPrefs.WindowLocation.Y;
				this.Width = DataStore.AppPrefs.WindowLocation.Width;
				this.Height = DataStore.AppPrefs.WindowLocation.Height;
			}

			// Create and connect our modules to our data manager
			_dataManager = new DataManager();
            oComponents.SetDataManager(_dataManager);
			dComponents.SetDataManager(_dataManager);
			fComponents.SetDataManager(_dataManager);

			_viewModel = new MainWindowViewModel(_dataManager);
			this.DataContext = _viewModel;
		}
				
		private async void Load(object sender, RoutedEventArgs e)
		{
			string regionFile = _txtRegionFile.Text;
			string subjectFile = _txtSubjectFile.Text;
			string dataFolder = _txtDataFolder.Text;

			_btnData.IsEnabled = false;
			await _viewModel.Load(regionFile, subjectFile, dataFolder);
			_btnPermute.IsEnabled = true;
		}

		private async void Permute(object sender, RoutedEventArgs e)
		{
            oComponents.Clear();
            dComponents.Clear();
            fComponents.Clear();
	
			await _viewModel.Permute();

            Overlap overlap = _dataManager.GetOverlap();

            oComponents.LoadGraphComponents(overlap, "DTI");
            dComponents.LoadGraphComponents(overlap, "DTI", Color.FromArgb(255, 0, 255, 0));
            fComponents.LoadGraphComponents(overlap, "fMRI", Color.FromArgb(255, 0, 0, 255));

			//_btnDisplay.IsEnabled = true;
		}

		private void Display(object sender, RoutedEventArgs e)
		{
			//_btnReport.IsEnabled = true;

			//DistroSummary ds = null;
			//for (double d = 2.16; d < 2.19; d += 0.001)
			//{
			//	ds = permStore.ProcessDimension("DTI", d);
			//	Console.WriteLine("DTI - T {0} Size {1}", d, ds.LargestComponentTopoExtent);
			//}

			//for (double d = 5.35; d < 5.40; d += 0.005)
			//{
			//	ds = permStore.ProcessDimension("fMRI", d);
			//	Console.WriteLine("fMRI - T {0} Size {1}", d, ds.LargestComponentTopoExtent);
			//}

			//Console.WriteLine("########## DTI NODES");
			//DistroSummary dsDTI = permStore.ProcessDimension("DTI", 2.15);
			//foreach (KeyValuePair<int, List<int>> kvp in dsDTI.Nodes)
			//{
			//	//if (kvp.Value.Count > 30)
			//	{
			//		Console.WriteLine("{0} ----------------------------", kvp.Value.Count);
			//		foreach (int i in kvp.Value)
			//		{
			//			Console.WriteLine("{0}", aalByIndex[i].Name);
			//		}
			//	}
			//}

			//Console.WriteLine("########## fMRI Nodes");
			//DistroSummary dsfMRI = permStore.ProcessDimension("fMRI", 3.225);
			//foreach (KeyValuePair<int, List<int>> kvp in dsfMRI.Nodes)
			//{
			//	//if (kvp.Value.Count > 29)
			//	{
			//		Console.WriteLine("{0} ----------------------------", kvp.Value.Count);
			//		foreach (int i in kvp.Value)
			//		{
			//			Console.WriteLine("{0}", aalByIndex[i].Name);
			//		}
			//	}
			//}

			//// Process global measures
			//gfStrength.Load(aalByIndex, "fMRI Global Strength", DoubleArray.From(ctls.Select(s => s.Graphs["fMRI"].StrengthGlobal)), DoubleArray.From(pros.Select(s => s.Graphs["fMRI"].StrengthGlobal)));
			//gdStrength.Load(aalByIndex, "DTI Global Strength", DoubleArray.From(ctls.Select(s => s.Graphs["DTI"].StrengthGlobal)), DoubleArray.From(pros.Select(s => s.Graphs["DTI"].StrengthGlobal)));

			//fStrengthO.LoadData(aalByIndex, "fMRI Strength", DoubleArray.From(ctls.Select(s => s.Graphs["fMRI"].Strength)), DoubleArray.From(pros.Select(s => s.Graphs["fMRI"].Strength)), SeriesChartType.Column, 0.05, 0.0005);
			//dStrengthO.LoadData(aalByIndex, "DTI Strength", DoubleArray.From(ctls.Select(s => s.Graphs["DTI"].Strength)), DoubleArray.From(pros.Select(s => s.Graphs["DTI"].Strength)), SeriesChartType.Column, 0.05, 0.0005);

			//fStrengthD.LoadData(aalByIndex, "fMRI Strength", DoubleArray.From(ctls.Select(s => s.Graphs["fMRI"].Strength)), DoubleArray.From(pros.Select(s => s.Graphs["fMRI"].Strength)), SeriesChartType.Column, 0.05, 0.0005);
			//dStrengthD.LoadData(aalByIndex, "DTI Strength", DoubleArray.From(ctls.Select(s => s.Graphs["DTI"].Strength)), DoubleArray.From(pros.Select(s => s.Graphs["DTI"].Strength)), SeriesChartType.Column, 0.05, 0.0005);

			//fEdgeOverview.LoadData(aalByIndex, "fMRI", DoubleArray.From(ctls.Select(s => s.Graphs["fMRI"].AdjMatrix)), DoubleArray.From(pros.Select(s => s.Graphs["fMRI"].AdjMatrix)), SeriesChartType.Column, permStore);
			//dEdgeOverview.LoadData(aalByIndex, "DTI", DoubleArray.From(ctls.Select(s => s.Graphs["DTI"].AdjMatrix)), DoubleArray.From(pros.Select(s => s.Graphs["DTI"].AdjMatrix)), SeriesChartType.Column, permStore);
		}

		public void Report(object sender, RoutedEventArgs e)
		{
			string dReport = dComponents.GetReport();
			string fReport = fComponents.GetReport();

			string dta = string.Format("DTI\n{0}\nfMRI{1}", dReport, fReport);

			Report rpt = new Report();
			rpt.SetData(dta);

			rpt.ShowDialog();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			DataStore.AppPrefs.RoiFilePath = _txtRegionFile.Text;
			DataStore.AppPrefs.SubjectfilePath = _txtSubjectFile.Text;
			DataStore.AppPrefs.DataFileDir = _txtDataFolder.Text;

			DataStore.AppPrefs.WindowLocation.X = this.Left;
			DataStore.AppPrefs.WindowLocation.Y = this.Top;
			DataStore.AppPrefs.WindowLocation.Width = this.Width;
			DataStore.AppPrefs.WindowLocation.Height = this.Height;
			
			DataStore.Save();
			DataStore.Close();
		}
	}
}