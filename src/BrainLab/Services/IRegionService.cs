using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Services.Loaders;
using BrainLabStorage;

namespace BrainLab.Services
{
	public interface IRegionService
	{
		void Load(string fullPath);
		
		List<ROI> GetRegionsByIndex();

		int GetNodeCount();
		int GetEdgeCount();
	}

	public class RegionService : IRegionService
	{
		private List<ROI> _regionsOfInterest;
		private Dictionary<int, ROI> _regionsOfInterestByIndex;

		public RegionService()
		{
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

			this.XMin = roiLoader.XMin;
			this.XMax = roiLoader.XMax;
			this.YMin = roiLoader.YMin;
			this.YMax = roiLoader.YMax;
			this.ZMin = roiLoader.ZMin;
			this.ZMax = roiLoader.ZMax;
		}

		public List<ROI> GetRegionsByIndex()
		{
			return _regionsOfInterest.OrderBy(r => r.Index).ToList();
		}

		public double XMin;
		public double XMax;
		public double YMin;
		public double YMax;
		public double ZMin;
		public double ZMax;

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
