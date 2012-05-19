using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrainLab.Studio
{
	/// <summary>
	/// Interaction logic for SpatialPlot.xaml
	/// </summary>
	public partial class SpatialPlot : UserControl
	{
		private DataManager _dataManager;
		private string _dataType = "";

		public SpatialPlot()
		{
			InitializeComponent();

			_gridRoot.DataContext = this;
		}

		public void SetDataManager(DataManager dataManager)
		{
			_dataManager = dataManager;
		}

		public void Load(string dataType, string group1, string group2)
		{
			_dataType = dataType;

			var grp1 = _dataManager.FilteredSubjectDataByGroup[group1];
			var grp2 = _dataManager.FilteredSubjectDataByGroup[group2];

			List<double> grp1Vals = new List<double>();
			foreach (var sub in grp1)
			{
				List<float> regStrs = sub.Graphs[dataType].MeanVtxStrength();
				grp1Vals.Add(sub.Graphs[dataType].GlobalStrength());
			}

			List<double> grp2Vals = new List<double>();
			foreach (var sub in grp2)
				grp2Vals.Add(sub.Graphs[dataType].GlobalStrength());

			//double bt = 0; double lt = 0; double rt = 0;
			//alglib.mannwhitneyutest(grp1Vals.ToArray(), grp1Vals.Count, grp2Vals.ToArray(), grp2Vals.Count, out bt, out lt, out rt);

			//double tbt = 0; double tlt = 0; double trt = 0;
			//alglib.studentttest2(grp1Vals.ToArray(), grp1Vals.Count, grp2Vals.ToArray(), grp2Vals.Count, out tbt, out tlt, out trt);

			//GrpDiff = (grp1Vals.Average() - grp2Vals.Average()).ToString("0.0000");
			//PValue = "p" + tbt.ToString("0.0000") + " u" + bt.ToString("0.0000");

			//var model = new PlotModel() { IsLegendVisible = false };
			//model.PlotMargins = new OxyThickness(0, 0, 0, 0);

			//model.Axes.Add(new CategoryAxis("", "Controls", "Probands"));
			//model.Axes.Add(new LinearAxis(AxisPosition.Left) { MinimumPadding = 0.1, MaximumPadding = 0.1 });

			//var s1 = new BoxPlotSeries
			//{
			//	Title = "BoxPlotSeries",
			//	Fill = OxyColors.White,
			//	Stroke = OxyColors.Blue,
			//	StrokeThickness = 2,
			//	OutlierSize = 2,
			//	BoxWidth = 0.4
			//};

			//s1.Items.Add(MakeBoxPlotItem(0, grp1Vals));
			//s1.Items.Add(MakeBoxPlotItem(1, grp2Vals));

			//model.Series.Add(s1);
			//PlotModel = model;
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
