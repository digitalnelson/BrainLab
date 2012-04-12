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

			InterModalNodes = new ObservableCollection<string>();
			InterModalEdges = new ObservableCollection<string>();
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
			InterModalPValue = ((double)overlap.RightTailOverlapCount) / ((double)overlap.Permutations);

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

				InterModalNodes.Add(string.Format("{0} ({1})", nd.Roi.Name, nd.Roi.Index));
			}

            _graphCrXL.DoSomething(nodes, edges, r => r.XF, xRange, r => r.ZF, zRange, false);
            _graphSgXL.DoSomething(nodes, edges, r => r.XF, xRange, r => r.YF, yRange, false);
            _graphAxXL.DoSomething(nodes, edges, r => r.YF, yRange, r => r.ZF, zRange, true);
		}

		public void SaveGraphML(string folder, string dataType)
		{
			//_graphAxXL.SaveGraphML(folder, dataType, "Sagital");
			//_graphSgXL.SaveGraphML(folder, dataType, "Axial");
			//_graphCrXL.SaveGraphML(folder, dataType, "Coronal");
		}
		
		public event PropertyChangedEventHandler PropertyChanged;

		public double InterModalPValue
		{
			get { return _interModalPValue; }
			set { _interModalPValue = value; NotifyPropertyChanged("InterModalPValue"); }
		} private double _interModalPValue;
		
		public ObservableCollection<string> InterModalNodes { get; private set; }
		public ObservableCollection<string> InterModalEdges { get; private set; }

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
