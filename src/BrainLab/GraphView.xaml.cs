﻿using System;
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

			// These are to power lists in the displays
			CmpNodes = new ObservableCollection<NodeResult>();
			CmpEdges = new ObservableCollection<EdgeResult>();
		}

		public void SetDataManager(DataManager dataManager)
		{
			_dataManager = dataManager;
		}

        public void Clear()
        {
            CmpNodes.Clear();
            CmpEdges.Clear();

            _graphAxXL.Clear();
            _graphCrXL.Clear();
            _graphSgXL.Clear();
        }

		public void LoadGraphComponents(Overlap overlap, string dataType, System.Windows.Media.Color componentColor)
		{
			DataType = dataType;

			List<GraphComponent> components = overlap.Components[dataType];
			List<ROIVertex> nodes = new List<ROIVertex>();
			List<GraphEdge> edges = new List<GraphEdge>();

			double xRange = _dataManager.XMax - _dataManager.XMin;
			double yRange = _dataManager.YMax - _dataManager.YMin;
			double zRange = _dataManager.ZMax - _dataManager.ZMin;

			for (var i = 0; i < 90; i++)
			{
				ROI roi = _dataManager.GetROI(i);

				double xFactor = (roi.X - _dataManager.XMin) / xRange;
				double yFactor = (roi.Y - _dataManager.YMin) / yRange;
				double zFactor = (roi.Z - _dataManager.ZMin) / zRange;

				nodes.Add( new ROIVertex() { Roi = roi, XF = xFactor, YF = yFactor, ZF = zFactor } );
			}

			int cmpSize = 0;
			GraphComponent cmp = null;
			for (var i = 0; i < components.Count; i++)
			{
				if (components[i].Edges.Count > cmpSize)
				{
					cmp = components[i];
					cmpSize = cmp.Edges.Count;
				}
			}

			if (cmp != null)
			{
                _graphCrXL.DoSomething(nodes, cmp.Edges, r => r.XF, xRange, r => r.ZF, zRange, false, componentColor, overlap);
				_graphSgXL.DoSomething(nodes, cmp.Edges, r => r.XF, xRange, r => r.YF, yRange, false, componentColor, overlap);
				_graphAxXL.DoSomething(nodes, cmp.Edges, r => r.YF, yRange, r => r.ZF, zRange, true, componentColor, overlap);
			}

            if (cmp != null)
            {
                CmpPValue = ((double)cmp.RightTailExtentCount) / ((double)overlap.Permutations);
                Dictionary<int, ROIVertex> cmpVerts = new Dictionary<int, ROIVertex>();

                foreach (var edge in cmp.Edges)
                {
                    ROIVertex v1 = nodes[edge.V1];
                    ROIVertex v2 = nodes[edge.V2];

                    if (!cmpVerts.ContainsKey(edge.V1))
                        cmpVerts[edge.V1] = v1;
                    if (!cmpVerts.ContainsKey(edge.V2))
                        cmpVerts[edge.V2] = v2;

                    double diff = edge.M2 - edge.M1;
                    double pval = ((double)edge.RightTailCount) / ((double)overlap.Permutations);

					CmpEdges.Add(new EdgeResult() { V1 = v1.Roi.Name, V2 = v2.Roi.Name, Diff = diff, TStat = edge.TStat, PVal = pval });
                }

                var itms = from v in cmpVerts.Values orderby v.Roi.Index select v;
                foreach (var vert in itms)
					CmpNodes.Add(new NodeResult() { Name = vert.Roi.Name, Id = vert.Roi.Index });
            }		
		}

		public void SaveGraphML(string folder, string dataType)
		{
			_graphAxXL.SaveGraphML(folder, dataType, "Sagital");
			_graphSgXL.SaveGraphML(folder, dataType, "Axial");
			_graphCrXL.SaveGraphML(folder, dataType, "Coronal");
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
		
		public double CmpPValue
		{
			get { return _cmpPValue; }
			set { _cmpPValue = value; NotifyPropertyChanged("CmpPValue"); }
		} private double _cmpPValue;

		public ObservableCollection<NodeResult> CmpNodes { get; private set; }
		public ObservableCollection<EdgeResult> CmpEdges { get; private set; }

		protected void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		private DataManager _dataManager;
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
}
