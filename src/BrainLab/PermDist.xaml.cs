using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.DataVisualization.Charting;
using ShoNS.Array;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BrainLabStorage;

namespace BrainLab.Studio
{
	public partial class PermDist : UserControl
	{
		private string[] _modalities = new string[] { "DTI", "fMRI" };
		
		private List<Subject> _subjects = null;
		private List<int> _actualLabels = null;
		private List<int> _indexLabels = null;
		private pair[] _luAdj = new pair[4005];
		
		private Dictionary<string, double[,]> _flatByMod = new Dictionary<string, double[,]>();
		private Dictionary<string, List<double>> _tDistByMod = new Dictionary<string, List<double>>();
		private Dictionary<string, List<int>> _netCmpByMod = new Dictionary<string, List<int>>();

		public PermDist()
		{
			InitializeComponent();

			// Fill our adj lu table
			int luIdx = 0;
			for (var i = 0; i < 90; i++)
			{
				for (var j = 0; j < 90; j++)
				{
					if (j > i)
					{
						_luAdj[luIdx].i = i;
						_luAdj[luIdx].j = j;

						luIdx++;
					}
				}
			}
		}

		public pair LookupIdx(int idx)
		{
			return _luAdj[idx];
		}

		public double[,] GetSubjects(string dimension)
		{
			return _flatByMod[dimension];
		}

		public int[] GetActualLabels()
		{
			return _actualLabels.ToArray();
		}

		public void ProcessData(List<Subject> subjects)
		{
			_subjects = subjects;
			_indexLabels = new List<int>();

			List<int> ctls = new List<int>();
			List<int> pros = new List<int>();

			for(int i = 0; i < _subjects.Count; i++)
			{
				var s = _subjects[i];

				if(s.GroupId == "1")
					pros.Add(i);
				else
					ctls.Add(i);
				_indexLabels.Add(i);

				foreach (string key in s.Graphs.Keys)
				{
					if(!_flatByMod.ContainsKey(key))
					{
						_flatByMod[key] = new double[4005,58];
						_tDistByMod[key] = new List<double>();
						_netCmpByMod[key] = new List<int>();
					}
				}
			}

			_actualLabels = new List<int>();
			_actualLabels.AddRange(pros);
			_actualLabels.AddRange(ctls);

			foreach(string mod in _flatByMod.Keys)
			{
				var edges = _flatByMod[mod];

			//	Parallel.For(0, _subjects.Count, s =>
			//	//for (var s = 0; s < _subjects.Count; s++)
			//	{
			//		var idx = 0;
			//		var adjData = _subjects[s].Graphs[mod].Adj;
			//		for (int i = 0; i < 90; i++)  // Row
			//		{
			//			for (int j = 0; j < 90; j++)  // Col
			//			{
			//				if (j > i)
			//				{
			//					edges[idx, s] = Math.Abs(adjData.At(i, j));
			//					idx++;

			//					//Console.WriteLine(string.Format("i:{0} j:{1}", i, j));
			//				}
			//			}
			//		}

			//		//Console.WriteLine("edges: " + idx);
			//	});
			}
		}

		public double TStat(int edgeIdx, int[] subIdxs, double[,] edges)
		{
			double v1 = 0;
			double v2 = 0;
			double t1 = 0;
			double t2 = 0;
			ulong j1 = 0;
			ulong j2 = 0;
			double m1 = 0;
			double m2 = 0;
			double s1 = 0;
			double s2 = 0;

			//List<double> vals1 = new List<double>();
			//List<double> vals2 = new List<double>();

			for (var idx = 0; idx < subIdxs.Length; idx++)
			{
				double edgeVal = edges[edgeIdx, subIdxs[idx]];

				if (Double.IsNaN(edgeVal))
					return 0;

				if (idx < 29)
				{
					//Console.WriteLine("GroupA {0}", edgeVal);

					if (idx == 0)
					{
						j1++;
						t1 = edgeVal;
					}
					else
					{
						j1++;
						double xi = edgeVal;
						t1 += xi;
						double diff = (j1 * xi) - t1;
						v1 += (diff * diff) / (j1 * (j1 - 1));
					}

					m1 += edgeVal;
					//vals1.Add(edgeVal);
				}
				else
				{
					//Console.WriteLine("GroupB {0}", edgeVal);

					if (idx == 29)
					{
						j2++;
						t2 = edgeVal;
					}
					else
					{
						j2++;
						double xi = edgeVal;
						t2 += xi;
						double diff = (j2 * xi) - t2;
						v2 += (diff * diff) / (j2 * (j2 - 1));
					}

					m2 += edgeVal;
					//vals2.Add(edgeVal);
				}
			}

			double vc1 = v1 / (j1 - 1);
			m1 /= 29;
			s1 = Math.Sqrt(vc1);

			double vc2 = v2 / (j2 - 1);
			m2 /= 29;
			s2 = Math.Sqrt(vc2);

			double tstat = (m1 - m2) / Math.Sqrt((Math.Pow(s1, 2) / 29) + (Math.Pow(s2, 2) / 29));

			if (Double.IsNaN(tstat))
				return 1;
			else
				return tstat;
		}

