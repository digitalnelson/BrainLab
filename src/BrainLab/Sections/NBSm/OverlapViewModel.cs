using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BrainLabStorage;
using Caliburn.Micro;

namespace BrainLab.Sections.NBSm
{
	public class OverlapViewModel : Screen
	{
		public OverlapViewModel()
		{
			InterModalNodes = new BindableCollection<NodeResult>();
			InterModalEdges = new BindableCollection<EdgeResult>();
		}

		public void SetDataManager(DataManager dataManager)
		{
			_dataManager = dataManager;
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
				var nd = nodes[node.Id];
				nd.Highlight = true;

				InterModalNodes.Add(new NodeResult(){ Id = nd.Roi.Index, Name = nd.Roi.Name, Count = node.RandomOverlapCount.ToString("0")});
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

				for (var i=0; i<cmp.Edges.Count; i++)
				{
                    var edge = cmp.Edges[i];

					if (nodes[edge.V1].Highlight && nodes[edge.V2].Highlight)
					{
                        edge.Color = overlap.Colors[cmp.DataType];
						edges.Add(edge);

						ROIVertex v1 = nodes[edge.V1];
						ROIVertex v2 = nodes[edge.V2];

						double diff = edge.M2 - edge.M1;
						double pval = ((double)edge.RightTailCount) / ((double)overlap.Permutations);

						InterModalEdges.Add(new EdgeResult() { V1 = v1.Roi.Name, V2 = v2.Roi.Name, Diff = diff, TStat = edge.TStat, PVal = pval });
					}
				}
			}

			//_graphCrXL.SetData(nodes, edges,
			//		r => new ROIDim() { Raw = r.Roi.X, Factor = r.XF }, xRange, _dataManager.XMin, _dataManager.XMax,
			//		r => new ROIDim() { Raw = r.Roi.Z, Factor = r.ZF }, zRange, _dataManager.ZMin, _dataManager.ZMax,
			//		false, ComponentColor, overlap);

			//_graphSgXL.SetData(nodes, edges,
			//	r => new ROIDim() { Raw = r.Roi.X, Factor = r.XF }, xRange, _dataManager.XMin, _dataManager.XMax,
			//	r => new ROIDim() { Raw = r.Roi.Y, Factor = r.YF }, yRange, _dataManager.YMin, _dataManager.YMax,
			//	false, ComponentColor, overlap);

			//_graphAxXL.SetData(nodes, edges,
			//	r => new ROIDim() { Raw = r.Roi.Y, Factor = r.YF }, yRange, _dataManager.YMin, _dataManager.YMax,
			//	r => new ROIDim() { Raw = r.Roi.Z, Factor = r.ZF }, zRange, _dataManager.ZMin, _dataManager.ZMax,
			//	true, ComponentColor, overlap);

			//_graphCrXL.Draw();
			//_graphSgXL.Draw();
			//_graphAxXL.Draw();
		}

        public void Save(StringBuilder htmlSink, string folderPath)
        {
            htmlSink.AppendFormat("<h1>{0}</h1>", DataType);

            htmlSink.Append("<h3>Global Strength</h3>");

            htmlSink.Append("<h3>NBSm</h3>");
            htmlSink.AppendFormat("<p>Nodes: {0} Edges: {1} pVal: {2}</p>", InterModalNodes.Count, InterModalEdges.Count, InterModalPValue);

			//_graphCrXL.Save(htmlSink, folderPath, DataType, "cr", 300, 250);
			//_graphSgXL.Save(htmlSink, folderPath, DataType, "ax", 250, 250);
			//_graphAxXL.Save(htmlSink, folderPath, DataType, "sg", 350, 250);

			//_graphAxXL.SaveGraphML(folderPath, DataType, "sg");
			//_graphSgXL.SaveGraphML(folderPath, DataType, "ax");
			//_graphCrXL.SaveGraphML(folderPath, DataType, "cr");


        }

        public void Clear()
        {
            InterModalNodes.Clear();
            InterModalEdges.Clear();

			//_graphAxXL.Clear();
			//_graphCrXL.Clear();
			//_graphSgXL.Clear();
        }
		
		public string InterModalPValue
		{
			get { return _interModalPValue; }
			set { _interModalPValue = value; NotifyOfPropertyChange(() => InterModalPValue); }
		} private string _interModalPValue;

		public BindableCollection<NodeResult> InterModalNodes { get; private set; }
		public BindableCollection<EdgeResult> InterModalEdges { get; private set; }

		private DataManager _dataManager;

        private string DataType = "Overlap";
        private Color ComponentColor = Colors.Red;
	}
}
