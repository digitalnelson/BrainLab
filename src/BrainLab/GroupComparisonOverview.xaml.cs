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
	public partial class GroupComparisonOverview : UserControl
	{
		public GroupComparisonOverview()
		{
			InitializeComponent();
		}

		public void LoadData(List<ROI> aalByIndex, string title, DoubleArray ctls, DoubleArray pros, SeriesChartType chartType, double pValTrend, double pValSignif)
		{
			// Process the ROI charts
			Chart chart = new Chart();

			lblPVal.Content = "Blue pVal < " + pValTrend.ToString() + " - Red pVal < " + pValSignif;

			var mainAreaName = "main";

			var main = new ChartArea(mainAreaName);
			main.AxisX.Title = title;
			chart.ChartAreas.Add(main);

			// Calculate the pvalues for all of the ROIs
			List<double> pvals = new List<double>(90);

			for (int idx = 0; idx < 90; idx++)
			{
				var ctl = ctls.GetCol(idx);
				var pro = pros.GetCol(idx);

				//pvals.Add(TTestInd.Test(ctl, pro));
			}

			// Build the chart series objects
			var sCtl = new Series();
			sCtl.ChartArea = mainAreaName;
			sCtl.ChartType = SeriesChartType.FastPoint;
			sCtl.MarkerStyle = MarkerStyle.Circle;
			sCtl.Color = System.Drawing.Color.DarkGray;
			sCtl.MarkerSize = 6;

			var sPro = new Series();
			sPro.ChartArea = mainAreaName;
			sPro.ChartType = SeriesChartType.FastPoint;
			sPro.MarkerStyle = MarkerStyle.Cross;
			sPro.Color = System.Drawing.Color.Black;
			sPro.MarkerSize = 6;

			var sProTrend = new Series();
			sProTrend.ChartArea = mainAreaName;
			sProTrend.ChartType = SeriesChartType.FastPoint;
			sProTrend.MarkerStyle = MarkerStyle.Cross;
			sProTrend.Color = System.Drawing.Color.Blue;
			sProTrend.MarkerSize = 6;

			var sProSig = new Series();
			sProSig.ChartArea = mainAreaName;
			sProSig.ChartType = SeriesChartType.FastPoint;
			sProSig.MarkerStyle = MarkerStyle.Cross;
			sProSig.Color = System.Drawing.Color.Red;
			sProSig.MarkerSize = 6;

			// Loop through ROIs
			for (var idx = 0; idx < 90; idx++)
			{
				var dpv = pvals[idx];

				// Process controls
				var dctl = ctls.GetCol(idx);
				foreach (var itm in dctl)
					sCtl.Points.AddXY(idx + 1, itm);
				
				// Process probands
				var dpro = pros.GetCol(idx);
				foreach (var itm in dpro)
				{
					if (dpv < pValSignif)
						sProSig.Points.AddXY(idx + 1, itm);
					else if (dpv < pValTrend)
						sProTrend.Points.AddXY(idx + 1, itm);
					else
						sPro.Points.AddXY(idx + 1, itm);
				}

				if (dpv < pValSignif)
					sigROIs.Items.Add(aalByIndex[idx].Name + " (" + aalByIndex[idx].Index + ")" + " " + dpv.ToString("0.000#"));
				else if (dpv < pValTrend)
					trendROIs.Items.Add(aalByIndex[idx].Name + " (" + aalByIndex[idx].Index + ")" + " " + dpv.ToString("0.000#"));
			}

			chart.Series.Add(sCtl);
			chart.Series.Add(sPro);
			chart.Series.Add(sProTrend);
			chart.Series.Add(sProSig);

			chartHost.Child = chart;
		}
	}
}
