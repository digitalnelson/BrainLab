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
using ShoNS.Array;
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
		private DataManager _dataManager;
		Dictionary<string, double> _thresholds;

		public MainWindow()
		{
			InitializeComponent();

			_dataManager = new DataManager();
			dComponents.SetDataManager(_dataManager);
			fComponents.SetDataManager(_dataManager);
		}
				
		private async void Load(object sender, RoutedEventArgs e)
		{
			string regionFile = _txtRegionFile.Text;
			string subjectFile = _txtSubjectFile.Text;
			string dataFolder = _txtDataFolder.Text;
			string vertexCount = _txtVertexCount.Text;
			double threshold = Double.Parse(_txtThreshold.Text);

			_btnData.IsEnabled = false;

			await Task.Run(delegate
			{
				// Load the data files into the data manager
				_dataManager.LoadROIFile(regionFile);
				_dataManager.LoadSubjectFile(subjectFile);
				_dataManager.LoadAdjFiles(dataFolder, Int32.Parse(vertexCount));

				// Allow each data source to be NBS thresholded at a different level
				_thresholds = new Dictionary<string, double>();
				_thresholds["DTI"] = threshold; //2.0; //2.15;
				_thresholds["fMRI"] = threshold; //2.15; //3.225;

				// Load the graphs into the comparison system
				_dataManager.LoadComparisons();
			});
			

			_btnPermute.IsEnabled = true;
		}

		private async void Permute(object sender, RoutedEventArgs e)
		{
			int numOfPerms = Int32.Parse(_txtPermutations.Text);
			long dur = 0;

			_btnPermute.IsEnabled = false;

			await Task.Run(delegate
			{
				// Calculate our group differences
				_dataManager.CalculateGroupDifferences("0", "1", _thresholds);

				Stopwatch sw = new Stopwatch();
				sw.Start();

				// Run permutations
				_dataManager.PermuteComparisons(numOfPerms, 29, _thresholds);  // TODO: Make the subject size dynamic based on groups chosen

				sw.Stop();
				dur = sw.ElapsedMilliseconds;
			});

			// Put together a quick timing popup for debugging release mode
			MessageBox.Show("Elapsed - " + dur.ToString());

			_btnDisplay.IsEnabled = true;
		}

		private void Display(object sender, RoutedEventArgs e)
		{
			Dictionary<string, List<GraphComponent>> cmps = _dataManager.GetGraphComponents();

			dComponents.LoadGraphComponents(cmps["DTI"]);
			fComponents.LoadGraphComponents(cmps["fMRI"]);

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
	}
}

