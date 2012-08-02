using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using BrainLabStorage;

namespace BrainLab.Sections.Setup
{
	public class RegionViewModel : Screen
	{
		public ROI Region { get; set; }

		public int Index { get { return Region != null ? Region.Index : 0; } }
		public string Title { get { return Region != null ? Region.Name : ""; } }
	}
}
