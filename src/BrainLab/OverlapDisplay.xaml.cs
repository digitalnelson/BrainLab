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
	/// <summary>
	/// Interaction logic for GraphDisplay.xaml
	/// </summary>
	public partial class OverlapDisplay : UserControl
	{
		public OverlapDisplay()
		{
			InitializeComponent();

            Clear();
		}

        public void Clear()
        {
            _oNodeXLControl = new NodeXLControl();
            _oNodeXLControl.Graph = new Smrf.NodeXL.Core.Graph(GraphDirectedness.Undirected);

            _graphXL.Child = _oNodeXLControl;
        }

		public void DoSomething(List<ROIVertex> nodes, List<GraphEdge> edges, SelectDim horiz, double hRange, SelectDim vert, double vRange, bool flipX, Overlap overlap)
		{
			Color componentColor = Color.FromArgb(255, 255, 0, 0);

			double width = this.ActualWidth - 30;
			double height = this.ActualHeight - 30;

            double hCalc = 0; double hSize = 0; double hOffset = 0;
            double vCalc = 0; double vSize = 0; double vOffset = 0;

            hCalc = (height * hRange) / vRange;
            vCalc = (width * vRange) / hRange;

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
			foreach (var node in nodes)
			{
				double hf = horiz(node);
				double vf = vert(node);

                double xCoord = (hf * hSize) + hOffset;
                if (flipX)
                    xCoord = width - xCoord;

				double yCoordNative = (vf * vSize) + vOffset;
				double yCoord = height - yCoordNative;

				IVertex vertex = vc.Add();
				vertex.Name = node.Roi.Index.ToString();

                if (node.Highlight)
                {
					vertex.SetValue(ReservedMetadataKeys.PerColor, componentColor);
                    vertex.SetValue(ReservedMetadataKeys.PerVertexRadius, 8.0f);

					vertex.SetValue("r", componentColor.R.ToString());
					vertex.SetValue("g", componentColor.G.ToString());
					vertex.SetValue("b", componentColor.B.ToString());
                }
                else
                {
                    vertex.SetValue(ReservedMetadataKeys.PerVertexRadius, 8.0f);
                    vertex.SetValue(ReservedMetadataKeys.PerAlpha, 75.0f);
                }

				vertex.SetValue(ReservedMetadataKeys.LockVertexLocation, true);
				vertex.Location = new System.Drawing.PointF((float)xCoord + 15, (float)yCoord + 15);

				vertex.SetValue("x", xCoord.ToString());
				vertex.SetValue("y", yCoordNative.ToString());
				vertex.SetValue("name", node.Roi.Name);

				verts.Add(vertex);
			}

			foreach (var edge in edges)
			{
				IVertex v1 = verts[edge.V1];
				IVertex v2 = verts[edge.V2];
				v1.SetValue(ReservedMetadataKeys.PerColor, componentColor);
				v1.SetValue(ReservedMetadataKeys.PerAlpha, 100.0f);
				v2.SetValue(ReservedMetadataKeys.PerColor, componentColor);
				v2.SetValue(ReservedMetadataKeys.PerAlpha, 100.0f);

				v1.SetValue("r", componentColor.R.ToString());
				v1.SetValue("g", componentColor.G.ToString());
				v1.SetValue("b", componentColor.B.ToString());
				v2.SetValue("r", componentColor.R.ToString());
				v2.SetValue("g", componentColor.G.ToString());
				v2.SetValue("b", componentColor.B.ToString());

				double diff = edge.M2 - edge.M1;
				double pval = ((double)edge.RightTailCount) / ((double)overlap.Permutations);

				IEdge e = ec.Add(v1, v2);
				e.SetValue("diff", diff);
				e.SetValue("pval", pval);
				e.SetValue(ReservedMetadataKeys.PerEdgeWidth, 2.0f);
				e.SetValue(ReservedMetadataKeys.PerColor, componentColor);
			}

			_oNodeXLControl.DrawGraph(true);
		}

		public void SaveGraphML(string folder, string dataType, string view)
		{
			string fileName = "graph_" + dataType + "_" + view + ".graphml";

			_oNodeXLControl.Graph.SetValue(ReservedMetadataKeys.AllEdgeMetadataKeys, new string[] { "diff", "pval" });
			_oNodeXLControl.Graph.SetValue(ReservedMetadataKeys.AllVertexMetadataKeys, new string[] {"x", "y", "name", "r", "g", "b" });

			Smrf.NodeXL.Adapters.GraphMLGraphAdapter ga = new Smrf.NodeXL.Adapters.GraphMLGraphAdapter();
			ga.SaveGraph(_oNodeXLControl.Graph, System.IO.Path.Combine(folder, fileName));
		}

		private NodeXLControl _oNodeXLControl;
	}
}
