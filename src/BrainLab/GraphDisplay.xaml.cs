using System;
using System.Collections.Generic;
using System.IO;
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

namespace BrainLab
{
	public delegate ROIDim SelectDim(ROIVertex v);

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

		public void SetData(List<ROIVertex> nodes, List<GraphEdge> edges, SelectDim horiz, double hRange, double hMin, double hMax, SelectDim vert, 
			double vRange, double vMin, double vMax, bool flipX, System.Windows.Media.Color componentColor, Overlap overlap)
		{
			_nodes = nodes;
			_edges = edges;
			_horiz = horiz;
			_hRange = hRange;
            _hMin = hMin;
            _hMax = hMax;
			_vert = vert;
			_vRange = vRange;
            _vMin = vMin;
            _vMax = vMax;
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

        class MyVert
        {
            public double X;
            public double Y;
            public Color Color;
            public string StrokeWidth;
        }

        class MyEdge
        {
            public int V1;
            public int V2;
            public Color Color;
        }

        private List<MyVert> myVerts = new List<MyVert>();
        private List<MyEdge> myEdges = new List<MyEdge>();

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

                myVerts = new List<MyVert>();
				List<IVertex> verts = new List<IVertex>();
				foreach (var node in _nodes)
				{
                    ROIDim hd = _horiz(node);
                    ROIDim vd = _vert(node);

					double hf = hd.Factor;
					double vf = vd.Factor;

                    MyVert mv = new MyVert();
                    if (_flipX)
                        mv = new MyVert() { X = _hMax - hd.Raw, Y = _vMax - vd.Raw };
                    else
                        mv = new MyVert() { X = hd.Raw, Y = _vMax - vd.Raw };
                    myVerts.Add(mv);

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

                myEdges = new List<MyEdge>();
				foreach (var edge in _edges)
				{
                    MyVert mv1 = myVerts[edge.V1];
                    MyVert mv2 = myVerts[edge.V2];
                    mv1.Color = _componentColor;
                    mv1.StrokeWidth = "1";
                    mv2.Color = _componentColor;
                    mv2.StrokeWidth = "1";

                    MyEdge me = new MyEdge() { V1 = edge.V1, V2 = edge.V2, Color = edge.Color };
                    myEdges.Add(me);

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

		public void Save(StringBuilder htmlSink, string folderPath, string dataType, string view, int width, int height)
		{
			if (myEdges.Count > 0)
			{
                // TODO: Get rid of stringbuilder and use XmlBuilder instead
                StringBuilder sb = new StringBuilder();

                var sbCmpNodes = new StringBuilder();
                var sbCmpEdges = new StringBuilder();
                var sbNodes = new StringBuilder();

                foreach (var n in myVerts)
                {
                    if (String.IsNullOrEmpty(n.StrokeWidth))
                    {
                        sbNodes.AppendFormat("\t<circle fill=\"#999999\" fill-opacity=\"0.5\" r=\"3.0\" class=\"0\" cx=\"{0}\" cy=\"{1}\"/>\n",
                            n.X, n.Y);
                    }
                    else
                    {
                        sbCmpNodes.AppendFormat("\t<circle fill=\"{2}\" stroke=\"black\" stroke-width=\"0.5\" fill-opacity=\"0.75\" r=\"3.0\" class=\"0\" cx=\"{0}\" cy=\"{1}\"/>\n",
                            n.X, n.Y,
                            n.Color.ToString().Substring(3));
                    }
                }

                foreach (var e in myEdges)
                {
                    var v1 = myVerts[e.V1];
                    var v2 = myVerts[e.V2];

                    sbCmpEdges.AppendFormat("\t<path fill=\"none\" stroke-width=\"0.7\" d=\"M {0},{1} L {2},{3}\" stroke-opacity=\"1.0\" stroke=\"#{4}\"/>\n",
                            v1.X, v1.Y, v2.X, v2.Y, e.Color.ToString().Substring(3));    
                }
                
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\" >");

                if (_flipX)
                    sb.AppendFormat("<svg contentScriptType=\"text/ecmascript\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" zoomAndPan=\"magnify\" contentStyleType=\"text/css\" viewBox=\"{0} {1} {2} {3}\" preserveAspectRatio=\"xMidYMid meet\" xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">",
                        -10, -10, _hMax, _vMax);  // TODO: Super hack - figure out actual ranges
                else
                    sb.AppendFormat("<svg contentScriptType=\"text/ecmascript\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" zoomAndPan=\"magnify\" contentStyleType=\"text/css\" viewBox=\"{0} {1} {2} {3}\" preserveAspectRatio=\"xMidYMid meet\" xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">",
                        10, -10, _hMax, _vMax);  // TODO: Super hack - figure out actual ranges
                sb.Append("<g id=\"backNodes\">");
                sb.Append(sbNodes.ToString());
                sb.Append("</g>");
                sb.Append("<g id=\"cmpEdges\">");
                sb.Append(sbCmpEdges.ToString());
                sb.Append("</g>");
                sb.Append("<g id=\"cmpNodes\">");
                sb.Append(sbCmpNodes.ToString());
                sb.Append("</g>");
                sb.Append("</svg>");

                // TODO: May want to tack on a guid so things don't overwrite
				string fileName = "NBSm_" + dataType + "_" + view + ".svg";

				htmlSink.AppendFormat("<img src=\"{0}\" width=\"{1}px\" />\n", fileName, width);

                using(StreamWriter sw = new StreamWriter(System.IO.Path.Combine(folderPath, fileName)))
                {
                    sw.WriteLine(sb.ToString());
                }
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

        private List<ROIVertex> _nodes;
        private List<GraphEdge> _edges;
        private SelectDim _horiz;
        private double _hRange;
        private double _hMin;
        private double _hMax;
        private SelectDim _vert;
        private double _vRange;
        private double _vMin;
        private double _vMax;
        private bool _flipX;
        private System.Windows.Media.Color _componentColor;
        private Overlap _overlap;

		private NodeXLControl _oNodeXLControl;
	}
}
