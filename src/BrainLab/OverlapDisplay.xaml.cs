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

			_oNodeXLControl = new NodeXLControl();
			_graphXL.Child = _oNodeXLControl;
		}

		public void DoSomething(List<ROIVertex> nodes, List<GraphEdge> edges, SelectDim horiz, double hRange, SelectDim vert, double vRange, bool flipX)
		{
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
                
                double yCoord = (vf * vSize) + vOffset;
				yCoord = height - yCoord;

				IVertex vertex = vc.Add();

				//if (!node.Roi.Special)
					//vertex.SetValue(ReservedMetadataKeys.PerAlpha, 40.0f);

				//vertex.SetValue(ReservedMetadataKeys.PerVertexLabel, node.Roi.Name);
                if (node.Roi.Special)
                {
                    vertex.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 255, 0, 0));
                    vertex.SetValue(ReservedMetadataKeys.PerVertexRadius, 8.0f);
                }
                else
                {
                    vertex.SetValue(ReservedMetadataKeys.PerVertexRadius, 8.0f);
                    vertex.SetValue(ReservedMetadataKeys.PerAlpha, 75.0f);
                }

				vertex.SetValue(ReservedMetadataKeys.LockVertexLocation, true);

				vertex.Location = new System.Drawing.PointF((float)xCoord + 15, (float)yCoord + 15);

				verts.Add(vertex);
			}

			//Dictionary<int, ROIVertex> cmpVerts = new Dictionary<int, ROIVertex>();

			foreach (var edge in edges)
			{
			//	ROIVertex v1 = nodes[edge.V1];
			//	ROIVertex v2 = nodes[edge.V2];

			//	if (!cmpVerts.ContainsKey(edge.V1))
			//		cmpVerts[edge.V1] = v1;
			//	if (!cmpVerts.ContainsKey(edge.V2))
			//		cmpVerts[edge.V2] = v2;

			//	IEdge e = ec.Add(v1.Vertex, v2.Vertex);

			//	double diff = edge.M2 - edge.M1;
			//	//double pval = ((double)edge.RightTailCount) / ((double)overlap.Permutations);

			//	string lbl = string.Format("{0} ({1})", diff.ToString("0.000"), pval.ToString("0.0000"));

			//	if (v1.Roi.Special && v2.Roi.Special)
			//	{
			//		//e.SetValue(ReservedMetadataKeys.PerEdgeLabel, lbl);
			//		//e.SetValue(ReservedMetadataKeys.PerEdgeLabelFontSize, 16.0f);
			//		e.SetValue(ReservedMetadataKeys.PerEdgeWidth, 4.0f);
			//		e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 0, 0, 0));

			//		//InterModalEdges.Add(string.Format("{0} - {1} [{2} ({3} {4})] ", v1.Roi.Name, v2.Roi.Name, diff.ToString("0.000"), edge.TStat.ToString("0.00"), pval.ToString("0.0000")));
			//	}
			//	else
			//	{
			//		//e.SetValue(ReservedMetadataKeys.PerEdgeLabel, lbl);
			//		e.SetValue(ReservedMetadataKeys.PerAlpha, 70.0f);
			//		e.SetValue(ReservedMetadataKeys.PerColor, Color.FromArgb(255, 0, 0, 0));
			//	}
			}

			_oNodeXLControl.DrawGraph(true);
		}

		//private NodeXLWithAxesControl m_oNodeXLWithAxesControl;
		private NodeXLControl _oNodeXLControl;
	}
}
