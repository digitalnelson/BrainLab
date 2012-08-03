using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrainLab.Services;
using Caliburn.Micro;
using OxyPlot;

namespace BrainLab.Sections.Graph
{
	public partial class StrengthViewModel : Screen
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IComputeService _computeService;

		public StrengthViewModel(IEventAggregator eventAggregator, IComputeService computeService)
		{
			_eventAggregator = eventAggregator;
			_computeService = computeService;
		}

		public PlotModel GlobalPlotModel { get { return _inlGlobalPlotModel; } set { _inlGlobalPlotModel = value; NotifyOfPropertyChange(() => GlobalPlotModel); } } private PlotModel _inlGlobalPlotModel;
		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;

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

		public void Load(string dataType)
		{
			DataType = dataType;

			var filtSubs = _computeService.GetFilteredSubjectsByComputeGroup();

			var grp1 = filtSubs[ComputeGroup.GroupOne];
			var grp2 = filtSubs[ComputeGroup.GroupTwo];

			List<List<double>> vtxStrs1 = new List<List<double>>();
			List<List<double>> vtxStrs2 = new List<List<double>>();
			for (var i = 0; i < 90; i++)
			{
				vtxStrs1.Add(new List<double>());
				vtxStrs2.Add(new List<double>());
			}

			List<double> grp1Vals = new List<double>();
			foreach (var sub in grp1)
			{
				List<float> regStrs = sub.Graphs[dataType].MeanVtxStrength();
				for (int i = 0; i < regStrs.Count; i++)
					vtxStrs1[i].Add(regStrs[i]);

				float globalStr = regStrs.Average();
				float glbStr = sub.Graphs[dataType].GlobalStrength();
				grp1Vals.Add(glbStr);
			}

			List<double> grp2Vals = new List<double>();
			foreach (var sub in grp2)
			{
				List<float> regStrs = sub.Graphs[dataType].MeanVtxStrength();
				for (int i = 0; i < regStrs.Count; i++)
					vtxStrs2[i].Add(regStrs[i]);

				float globalStr = regStrs.Average();
				float glbStr = sub.Graphs[dataType].GlobalStrength();
				grp2Vals.Add(glbStr);
			}

			List<double> diffs = new List<double>();
			List<double> pVals = new List<double>();
			for (int i = 0; i < 90; i++)
			{
				var lst1 = vtxStrs1[i];
				var lst2 = vtxStrs2[i];

				double ybt = 0; double ylt = 0; double yrt = 0;
				alglib.studentttest2(lst1.ToArray(), lst1.Count, lst2.ToArray(), lst2.Count, out ybt, out ylt, out yrt);

				if (ybt < 0.05)
				{
					diffs.Add(lst1.Average() - lst2.Average());
					pVals.Add(ybt);
				}
			}

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

		public void SaveReport(StringBuilder htmlSink, string folderPath)
		{
			//if (_plot != null)
			//{
			//	// TODO: May want to tack on a guid so things don't overwrite
			//	string fileName = "GlobalStrength_" + _dataType + ".svg";
			//	htmlSink.AppendFormat("<span>Diff: {0} Tests: {1}</span><br/>", GrpDiff, PValue);
			//	htmlSink.AppendFormat("<img src=\"{0}\" />\n", fileName);

			//	_plotModel.SaveSvg(System.IO.Path.Combine(folderPath, fileName), 300, 250);
			//}
		}
	}
}

