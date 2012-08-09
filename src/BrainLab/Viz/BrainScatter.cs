using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabStorage;
using OxyPlot;


namespace BrainLab.Viz
{
	public class BrainDataPoint : IDataPoint
	{
		public static readonly DataPoint Undefined = new DataPoint(double.NaN, double.NaN);

		internal double x;
		internal double y;
		internal ROI roi;

		public BrainDataPoint(double x, double y, ROI roi)
		{
			this.x = x;
			this.y = y;
			this.roi = roi;
		}

		public double X { get { return this.x; } set { this.x = value; } }
		public double Y { get { return this.y; } set { this.y = value; } }
		public ROI ROI { get { return this.roi; } set { this.roi = value; } }

		public string ToCode()
		{
			return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1}", this.x, this.y);
		}

		public override string ToString()
		{
			return this.x + " " + this.y;
		}

	}

	public class BrainScatterSeries : ScatterSeries
	{
		public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
		{
			if (this.XAxis == null || this.YAxis == null)
			{
				return null;
			}

			if (interpolate)
			{
				return null;
			}

			TrackerHitResult result = null;
			double minimumDistance = double.MaxValue;
			int i = 0;

			var xaxisTitle = this.XAxis.Title ?? "X";
			var yaxisTitle = this.YAxis.Title ?? "Y";
			var colorAxisTitle = (this.ColorAxis != null ? this.ColorAxis.Title : null) ?? "Z";

			var formatString = TrackerFormatString;
			if (string.IsNullOrEmpty(this.TrackerFormatString))
			{
				// Create a default format string
				formatString = "{1}: {2}\n{3}: {4}";
				if (this.ColorAxis != null)
				{
					formatString += "\n{5}: {6}";
				}
			}

			foreach (var p in this.Points)
			{
				if (this.XAxis == null || this.YAxis == null || p.X < this.XAxis.ActualMinimum || p.X > this.XAxis.ActualMaximum || p.Y < this.YAxis.ActualMinimum
					|| p.Y > this.YAxis.ActualMaximum)
				{
					i++;
					continue;
				}

				var dp = new DataPoint(p.X, p.Y);
				var sp = Axis.Transform(dp, this.XAxis, this.YAxis);
				double dx = sp.X - point.X;
				double dy = sp.Y - point.Y;
				double d2 = (dx * dx) + (dy * dy);

				if (d2 < minimumDistance)
				{
					var item = this.GetItem(i);
					result = new TrackerHitResult(this, dp, sp, item);

					object xvalue = this.XAxis != null ? this.XAxis.GetValue(dp.X) : dp.X;
					object yvalue = this.YAxis != null ? this.YAxis.GetValue(dp.Y) : dp.Y;
					object zvalue = null;

					var scatterPoint = p as ScatterPoint;
					if (scatterPoint != null)
					{
						if (!double.IsNaN(scatterPoint.Value) && !double.IsInfinity(scatterPoint.Value))
						{
							zvalue = scatterPoint.Value;
						}
					}

					//result.Text = StringHelper.Format(
					//	this.ActualCulture,
					//	formatString,
					//	item,
					//	this.Title,
					//	xaxisTitle,
					//	xvalue,
					//	yaxisTitle,
					//	yvalue,
					//	colorAxisTitle,
					//	zvalue);

					var rdp = p as BrainDataPoint;

					if (rdp != null)
						result.Text = rdp.ROI.Name;

					minimumDistance = d2;
				}

				i++;
			}

			return result;
		}
	}
}
