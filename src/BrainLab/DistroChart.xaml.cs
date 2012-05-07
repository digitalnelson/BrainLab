using System;
using System.Collections.Generic;
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
using BrainLabStorage;
using System.IO;
using OxyPlot;
using System.ComponentModel;

namespace BrainLab.Studio
{
	/// <summary>
	/// Interaction logic for DistroChart.xaml
	/// </summary>
	public partial class DistroChart : UserControl,  INotifyPropertyChanged
	{
		private DataManager _dataManager;
		private Chart _chart;
		private string _mainAreaName = "main";

		public DistroChart()
		{
			InitializeComponent();

			_gridRoot.DataContext = this;
		}

		public PlotModel PlotModel 
		{
			get
			{
				return _plotModel;
			}
			set
			{
				_plotModel = value;
				RaisePropertyChanged("PlotModel");
			}
		}
		private PlotModel _plotModel;

		public void SetDataManager(DataManager dataManager)
		{
			_dataManager = dataManager;
		}

		private static double GetMedian(IEnumerable<double> values)
		{
			var sortedInterval = new List<double>(values);
			sortedInterval.Sort();
			var count = sortedInterval.Count;
			if (count % 2 == 1)
			{
				return sortedInterval[(count - 1) / 2];
			}

			return 0.5 * sortedInterval[count / 2] + 0.5 * sortedInterval[(count / 2) - 1];
		}

		public BoxPlotItem MakeBoxPlotItem(int index, List<double> values)
		{
			values.Sort();
			var median = GetMedian(values);

			int r = values.Count % 2;
			double firstQuartil = GetMedian(values.Take((values.Count + r) / 2));
			double thirdQuartil = GetMedian(values.Skip((values.Count - r) / 2));

			var iqr = thirdQuartil - firstQuartil;
			var step = iqr * 1.5;
			
			var upperWhisker = thirdQuartil + step;
			upperWhisker = values.Where(v => v <= upperWhisker).Max();
			
			var lowerWhisker = firstQuartil - step;
			lowerWhisker = values.Where(v => v >= lowerWhisker).Min();

			var outliers = values.Where(v => v > upperWhisker || v < lowerWhisker).ToList();

			return new BoxPlotItem(index, lowerWhisker, firstQuartil, median, thirdQuartil, upperWhisker, outliers);
		}

		public void Load(string dataType, string group1, string group2)
		{
			var grp1 = _dataManager.FilteredSubjectDataByGroup[group1];
			var grp2 = _dataManager.FilteredSubjectDataByGroup[group2];

			List<double> grp1Vals = new List<double>();
			foreach (var sub in grp1)
				grp1Vals.Add(sub.Graphs[dataType].GlobalStrength());

			List<double> grp2Vals = new List<double>();
			foreach (var sub in grp2)
				grp2Vals.Add(sub.Graphs[dataType].GlobalStrength());

			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);

			model.Axes.Add(new CategoryAxis("", "Probands", "Controls"));
			model.Axes.Add(new LinearAxis(AxisPosition.Left) { MinimumPadding = 0.1, MaximumPadding = 0.1 });

			var s1 = new BoxPlotSeries
			{
				Title = "BoxPlotSeries",
				Fill = OxyColors.White,
				Stroke = OxyColors.Blue,
				StrokeThickness = 2,
				OutlierSize = 2,
				BoxWidth = 0.4
			};

			s1.Items.Add(MakeBoxPlotItem(0, grp1Vals));
			s1.Items.Add(MakeBoxPlotItem(1, grp2Vals));

			model.Series.Add(s1);			
			PlotModel = model;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string property)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(property));
			}
		}
	}
}
