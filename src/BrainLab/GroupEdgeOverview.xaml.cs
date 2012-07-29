using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Threading.Tasks;
using BrainLabStorage;

namespace BrainLab
{
	/// <summary>
	/// Interaction logic for GroupComparisonChart.xaml
	/// </summary>
	public partial class GroupEdgeOverview : UserControl
	{
		private List<double> _dPDist;

		public ObservableCollection<string> SigEdges { get; private set; }

		public GroupEdgeOverview()
		{
			InitializeComponent();
			_dPDist = new List<double>();

			SigEdges = new ObservableCollection<string>();
		}
		
		//public void LoadData(List<ROI> aalByIndex, string title, DoubleArray ctls, DoubleArray pros, SeriesChartType chartType, PermDist dist)
		//{
		//	double[] adj = dist.ProcessEdges(title);
		//	heatMap.SetArray(adj, 0.05, dist);


		//	//var mtx = new DenseMatrix(58, 8100);

		//	//for (int i = 0; i < 29; i++)
		//	//{
		//	//	var itms = ctls.GetSlice(new Slice(i), Slice.All, Slice.All).Squeeze();
		//	//	mtx.SetRow(i, itms.ToFlatSystemArray());
		//	//}

		//	//for (int i = 0; i < 29; i++)
		//	//{
		//	//	var itms = pros.GetSlice(new Slice(i), Slice.All, Slice.All).Squeeze();
		//	//	mtx.SetRow(i+29, itms.ToFlatSystemArray());
		//	//}

		//	//DoubleArray da = new DoubleArray(8100);
		//	//da.FillValue(1);

		//	//var splitIdx = mtx.RowCount / 2;

		//	//for (var i = 0; i < 90; i++)
		//	//{
		//	//	for (var j = i + 1; j < 90; j++)
		//	//	{
		//	//		var denseIdx = (i * 90) + j;
		//	//		var g1 = mtx.Column(denseIdx, 0, splitIdx);
		//	//		var g2 = mtx.Column(denseIdx, splitIdx, mtx.RowCount - splitIdx);

		//	//		var tstat = Math.Abs(Stats.TStat(g1, g2));
		//	//		var pval = dist.GetPVal(title, tstat);
		//	//		if (pval < 0.05)
		//	//		{
		//	//			da[denseIdx] = pval;
		//	//			if (SigEdges.Count < 20)
		//	//			{
		//	//				double diff = Stats.MeanDiff(g1, g2);
		//	//				SigEdges.Add(aalByIndex[i].Name + " & " + aalByIndex[j].Name + " = " + diff.ToString("0.0000##"));
		//	//			}
		//	//		}
		//	//	}
		//	//}

		//	//foreach (var nm in SigEdges)
		//	//{
		//	//	if (lstEdges.Items.Count < 100)
		//	//		lstEdges.Items.Add(nm);
		//	//}

		//	//lblPVal.Content = "Red tstat > 0.05";
		//	//sigROIs.Content = da.Where(x => x < 0.05).Count().ToString();

		//	//heatMap.SetArray(ArrayUtils.Reshape(da, 90, 90, OrderOp.RowMajor), 0.05);
		//}
	}
}
