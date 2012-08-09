using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BrainLab.Services;
using BrainLab.Viz;
using BrainLabLibrary;
using BrainLabStorage;
using Caliburn.Micro;
using OxyPlot;

namespace BrainLab.Sections.Graph
{
	public partial class StrengthViewModel : Screen
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IComputeService _computeService;
		private readonly IRegionService _regionService;
		private readonly IAppPreferences _appPreferences;

		public StrengthViewModel(IEventAggregator eventAggregator, IComputeService computeService, IRegionService regionService, IAppPreferences appPreferences)
		{
			_eventAggregator = eventAggregator;
			_computeService = computeService;
			_regionService = regionService;
			_appPreferences = appPreferences;
		}

		public PlotModel GlobalPlotModel { get { return _inlGlobalPlotModel; } set { _inlGlobalPlotModel = value; NotifyOfPropertyChange(() => GlobalPlotModel); } } private PlotModel _inlGlobalPlotModel;
		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;

		public string DataType { get { return _inlDataType; } set { _inlDataType = value; NotifyOfPropertyChange(() => DataType); } } private string _inlDataType;
		public string GrpDiff { get { return _inlGrpDiff; } set { _inlGrpDiff = value; NotifyOfPropertyChange(() => GrpDiff); } } private string _inlGrpDiff;
		public string PValue { get { return _inlPValue; } set { _inlPValue = value; NotifyOfPropertyChange(() => PValue); } } private string _inlPValue;

		private double GetMedian(IEnumerable<double> values)
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

		protected class RegionalStrengthViewModel
		{
			public ROI ROI { get; set; }
			public double PValue { get; set; }
			public double GroupDifference { get; set; }
		}

		protected List<double> CalculateStrength(List<Subject> subjects, string dataType, List<List<double>> vertexStrengths)
		{
			List<double> globalStrengths = new List<double>();

			foreach (var sub in subjects)
			{
				List<float> regStrs = sub.Graphs[dataType].MeanVtxStrength();
				for (int i = 0; i < regStrs.Count; i++)
					vertexStrengths[i].Add(regStrs[i]);

				float globalStr = regStrs.Average();
				float glbStr = sub.Graphs[dataType].GlobalStrength();
				globalStrengths.Add(glbStr);
			}

			return globalStrengths;
		}

		protected PlotModel LoadPlotModel(List<RegionalStrengthViewModel> rsvms, Func<ROI, double> horizSelector, Func<ROI, double> vertSelector)
		{
			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);

			var ba = new LinearAxis(AxisPosition.Bottom) { AbsoluteMaximum = 100, AbsoluteMinimum = 0, Maximum = 100, Minimum = 0 };
			var la = new LinearAxis(AxisPosition.Left) { AbsoluteMaximum = 100, AbsoluteMinimum = 0, Maximum = 100, Minimum = 0 };

			model.Axes.Add(ba);
			model.Axes.Add(la);

