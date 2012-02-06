using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Smrf.NodeXL.Core;
using Smrf.NodeXL.Adapters;
using Smrf.NodeXL.Visualization.Wpf;
using Smrf.WpfGraphicsLib;
using BrainLabStorage;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace BrainLab.Studio
{
	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	public partial class GraphView : UserControl, INotifyPropertyChanged
	{
		public GraphView()
		{
			InitializeComponent();

			_ctl.DataContext = this;
			InterModalNodes = new ObservableCollection<string>();
			InterModalEdges = new ObservableCollection<string>();
			CmpNodes = new ObservableCollection<string>();
			CmpEdges = new ObservableCollection<string>();

			m_oNodeXLWithAxesControl = new NodeXLWithAxesControl();
			m_oNodeXLWithAxesControl.XAxis.Label = "R  ---  L";
			m_oNodeXLWithAxesControl.YAxis.Label = "P  ---  A";
			m_oNodeXLControl = m_oNodeXLWithAxesControl.NodeXLControl;

			_graphXL.Child = m_oNodeXLWithAxesControl;
		}

		public void SetDataManager(DataManager dataManager)
		{
			_dataManager = dataManager;
		}

		public void LoadGraphComponents(Overlap overlap, string dataType)
		{
			DataType = dataType;
			InterModalPValue = ((double)overlap.RightTailOverlapCount) / ((double)overlap.Permutations);

			List<GraphComponent> components = overlap.Components[dataType];
			
			
			
			
			
			
			
			double width = _graphXL.ActualWidth - 30;
			double height = _graphXL.ActualHeight - 30;
			double xRange = _dataManager.XMax - _dataManager.XMin;
			double yRange = _dataManager.YMax - _dataManager.YMin;

			IGraph g = m_oNodeXLControl.Graph;
			IVertexCollection vc = g.Vertices;
			IEdgeCollection ec = g.Edges;

			_mapVtx = new Dictionary<int, ROIVertex>();

			for (var i = 0; i < 90; i++)
			{
				ROI roi = _dataManager.GetROI(i);

				double xFactor = (roi.X - _dataManager.XMin) / xRange;
				double yFactor = (roi.Y - _dataManager.YMin) / yRange;

				double x = xFactor * width;
				double y = yFactor * height;
				y = height - y;

				IVertex v = vc.Add();

				if (roi.Special)
				{
					InterModalNodes.Add(string.Format("{0} ({1}) ", roi.Name, roi.Index));
				}
				else
				{
					v.SetValue(ReservedMetadataKeys.PerAlpha, 40.0f);
				}

				v.SetValue(ReservedMetadataKeys.PerVertexLabel, roi.Name);
				v.SetValue(ReservedMetadataKeys.PerVertexRadius, 11.0f);
				v.SetValue(ReservedMetadataKeys.LockVertexLocation, true);

				v.Location = new System.Drawing.PointF((float)x + 15, (float)y + 15);
				
				_mapVtx[i] = new ROIVertex() { Vertex = v, Roi = roi };
			}

			int cmpSize = 0;
			GraphComponent cmp = null;
			for (var i = 0; i < components.Count; i++ )
			{
				double pval = ((double)components[i].RightTailExtentCount) / ((double)overlap.Permutations);

				if ((pval < 0.05) && (components[i].Edges.Count > cmpSize))
				{
					cmp = components[i];
					cmpSize = cmp.Edges.Count;
				}
			}

			if (cmp != null)
			{
				CmpPValue = ((double)cmp.RightTailExtentCount) / ((double)overlap.Permutations);
				Dictionary<int, ROIVertex> cmpVerts = new Dictionary<int, ROIVertex>();

				foreach (var edge in cmp.Edges)
				{
					ROIVertex v1 = _mapVtx[edge.V1];
					ROIVertex v2 = _mapVtx[edge.V2];

					if (!cmpVerts.ContainsKey(edge.V1))
						cmpVerts[edge.V1] = v1;
					if (!cmpVerts.ContainsKey(edge.V2))
						cmpVerts[edge.V2] = v2;

					IEdge e = ec.Add(v1.Vertex, v2.Vertex);

					double diff = edge.M2 - edge.M1;
					double pval = ((double)edge.RightTailCount) / ((double)overlap.Permutations);

					string lbl = string.Format("{0} ({1})", diff.ToString("0.000"), pval.ToString("0.0000"));

					if (v1.Roi.Special && v2.Roi.Special)
					{
						//e.SetValue(ReservedMetadataKeys.PerEdgeLabel, lbl);
						//e.SetValue(ReservedMetadataKeys.PerEdgeLabelFontSize, 16.0f);
						e.SetValue(ReservedMetadataKeys.PerEdgeWidth, 4.0f);
						e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 0, 0, 0));

						InterModalEdges.Add(string.Format("{0} - {1} [{2} ({3} {4})] ", v1.Roi.Name, v2.Roi.Name, diff.ToString("0.000"), edge.TStat.ToString("0.00"), pval.ToString("0.0000")));
					}
					else
					{
						//e.SetValue(ReservedMetadataKeys.PerEdgeLabel, lbl);
						e.SetValue(ReservedMetadataKeys.PerAlpha, 70.0f);
						e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 0, 0, 0));
					}

					CmpEdges.Add(string.Format("{0} - {1} [{2} ({3} {4})] ", v1.Roi.Name, v2.Roi.Name, diff.ToString("0.000"), edge.TStat.ToString("0.00"), pval.ToString("0.0000")));
				}

				var itms = from v in cmpVerts.Values orderby v.Roi.Index select v;
				foreach (var vert in itms)
					CmpNodes.Add(string.Format("{0} ({1})", vert.Roi.Name, vert.Roi.Index));

				m_oNodeXLControl.DrawGraph(true);
			}
		}

		public string GetReport()
		{
			StringBuilder sbReport = new StringBuilder();

			sbReport.AppendLine("Inter Modal Nodes");
			foreach (var itm in InterModalNodes)
				sbReport.AppendLine(itm);
			
			sbReport.AppendLine("Inter Modal Edges");
			foreach (var itm in InterModalEdges)
				sbReport.AppendLine(itm);

			sbReport.AppendLine("Cmp Nodes");
			foreach (var itm in CmpNodes)
				sbReport.AppendLine(itm);

			sbReport.AppendLine("Cmp Edges");
			foreach (var itm in CmpEdges)
				sbReport.AppendLine(itm);

			return sbReport.ToString();
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
		private void OutputVals(List<DataManager.EdgeStats> stats, string dataType, ROIVertex v1, ROIVertex v2)
		{
			foreach (var es in stats)
			{
				if (es.Both < 0.05)
				{
					string str = string.Format("{6} {0} {1} {2} {3} {4} ({5})", dataType, es.Group, v1.Roi.Name, v2.Roi.Name, es.Corr.ToString("0.0000"), es.Both.ToString("0.0000"), es.Measure);
					//_results += "\n" + str;
				}
			}
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		public string DataType
		{
			get { return _dataType; }
			set { _dataType = value; NotifyPropertyChanged("DataType"); }
		} private string _dataType;

		public double InterModalPValue
		{
			get { return _interModalPValue; }
			set { _interModalPValue = value; NotifyPropertyChanged("InterModalPValue"); }
		} private double _interModalPValue;
		
		public double CmpPValue
		{
			get { return _cmpPValue; }
			set { _cmpPValue = value; NotifyPropertyChanged("CmpPValue"); }
		} private double _cmpPValue;

		public ObservableCollection<string> InterModalNodes { get; private set; }
		public ObservableCollection<string> InterModalEdges { get; private set; }
		public ObservableCollection<string> CmpNodes { get; private set; }
		public ObservableCollection<string> CmpEdges { get; private set; }

		protected void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		private DataManager _dataManager;
		private NodeXLWithAxesControl m_oNodeXLWithAxesControl;
		private NodeXLControl m_oNodeXLControl;
		private Dictionary<int, ROIVertex> _mapVtx = null;
	}

	class ROIVertex
	{
		public IVertex Vertex;
		public ROI Roi;
	}
}
