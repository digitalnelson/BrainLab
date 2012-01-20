using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainLabStorage
{
	public class Overlap
	{
		public double PValue;
		public List<int> Vertices;

		public Dictionary<string, List<GraphComponent>> Components;
	}

	public class GraphComponent
	{
		public int Id;
		public double PValue = 1;
		public double VertexCount;
		public List<GraphEdge> Edges = new List<GraphEdge>();
	}

	public struct GraphEdge
	{
		public int V1;
		public int V2;

		public double Value;
		public double M1;
		public double M2;
		public double Var1;
		public double Var2;
		public double TStat;
		public double PValue;

		public GraphEdge(int v1, int v2)
		{
			V1 = v1;
			V2 = v2;
			Value = 0; M1 = 0; M2 = 0; Var1 = 0; Var2 = 0; TStat = 0; PValue = 0;
		}
	}
}
