using System;
using System.Collections.Generic;
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

namespace BrainLab.Studio
{
	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	public partial class GraphView : UserControl
	{
		public GraphView()
		{
			InitializeComponent();

			m_oNodeXLWithAxesControl = new NodeXLWithAxesControl();
			m_oNodeXLWithAxesControl.XAxis.Label = "R  ---  L";
			m_oNodeXLWithAxesControl.YAxis.Label = "P  ---  A";
			m_oNodeXLControl = m_oNodeXLWithAxesControl.NodeXLControl;

			GraphXL.Child = m_oNodeXLWithAxesControl;
		}

		public void SetDataManager(DataManager dataManager)
		{
			_dataManager = dataManager;
		}

		public void LoadGraphComponents(Overlap overlap, string dataType)
		{
			List<GraphComponent> components = overlap.Components[dataType];

			int edgeCount = 0;
			double width = this.ActualWidth - 30;
			double height = this.ActualHeight - 30;

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
					v.SetValue(ReservedMetadataKeys.PerVertexLabelFontSize, 18.0f);
				}
				else
				{
					v.SetValue(ReservedMetadataKeys.PerAlpha, 40.0f);
					v.SetValue(ReservedMetadataKeys.PerVertexLabelFontSize, 10.0f);
				}

				v.SetValue(ReservedMetadataKeys.PerVertexLabel, roi.Name);				
				v.SetValue(ReservedMetadataKeys.LockVertexLocation, true);
				v.Location = new System.Drawing.PointF((float)x + 15, (float)y + 15);

				_mapVtx[i] = new ROIVertex() { Vertex = v, Roi = roi };
			}

			int cmpSize = 0;
			GraphComponent cmp = null;
			for (var i = 0; i < components.Count; i++ )
			{
				if ((components[i].PValue < 0.05) && (components[i].Edges.Count > cmpSize))
				{
					cmp = components[i];
					cmpSize = cmp.Edges.Count;
				}
			}

			foreach (var edge in cmp.Edges)
			{
				ROIVertex v1 = _mapVtx[edge.V1];
				ROIVertex v2 = _mapVtx[edge.V2];

				IEdge e = ec.Add(v1.Vertex, v2.Vertex);

				double diff = edge.M2 - edge.M1;
				double pval = ( (double)edge.RightTailCount ) / ( (double)overlap.Permutations );

				string lbl = string.Format("{0} ({1})", diff.ToString("0.000"), pval.ToString("0.0000"));

				if (v1.Roi.Special && v2.Roi.Special)
				{
					edgeCount++;

					e.SetValue(ReservedMetadataKeys.PerEdgeLabel, lbl);
					e.SetValue(ReservedMetadataKeys.PerEdgeLabelFontSize, 16.0f);
					e.SetValue(ReservedMetadataKeys.PerEdgeWidth, 4.0f);
					e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 0, 0, 0));

					if (pval >= 0.05)
					{
						e.SetValue(ReservedMetadataKeys.PerAlpha, 40.0f);
						e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 0, 255, 0));
					}
					else
					{
						// Calc corr for this edge
						_dataManager.CorrelateEdgeAndMeasure(edge, dataType, "CogMem");
					}
				}
				else
				{
					e.SetValue(ReservedMetadataKeys.PerEdgeLabel, lbl);

					if (pval < 0.05)
					{
						e.SetValue(ReservedMetadataKeys.PerAlpha, 70.0f);
						e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 255, 0, 0));
					}
					else
					{
						e.SetValue(ReservedMetadataKeys.PerAlpha, 40.0f);
						e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 0, 0, 0));
					}
				}
			}

			m_oNodeXLControl.DrawGraph(true);

			_lblEdgeCount.Content = edgeCount.ToString("0");
		}

		private DataManager _dataManager;
		protected NodeXLWithAxesControl m_oNodeXLWithAxesControl;
		protected NodeXLControl m_oNodeXLControl;
		Dictionary<int, ROIVertex> _mapVtx = null;
	}

	class ROIVertex
	{
		public IVertex Vertex;
		public ROI Roi;
	}
}
