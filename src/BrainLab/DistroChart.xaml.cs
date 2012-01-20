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
	/// Interaction logic for DistroChart.xaml
	/// </summary>
	public partial class DistroChart : UserControl
	{
		private Chart _chart;
		private string _mainAreaName = "main";

		public DistroChart()
		{
			InitializeComponent();
		}

		public void Load(List<ROI> aalByIndex, string title, DoubleArray ctls, DoubleArray pros)
		{
			// Process the ROI charts
			_chart = new Chart();

			var main = new ChartArea(_mainAreaName);
			main.AxisX.Title = title;
			main.AxisY.IsStartedFromZero = false;
			_chart.ChartAreas.Add(main);

			var hidden = new ChartArea("hidden");
			hidden.Visible = false;
			_chart.ChartAreas.Add(hidden);

			// Build the chart series objects
			var sCtl = new Series("Probands");
			sCtl.ChartArea = "hidden";
			sCtl.ChartType = SeriesChartType.Line;
			sCtl.MarkerStyle = MarkerStyle.Circle;
			sCtl.Color = System.Drawing.Color.LightGray;
			sCtl.MarkerSize = 7;

			var sPro = new Series("Controls");
			sPro.ChartArea = "hidden";
			sPro.ChartType = SeriesChartType.Line;
			sPro.MarkerStyle = MarkerStyle.Cross;
			sPro.Color = System.Drawing.Color.DarkGray;
			sPro.MarkerSize = 7;

			var sBP = new Series();
			sBP.ChartArea = _mainAreaName;
			sBP.ChartType = SeriesChartType.BoxPlot;
			sBP["BoxPlotSeries"] = "Probands;Controls";
			sBP["BoxPlotShowUnusualValues"] = "True";

			var diff = ctls.Average() - pros.Average();
			//var pVal = TTestInd.Test(ctls, pros);

			//lblPVal.Content = "ctls-pros: " + diff.ToString("0.0000") +  " pVal: " + pVal.ToString("0.0000##");

			// Loop through ROIs
			for (var idx = 0; idx < 29; idx++)
			{
				int item = 0;
				//bool signif = pvals[idx] < 0.0005 ? true : false;

				// Process controls
				var dctl = ctls.GetCol(idx);
				foreach (var itm in dctl)
				{
					item = sCtl.Points.AddXY(idx + 1, itm);
				}

				// Process probands
				var dpro = pros.GetCol(idx);
				foreach (var itm in dpro)
				{
					item = sPro.Points.AddXY(idx + 1, itm);
				}
			}

			_chart.Series.Add(sCtl);
			_chart.Series.Add(sPro);
			_chart.Series.Add(sBP);

			_chart.Customize += new EventHandler(chart_Customize);

			_chart.AntiAliasing = AntiAliasingStyles.All;
			_chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

			chartHost.Child = _chart;
		}

		void chart_Customize(object sender, EventArgs e)
		{
			_chart.ChartAreas[_mainAreaName].AxisX.CustomLabels[0].Text = "";
			_chart.ChartAreas[_mainAreaName].AxisX.CustomLabels[1].Text = "Controls";
			_chart.ChartAreas[_mainAreaName].AxisX.CustomLabels[2].Text = "Probands";
			_chart.ChartAreas[_mainAreaName].AxisX.CustomLabels[3].Text = "";
		}
	}
}
