using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabStorage;
using Caliburn.Micro;
using BrainLab.Events;
using OxyPlot;
using BrainLab.Services;
using BrainLab.Viz;
using BrainLab.Common.Viz;

namespace BrainLab.Sections.NBSm
{
	public class GraphViewModel : Screen, IHandle<NBSResultsAvailable>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator = IoC.Get<IEventAggregator>();
		private IRegionService _regionService = IoC.Get<IRegionService>();
		private IComputeService _computeService = IoC.Get<IComputeService>();
		#endregion

		public GraphViewModel()
		{
			// These are to power lists in the displays
			CmpNodes = new BindableCollection<NodeResult>();
			CmpEdges = new BindableCollection<EdgeResult>();

			_eventAggregator.Subscribe(this);
		}

		public string DataType { get { return _inlDataType; } set { _inlDataType = value; NotifyOfPropertyChange(() => DataType); } } private string _inlDataType;

		protected PlotModel LoadGraph(List<RegionalViewModel> rsvms, List<GraphEdge> edges, Func<ROI, double> horizSelector, Func<ROI, double> vertSelector)
		{
			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotAreaBorderColor = OxyColors.White;
			model.PlotType = PlotType.Cartesian;
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);
			model.Padding = new OxyThickness(0, 0, 0, 0);

			var ba = new InvisibleAxis() { IsAxisVisible = false, Position = AxisPosition.Bottom };
			var la = new InvisibleAxis() { IsAxisVisible = false, Position = AxisPosition.Left };

			ba.MinimumPadding = 0.1;
			ba.MaximumPadding = 0.1;
			la.MinimumPadding = 0.1;
			la.MaximumPadding = 0.1;

			model.Axes.Add(ba);
			model.Axes.Add(la);

