using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainLab.Common.Viz
{
	public class InvisibleAxis : LinearAxis
	{
		public override bool IsXyAxis()
		{
			return true;
		}

		public override OxySize Measure(IRenderContext rc) { return new OxySize(0, 0); }
	}
}
