using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrainLabStorage
{
	public class ROI
	{
		public int Index { get; set; }
		public string Name { get; set; }
		public int Ident { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
		public bool Special { get; set; }
	}
}
