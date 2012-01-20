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
using BrainLabStorage;

namespace BrainLab.Studio
{
	/// <summary>
	/// Interaction logic for GroupComparisonChart.xaml
	/// </summary>
	public partial class GroupComparisonDetails : UserControl
	{
		public GroupComparisonDetails()
		{
			InitializeComponent();
		}

		public void LoadData(List<ROI> aalByIndex, string title, DoubleArray ctls, DoubleArray pros, SeriesChartType chartType, double pVal, double pValCorr)
		{
			// Process the ROI charts
			Chart chart = new Chart();

			lblPVal.Content = "pVal < " + pVal.ToString() + " pVal (bonn) < " + pValCorr;

			var cmpAreaName = "cmp";

			var cmp = new ChartArea(cmpAreaName);
			cmp.AxisX.Title = title + " Grp Diff";
			cmp.AxisX.IsLabelAutoFit = false;
			cmp.AxisX.LabelStyle.Enabled = true;
			cmp.AxisX.LabelStyle.Interval = 1;
			cmp.AxisY.IsStartedFromZero = false;
			chart.ChartAreas.Add(cmp);

			// Calculate the pvalues for all of the ROIs
			List<object> ct = new List<object>(90);
			List<object> pr = new List<object>(90);
			List<object> means = new List<object>(90);
			List<double> pvals = new List<double>(90);

			for (int idx = 0; idx < 90; idx++)
			{
				var ctl = ctls.GetCol(idx);
				var pro = pros.GetCol(idx);

				var pval = 1; //TTestInd.Test(ctl, pro);

				if (pval < pVal)
				{
					ct.Add(new { x = idx, y = ctl.Average() });
					pr.Add(new { x = idx, y = pro.Average() });
					means.Add(new { x = idx, y = ctl.Average() - pro.Average() });
				}
				
				pvals.Add(pval);
			}

			// Build the chart series objects
			var sMeans = new Series();
			sMeans.ChartArea = cmpAreaName;
			sMeans.ChartType = chartType;
			sMeans.Points.DataBind(means, "x", "y", "");
			sMeans.IsXValueIndexed = true;

			var sCt = new Series();
			sCt.ChartArea = cmpAreaName;
			sCt.ChartType = chartType;
			sCt.Points.DataBind(ct, "x", "y", "");
			sCt.IsXValueIndexed = true;

			var sPr = new Series();
			sPr.ChartArea = cmpAreaName;
			sPr.ChartType = chartType;
			sPr.Points.DataBind(pr, "x", "y", "");
			sPr.IsXValueIndexed = true;

			//chart.Series.Add(sCt);
			//chart.Series.Add(sPr);
			chart.Series.Add(sMeans);

			chartHost.Child = chart;
		}
	}
}
