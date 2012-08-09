using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Events;
using BrainLab.Services.Loaders;
using BrainLabStorage;
using Caliburn.Micro;

namespace BrainLab.Services
{
	public interface IRegionService
	{
		void Load(string fullPath);
		
		List<ROI> GetRegionsByIndex();

		int GetNodeCount();
		int GetEdgeCount();

		CoordRange X { get; set; }
		CoordRange Y { get; set; }
		CoordRange Z { get; set; }
	}

	public class CoordRange
	{
		public double Min { get; set; }
		public double Max { get; set; }
	}

	public class RegionService : IRegionService
	{
		private readonly IEventAggregator _eventAggregator;

		private List<ROI> _regionsOfInterest;
		private Dictionary<int, ROI> _regionsOfInterestByIndex;

		public RegionService(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;

			X = new CoordRange();
			Y = new CoordRange();
			Z = new CoordRange();

			_regionsOfInterest = new List<ROI>();
			_regionsOfInterestByIndex = new Dictionary<int, ROI>();
		}

		public void Load(string fullPath)
		{
			var roiLoader = new ROILoader(fullPath);
			var rois = roiLoader.Load();

			foreach (var roi in rois)
			{
				_regionsOfInterest.Add(roi);
				_regionsOfInterestByIndex[roi.Index] = roi;
			}

			X.Min = roiLoader.XMin;
			X.Max = roiLoader.XMax;
			Y.Min = roiLoader.YMin;
			Y.Max = roiLoader.YMax;
			Z.Min = roiLoader.ZMin;
			Z.Max = roiLoader.ZMax;

			_eventAggregator.Publish(new RegionsLoadedEvent());
		}

		public List<ROI> GetRegionsByIndex()
		{
			return _regionsOfInterest.OrderBy(r => r.Index).ToList();
		}

		public CoordRange X { get; set; }
		public CoordRange Y { get; set; }
		public CoordRange Z { get; set; }

		public int GetNodeCount()
		{
			return _regionsOfInterest.Count;
		}

		public int GetEdgeCount()
		{
			return (_regionsOfInterest.Count * (_regionsOfInterest.Count - 1)) / 2;
		}
	}
}