		public DistroSummary GetGraphDistroSummary(int[] subjectIdxs, double[,] edges, double threshold, bool includeFullSummary = false)
		{
			//GraphStats gs = new GraphStats();
			DistroSummary ds = new DistroSummary();

			//float[,] arr = new float[4005, 58];
			//for (var i = 0; i < 4005; i++)
			//	for(var j = 0; j < 58; j++) 
			//		arr[i, j] = (float)edges[i, j];
			//float[] tstats = new float[4005];

			//Stopwatch sw = new Stopwatch();

			//try
			//{
			//	sw.Start();

			//	square_array(arr, 4005, 58, tstats);

			//	sw.Stop();
			//	long dur = sw.ElapsedMilliseconds;
			//}
			//catch (Exception ex)
			//{
			//	int i = 0;
			//}
			
			//for (var edgeIdx = 0; edgeIdx < 4005; edgeIdx++)
			//Parallel.For(0, 4005, edgeIdx =>
			//{
			//	var tstat = Math.Abs(TStat(edgeIdx, subjectIdxs, edges));
			//	if (tstat != Double.NaN && tstat > threshold)
			//	{
			//		pair p = _luAdj[edgeIdx];

			//		lock (gs)
			//		{
			//			gs.AddEdge(p.i, p.j);
			//		}
			//	}

			//	ds.MaxEdgeTStat = tstat > ds.MaxEdgeTStat ? tstat : ds.MaxEdgeTStat;
			//});

			//ds.LargestComponentTopoExtent = gs.GetLargestComponentSize();

			//if (includeFullSummary)
			//{
			//	List<int> nodes = gs.GetComponentList();
			//	Dictionary<int, List<int>> cmpNodes = new Dictionary<int, List<int>>();

			//	for (int i = 0; i < nodes.Count; i++)
			//	{
			//		if (cmpNodes.ContainsKey(nodes[i]))
			//		{
			//			cmpNodes[nodes[i]].Add(i);
			//		}
			//		else
			//		{
			//			cmpNodes[nodes[i]] = new List<int>();
			//			cmpNodes[nodes[i]].Add(i);
			//		}
			//	}

			//	ds.Nodes = cmpNodes;
			//}

			return ds;
		}

		public void GenerateDistribution(string dimension, double threshold, int permutations)
		{
			var edges = _flatByMod[dimension];
			var tdist = _tDistByMod[dimension];
			var netcmp = _netCmpByMod[dimension];

			for (var perm = 0; perm < permutations; perm++)
			//Parallel.For(0, permutations, perm =>
			{
				//var randomIndxs = IntArray.From(ShoNS.Stats.Utils.RandPermute(DoubleArray.From(_indexLabels))).ToFlatSystemArray();

				var randomIndxs = new int[58];

				DistroSummary ds = GetGraphDistroSummary(randomIndxs, edges, threshold);

				// Add component size
				netcmp.Add(ds.LargestComponentTopoExtent);

				// Add max Tstat
				if (!Double.IsNaN(ds.MaxEdgeTStat))
					tdist.Add(ds.MaxEdgeTStat);
			}//);

			tdist.Sort();
			netcmp.Sort();
		}

		public double GetPVal(string dimension, double tstat)
		{
			List<double> tdist = _tDistByMod[dimension];
			int count = tdist.Where(t => t > tstat).Count();

			return (double)count / (double)tdist.Count;
		}

		public double[] ProcessEdges(string dimension)
		{
			var edges = _flatByMod[dimension];
			var idxs = _actualLabels.ToArray();

			var adj = new double[4005];

			//for (var edgeIdx = 0; edgeIdx < 4005; edgeIdx++)
			Parallel.For(0, 4005, edgeIdx =>
			{
				var tstat = Math.Abs(TStat(edgeIdx, idxs, edges));
				if (tstat != Double.NaN)
				{
					var pval = GetPVal(dimension, tstat);

					if (pval < 0.05)
						adj[edgeIdx] = pval;
					else
						adj[edgeIdx] = 1;
				}
			});

			return adj;
		}

		public DistroSummary ProcessDimension(string dimension, double threshold)
		{
			// Pull flat data and distros
			var edges = _flatByMod[dimension];
			var tdist = _tDistByMod[dimension];
			var netcmp = _netCmpByMod[dimension];

			return GetGraphDistroSummary(_actualLabels.ToArray(), edges, threshold, true);
		}
	}

	public class DistroSummary
	{
		public double MaxEdgeTStat = 0;
		public int LargestComponentTopoExtent = 0;
		public Dictionary<int, List<int>> Nodes;
	}

	public struct pair
	{
		public int i;
		public int j;
	}
}
