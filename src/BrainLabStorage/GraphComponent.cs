using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BrainLabStorage
{
	public class Overlap
	{
		public List<int> Vertices;
		public Dictionary<string, List<GraphComponent>> Components;
        public Dictionary<string, Color> Colors = new Dictionary<string,Color>();

		public int Permutations;
		public int RightTailOverlapCount;
	}

	public class GraphComponent
	{
		public int Id;
		public List<GraphEdge> Edges = new List<GraphEdge>();
        public string DataType;

		public int Permutations;
		public int RightTailExtentCount;
		//public double VertexCount;
	}

	public class GraphEdge
	{
		public int V1;
		public int V2;

		public double Value;
		public double M1;
		public double M2;
		public double Var1;
		public double Var2;
		public double TStat;

		public int RightTailCount;

        public Color Color;

		public GraphEdge(int v1, int v2)
		{
			V1 = v1;
			V2 = v2;
            Value = 0; M1 = 0; M2 = 0; Var1 = 0; Var2 = 0; TStat = 0; RightTailCount = 0; Color = Colors.Black;
		}
	}
}