			var s1 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.Gray),
			};

			var s2 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.Green),
				MarkerStroke = OxyColors.Black,
			};

			var s3 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.Red),
				MarkerStroke = OxyColors.Black,
			};

			foreach (var rsvm in rsvms)
			{
				if (rsvm.PValue < 0.0005)
					s3.Points.Add(new BrainDataPoint(horizSelector(rsvm.ROI), vertSelector(rsvm.ROI), rsvm.ROI));
				else if (rsvm.PValue < 0.05)
					s2.Points.Add(new BrainDataPoint(horizSelector(rsvm.ROI), vertSelector(rsvm.ROI), rsvm.ROI));
				else
					s1.Points.Add(new BrainDataPoint(horizSelector(rsvm.ROI), vertSelector(rsvm.ROI), rsvm.ROI));
			}

			model.Series.Add(s1);
			model.Series.Add(s2);
			model.Series.Add(s3);

			return model;
		}

		private List<RegionalStrengthViewModel> _rsvms;

		public void Load(string dataType)
		{
			DataType = dataType;

			var regions = _regionService.GetRegionsByIndex();
			var filtSubs = _computeService.GetFilteredSubjectsByComputeGroup();

			var grp1 = filtSubs[ComputeGroup.GroupOne];
			var grp2 = filtSubs[ComputeGroup.GroupTwo];

			List<List<double>> vtxStrs1 = new List<List<double>>();
			List<List<double>> vtxStrs2 = new List<List<double>>();
			foreach (var r in regions)
			{
				vtxStrs1.Add(new List<double>());
				vtxStrs2.Add(new List<double>());
			}

			// Calc our global and vertex strengths for our groups
			List<double> grp1Vals = CalculateStrength(grp1, dataType, vtxStrs1);
			List<double> grp2Vals = CalculateStrength(grp2, dataType, vtxStrs2);

			// Sig test the verticies
			_rsvms = new List<RegionalStrengthViewModel>();

			for (int i = 0; i < 90; i++)
			{
				var lst1 = vtxStrs1[i];
				var lst2 = vtxStrs2[i];

				double ybt = 0; double ylt = 0; double yrt = 0;
				alglib.studentttest2(lst1.ToArray(), lst1.Count, lst2.ToArray(), lst2.Count, out ybt, out ylt, out yrt);

				RegionalStrengthViewModel rsvm = new RegionalStrengthViewModel
				{
					ROI = regions[i],
					GroupDifference = lst1.Average() - lst2.Average(),
					PValue = ybt,
				};

				_rsvms.Add(rsvm);
			}

			AXPlotModel = LoadPlotModel(_rsvms, r => r.X, r => r.Y);
			SGPlotModel = LoadPlotModel(_rsvms, r => (100 - r.Y), r => r.Z);
			CRPlotModel = LoadPlotModel(_rsvms, r => r.X, r => r.Z);

			double bt = 0; double lt = 0; double rt = 0;
			alglib.mannwhitneyutest(grp1Vals.ToArray(), grp1Vals.Count, grp2Vals.ToArray(), grp2Vals.Count, out bt, out lt, out rt);

			double tbt = 0; double tlt = 0; double trt = 0;
			alglib.studentttest2(grp1Vals.ToArray(), grp1Vals.Count, grp2Vals.ToArray(), grp2Vals.Count, out tbt, out tlt, out trt);

			GrpDiff = (grp1Vals.Average() - grp2Vals.Average()).ToString("0.0000");
			PValue = "p" + tbt.ToString("0.0000") + " u" + bt.ToString("0.0000");

			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);

			model.Axes.Add(new CategoryAxis("", "Group 1", "Group 2"));
			model.Axes.Add(new LinearAxis(AxisPosition.Left) { MinimumPadding = 0.1, MaximumPadding = 0.1, IntervalLength = 20 });

			var s1 = new BoxPlotSeries
			{
				Title = "BoxPlotSeries",
				Fill = OxyColors.White,
				Stroke = OxyColors.DarkBlue,
				StrokeThickness = 2,
				OutlierSize = 2,
				BoxWidth = 0.4
			};

			s1.Items.Add(MakeBoxPlotItem(0, grp1Vals));
			s1.Items.Add(MakeBoxPlotItem(1, grp2Vals));

			model.Series.Add(s1);
			GlobalPlotModel = model;
		}

		public void Save()
		{
			// Configure open file dialog box
			var dlg = new System.Windows.Forms.FolderBrowserDialog();

			if (!string.IsNullOrEmpty(_appPreferences.OutputPath))
				dlg.SelectedPath = System.IO.Path.GetFullPath(_appPreferences.OutputPath);
			else if (!string.IsNullOrEmpty(_appPreferences.DataPath))
				dlg.SelectedPath = System.IO.Path.GetFullPath(_appPreferences.DataPath);

			// Show open file dialog box
			var result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				_appPreferences.OutputPath = dlg.SelectedPath;

				string imageName = DataType + "_{0}" + ".svg";
				GlobalPlotModel.SaveSvg(System.IO.Path.Combine(dlg.SelectedPath, string.Format(imageName, "GlobalStrength")), 300, 250);
				AXPlotModel.SaveSvg(System.IO.Path.Combine(dlg.SelectedPath, string.Format(imageName, "RegionalStrength_AX")), 300, 300);
				SGPlotModel.SaveSvg(System.IO.Path.Combine(dlg.SelectedPath, string.Format(imageName, "RegionalStrength_SG")), 300, 300);
				CRPlotModel.SaveSvg(System.IO.Path.Combine(dlg.SelectedPath, string.Format(imageName, "RegionalStrength_CR")), 300, 300);

				string reportName = DataType + "_{0}" + ".txt";
				using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(dlg.SelectedPath, string.Format(reportName, "Strength"))))
				{
					sw.WriteLine("Global Strength");
					sw.WriteLine(string.Format("Group Diff: {0}", GrpDiff));
					sw.WriteLine(string.Format("PValue: {0}", PValue));

					sw.WriteLine();
					sw.WriteLine("Regional Strength");
					sw.WriteLine("pval   \tdiff  \tregion");

					if(_rsvms != null)
					{
						foreach(var rvm in _rsvms)
							sw.WriteLine(string.Format("{2}\t{1}\t{0}", rvm.ROI.Name, rvm.GroupDifference.ToString("+0.00000;-0.00000;0"), rvm.PValue.ToString("0.00000")));
					}
				}
			}
		}
	}
}

