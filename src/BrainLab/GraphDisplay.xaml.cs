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
using BrainLabStorage;
using Smrf.NodeXL.Core;
using Smrf.NodeXL.Visualization.Wpf;

namespace BrainLab.Studio
{
	public delegate double SelectDim(ROIVertex v);

	/// <summary>
	/// Interaction logic for GraphDisplay.xaml
	/// </summary>
	public partial class GraphDisplay : UserControl
	{
		public GraphDisplay()
		{
			InitializeComponent();
            Clear();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			Clear();
			Draw();
		}

		private List<ROIVertex> _nodes;
		private List<GraphEdge> _edges; 
		private SelectDim _horiz;
		private double _hRange;
		private SelectDim _vert; 
		private double _vRange;
		private bool _flipX;
		private System.Windows.Media.Color _componentColor;
		private Overlap _overlap;

		public void SetData(List<ROIVertex> nodes, List<GraphEdge> edges, SelectDim horiz, double hRange, SelectDim vert, 
			double vRange, bool flipX, System.Windows.Media.Color componentColor, Overlap overlap)
		{
			_nodes = nodes;
			_edges = edges;
			_horiz = horiz;
			_hRange = hRange;
			_vert = vert;
			_vRange = vRange;
			_flipX = flipX;
			_componentColor = componentColor;
			_overlap = overlap;
		}

        public void Clear()
        {
            _oNodeXLControl = new NodeXLControl();
            _oNodeXLControl.Graph = new Smrf.NodeXL.Core.Graph(GraphDirectedness.Undirected);

            _graphXL.Child = _oNodeXLControl;
        }

		public void Draw()
		{
			if (_nodes != null)
			{
				double width = this.ActualWidth - 30;
				double height = this.ActualHeight - 30;

				double hCalc = 0; double hSize = 0; double hOffset = 0;
				double vCalc = 0; double vSize = 0; double vOffset = 0;

				hCalc = (height * _hRange) / _vRange;
				vCalc = (width * _vRange) / _hRange;

				double hDiff = width - hCalc;
				double vDiff = height - vCalc;

				if (hDiff > 0 && hDiff > vDiff)
				{
					hSize = hCalc;
					hOffset = hDiff / 2;
					vSize = height;
				}
				else
				{
					vSize = vCalc;
					vOffset = vDiff / 2;
					hSize = width;
				}

				IGraph g = _oNodeXLControl.Graph;
				IVertexCollection vc = g.Vertices;
				IEdgeCollection ec = g.Edges;

				List<IVertex> verts = new List<IVertex>();
				foreach (var node in _nodes)
				{
					double hf = _horiz(node);
					double vf = _vert(node);

					double xCoord = (hf * hSize) + hOffset;
					if (_flipX)
						xCoord = width - xCoord;

					double yCoordNative = (vf * vSize) + vOffset;
					double yCoord = height - yCoordNative;

					IVertex vertex = vc.Add();

					vertex.Name = node.Roi.Index.ToString();

					vertex.SetValue(ReservedMetadataKeys.PerVertexRadius, 8.0f);
					vertex.SetValue(ReservedMetadataKeys.PerAlpha, 75.0f);
					vertex.SetValue(ReservedMetadataKeys.LockVertexLocation, true);

					vertex.Location = new System.Drawing.PointF((float)xCoord + 15, (float)yCoord + 15);

					vertex.SetValue("x", xCoord.ToString());
					vertex.SetValue("y", yCoordNative.ToString());
					vertex.SetValue("name", node.Roi.Name);

					verts.Add(vertex);
				}

				foreach (var edge in _edges)
				{
					IVertex v1 = verts[edge.V1];
					IVertex v2 = verts[edge.V2];
					v1.SetValue(ReservedMetadataKeys.PerColor, _componentColor);
					v1.SetValue(ReservedMetadataKeys.PerAlpha, 100.0f);
					v2.SetValue(ReservedMetadataKeys.PerColor, _componentColor);
					v2.SetValue(ReservedMetadataKeys.PerAlpha, 100.0f);

					v1.SetValue("r", _componentColor.R.ToString());
					v1.SetValue("g", _componentColor.G.ToString());
					v1.SetValue("b", _componentColor.B.ToString());
					v2.SetValue("r", _componentColor.R.ToString());
					v2.SetValue("g", _componentColor.G.ToString());
					v2.SetValue("b", _componentColor.B.ToString());

					double diff = edge.M2 - edge.M1;
					double pval = ((double)edge.RightTailCount) / ((double)_overlap.Permutations);

					IEdge e = ec.Add(v1, v2);
					e.SetValue("diff", diff);
					e.SetValue("pval", pval);
					e.SetValue(ReservedMetadataKeys.PerEdgeWidth, 2.0f);
					e.SetValue(ReservedMetadataKeys.PerColor, _componentColor);
				}

				_oNodeXLControl.DrawGraph(true);
			}
		}

		public void SaveReport(StringBuilder htmlSink, string folderPath, string dataType, string view, int width, int height)
		{
			if (_oNodeXLControl.Graph != null)
			{
				// TODO: May want to tack on a guid so things don't overwrite
				string fileName = "NBSm_" + dataType + "_" + view + ".png";

				htmlSink.AppendFormat("<img src=\"{0}\" />\n", fileName);

				var bmp = _oNodeXLControl.CopyGraphToBitmap(width, height);
				bmp.Save(System.IO.Path.Combine(folderPath, fileName), System.Drawing.Imaging.ImageFormat.Png);

				//Smrf.NodeXL.Visualization.Wpf.
				//_plot.SaveBitmap(System.IO.Path.Combine(folderPath, fileName));
			}
		}

		public void SaveGraphML(string folder, string dataType, string view)
		{
			string fileName = "graph_" + dataType + "_" + view + ".graphml";

			_oNodeXLControl.Graph.SetValue(ReservedMetadataKeys.AllEdgeMetadataKeys, new string[] { "diff", "pval" });
			_oNodeXLControl.Graph.SetValue(ReservedMetadataKeys.AllVertexMetadataKeys, new string[] { "x", "y", "name", "r", "g", "b" });

			Smrf.NodeXL.Adapters.GraphMLGraphAdapter ga = new Smrf.NodeXL.Adapters.GraphMLGraphAdapter();
			ga.SaveGraph(_oNodeXLControl.Graph, System.IO.Path.Combine(folder, fileName));
		}

		//private NodeXLWithAxesControl m_oNodeXLWithAxesControl;
		private NodeXLControl _oNodeXLControl;
	}
}
