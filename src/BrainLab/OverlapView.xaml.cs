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
	public partial class OverlapView : UserControl, INotifyPropertyChanged
	{
		public OverlapView()
		{
			InitializeComponent();

			_ctl.DataContext = this;

			InterModalNodes = new ObservableCollection<NodeResult>();
			InterModalEdges = new ObservableCollection<EdgeResult>();
		}

		public void SetDataManager(DataManager dataManager)
		{
			_dataManager = dataManager;
		}

        public void Clear()
        {
            InterModalNodes.Clear();
            InterModalEdges.Clear();

            _graphAxXL.Clear();
            _graphCrXL.Clear();
            _graphSgXL.Clear();
        }

		public void LoadGraphComponents(Overlap overlap)
		{
			double interModalPValue = ((double)overlap.RightTailOverlapCount) / ((double)overlap.Permutations);
			InterModalPValue = interModalPValue.ToString("0.000");

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

				nodes.Add(new ROIVertex() { Roi = roi, XF = xFactor, YF = yFactor, ZF = zFactor });
			}

			foreach (var node in overlap.Vertices)
			{
				var nd = nodes[node];
				nd.Highlight = true;

				InterModalNodes.Add(new NodeResult(){ Id = nd.Roi.Index, Name = nd.Roi.Name});
			}

			foreach (var cmpList in overlap.Components.Values)
			{
				int cmpSize = 0;
				GraphComponent cmp = null;
				for (var i = 0; i < cmpList.Count; i++)
				{
					if (cmpList[i].Edges.Count > cmpSize)
					{
						cmp = cmpList[i];
						cmpSize = cmp.Edges.Count;
					}
				}

				foreach (var edge in cmp.Edges)
				{
					if (nodes[edge.V1].Highlight && nodes[edge.V2].Highlight)
					{
						edges.Add(edge);

						ROIVertex v1 = nodes[edge.V1];
						ROIVertex v2 = nodes[edge.V2];

						double diff = edge.M2 - edge.M1;
						double pval = ((double)edge.RightTailCount) / ((double)overlap.Permutations);

						InterModalEdges.Add(new EdgeResult() { V1 = v1.Roi.Name, V2 = v2.Roi.Name, Diff = diff, TStat = edge.TStat, PVal = pval });
					}
				}
			}

			_graphCrXL.DoSomething(nodes, edges, r => r.XF, xRange, r => r.ZF, zRange, false, overlap);
			_graphSgXL.DoSomething(nodes, edges, r => r.XF, xRange, r => r.YF, yRange, false, overlap);
			_graphAxXL.DoSomething(nodes, edges, r => r.YF, yRange, r => r.ZF, zRange, true, overlap);
		}

		public void SaveGraphML(string folder, string dataType)
		{
			_graphAxXL.SaveGraphML(folder, dataType, "Sagital");
			_graphSgXL.SaveGraphML(folder, dataType, "Axial");
			_graphCrXL.SaveGraphML(folder, dataType, "Coronal");
		}
		
		public event PropertyChangedEventHandler PropertyChanged;

		public string InterModalPValue
		{
			get { return _interModalPValue; }
			set { _interModalPValue = value; NotifyPropertyChanged("InterModalPValue"); }
		} private string _interModalPValue;

		public ObservableCollection<NodeResult> InterModalNodes { get; private set; }
		public ObservableCollection<EdgeResult> InterModalEdges { get; private set; }

		protected void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		private DataManager _dataManager;
	}
}