			var nonSigNodes = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.Gray),
			};

			var sigNodes = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.Green),
			};

			var sigEdges = new LineSeries
			{
				Color = OxyColor.FromAColor(125, OxyColors.Green),
			};

			Dictionary<int, int> cmpNodes = new Dictionary<int, int>();

			foreach (var edge in edges)
			{
				var v1 = rsvms[edge.V1];
				var v2 = rsvms[edge.V2];

				cmpNodes[edge.V1] = edge.V1;
				cmpNodes[edge.V2] = edge.V2;

				sigEdges.Points.Add(new DataPoint(Double.NaN, Double.NaN));
				sigEdges.Points.Add(new DataPoint(horizSelector(v1.ROI), vertSelector(v1.ROI)));
				sigEdges.Points.Add(new DataPoint(horizSelector(v2.ROI), vertSelector(v2.ROI)));
				sigEdges.Points.Add(new DataPoint(Double.NaN, Double.NaN));
			}

			foreach (var rsvm in rsvms)
			{
				if(cmpNodes.ContainsKey(rsvm.ROI.Index))
					sigNodes.Points.Add(new BrainDataPoint(horizSelector(rsvm.ROI), vertSelector(rsvm.ROI), rsvm.ROI));
				else
					nonSigNodes.Points.Add(new BrainDataPoint(horizSelector(rsvm.ROI), vertSelector(rsvm.ROI), rsvm.ROI));
			}

			model.Series.Add(nonSigNodes);
			model.Series.Add(sigEdges);
			model.Series.Add(sigNodes);

			return model;
		}

		public void Handle(NBSResultsAvailable message)
		{
			var overlap = _computeService.GetResults();
			var regions = _regionService.GetRegionsByIndex();

			var rvms = new List<RegionalViewModel>();
			foreach (var region in regions)
			{
				RegionalViewModel rvm = new RegionalViewModel
				{
					ROI = region,
				};

				rvms.Add(rvm);
			}

			var cmps = overlap.Components[DataType];
			if (cmps != null)
			{
				int cmpSize = 0;
				GraphComponent cmp = null;
				for (var i = 0; i < cmps.Count; i++)
				{
					if (cmps[i].Edges.Count > cmpSize)
					{
						cmp = cmps[i];
						cmpSize = cmp.Edges.Count;

						cmp.DataType = DataType;
					}
				}

				if (cmp != null)
				{
					AXPlotModel = LoadGraph(rvms, cmp.Edges, r => r.X, r => r.Y);
					SGPlotModel = LoadGraph(rvms, cmp.Edges, r => (100 - r.Y), r => r.Z);
					CRPlotModel = LoadGraph(rvms, cmp.Edges, r => r.X, r => r.Z);

					Dictionary<int, int> cmpVerts = new Dictionary<int, int>();

					foreach (var edge in cmp.Edges)
					{
						var v1 = rvms[edge.V1];
						var v2 = rvms[edge.V2];

						cmpVerts[edge.V1] = edge.V1;
						cmpVerts[edge.V2] = edge.V2;

						double diff = edge.M2 - edge.M1;
						double pval = ((double)edge.RightTailCount) / ((double)overlap.Permutations);

						CmpEdges.Add(new EdgeResult { V1 = v1.ROI.Name, V2 = v2.ROI.Name, Diff = diff, PVal = pval });
					}

					CmpPValue = "p" + (((double)cmp.RightTailExtentCount) / ((double)overlap.Permutations)).ToString("0.0000");

					var itms = from v in cmpVerts.Values orderby v select v;
					foreach (var vert in itms)
						CmpNodes.Add(new NodeResult() { Name = rvms[vert].ROI.Name, Id = rvms[vert].ROI.Index });
				}
			}
		}

		protected class RegionalViewModel
		{
			public ROI ROI { get; set; }
		}

		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;
				
        public void Save(StringBuilder htmlSink, string folderPath)
        {
			//htmlSink.AppendFormat("<h1>{0}</h1>", DataType);

			//htmlSink.Append("<h3>Global Strength</h3>");
			//_distro.SaveReport(htmlSink, folderPath);

			//htmlSink.Append("<h3>NBSm</h3>");
			//htmlSink.AppendFormat("<p>Nodes: {0} Edges: {1} pVal: {2}</p>", CmpNodes.Count, CmpEdges.Count, CmpPValue);

			//_graphCrXL.Save(htmlSink, folderPath, DataType, "cr", 300, 250);
			//_graphSgXL.Save(htmlSink, folderPath, DataType, "ax", 250, 250);
			//_graphAxXL.Save(htmlSink, folderPath, DataType, "sg", 350, 250);

			//_graphAxXL.SaveGraphML(folderPath, this.DataType, "sg");
			//_graphSgXL.SaveGraphML(folderPath, this.DataType, "ax");
			//_graphCrXL.SaveGraphML(folderPath, this.DataType, "cr");
        }

		#region Edge stuff
		// Calc corr for this edge
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "CogMem"), dataType, v1, v2);
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "CogAtten"), dataType, v1, v2);
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "delusionsTotal"), dataType, v1, v2);
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "bizarreTotal"), dataType, v1, v2);
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "disorganizationTotal"), dataType, v1, v2);
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "hallucinationsTotal"), dataType, v1, v2);
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "sansTotal"), dataType, v1, v2);
		//OutputVals(_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "sapsTotal"), dataType, v1, v2);
		//private void OutputVals(List<DataManager.EdgeStats> stats, string dataType, ROIVertex v1, ROIVertex v2)
		//{
		//	foreach (var es in stats)
		//	{
		//		if (es.Both < 0.05)
		//		{
		//			string str = string.Format("{6} {0} {1} {2} {3} {4} ({5})", dataType, es.Group, v1.Roi.Name, v2.Roi.Name, es.Corr.ToString("0.0000"), es.Both.ToString("0.0000"), es.Measure);
		//			//_results += "\n" + str;
		//		}
		//	}
		//}
		#endregion

		public string CmpPValue
		{
			get { return _cmpPValue; }
			set { _cmpPValue = value; NotifyOfPropertyChange(() => CmpPValue); }
		} private string _cmpPValue;

		public BindableCollection<NodeResult> CmpNodes { get; private set; }
		public BindableCollection<EdgeResult> CmpEdges { get; private set; }
	}

	public class NodeResult
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class EdgeResult
	{
		public string V1 { get; set; }
		public string V2 { get; set; }
		public double Diff { get; set; }
		public double PVal { get; set; }
		public double TStat { get; set; }
	}

	public class ROIVertex
	{
		public ROI Roi;

		public double XF;
		public double YF;
		public double ZF;

		public bool Highlight;
	}

    public class ROIDim
    {
        public double Factor;
        public double Raw;
    }
}
