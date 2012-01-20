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
using System.ComponentModel;

namespace BrainLab.Studio
{
	/// <summary>
	/// Interaction logic for DoubleArrayHeatMap.xaml
	/// </summary>
	public partial class DoubleArrayHeatMap : UserControl
	{
		public DoubleArrayHeatMap()
		{
			InitializeComponent();
		}

		private double[] _adj = null;
		private double _tstatPerm = 0;
		private PermDist _dist = null;

		public void SetArray(double[] adj, double tstatPerm, PermDist dist)
		{
			_adj = adj;
			_tstatPerm = tstatPerm;
			_dist = dist;

			this.InvalidateVisual();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			//if (DesignerProperties.GetIsInDesignMode(this))
			//{
			//	_array = new DoubleArray(90, 90);
			//	_array.FillValue(1);

			//	_array[45, 45] = 0.0002;
			//	_array[14, 27] = 0.04;
			//}

			if (_adj != null)
			{
				var w = this.ActualWidth;
				var h = this.ActualHeight;

				//var x = 0.0;
				var s = w < h ? w : h;

				var wa = (w - s) / 2.0;
				var ha = (h - s) / 2.0;

				var m = 10.0;
				var cs = (s - (m * 2)) / 90;

				for (int idx = 0; idx < _adj.Length; idx++)
				{
					pair p = _dist.LookupIdx(idx);

					if (_adj[idx] < _tstatPerm)
						drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Red, 1), new Rect((p.i * cs) + m + wa, (p.j * cs) + m + ha, cs, cs));
					else if (_adj[idx] < 0.1)
						drawingContext.DrawRectangle(Brushes.Blue, new Pen(Brushes.Blue, 1), new Rect((p.i * cs) + m + wa, (p.j * cs) + m + ha, cs, cs));
					else
						drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.LightGray, 1), new Rect((p.i * cs) + m + wa, (p.j * cs) + m + ha, cs, cs));
				}
			}
		}
	}
}
