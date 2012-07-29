using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabStorage;

namespace BrainLab.Loaders
{
	class ROILoader
	{
		public ROILoader(string fullPath)
		{
			_fullPath = fullPath;
		}

		public double XMax;
		public double XMin;
		public double YMax;
		public double YMin;
		public double ZMax;
		public double ZMin;

		private Dictionary<int, string> _isSpecial = new Dictionary<int, string>() 
		{
 			{2601, "Left Frontal Superior Medial"},
			{2201, "Left Frontal Mid"}, 
			{7001, "Left Caudate"}, 
			{7011, "Left Putamen"}, 
			{3001, "Left Insula"}, 
			{8101, "Left Heschl"}, 
			{6401, "Left Paracentral Lobule"}, 
			{8201, "Left Temporal Mid"}, 
			{6301, "Left Precuneus"}, 
			{5021, "Left Lingual"}, 
			{5012, "Right Cuneus"}, 
			{6302, "Right Precuneus"}
		};

		public List<ROI> Load()
		{
			// Read in all the lines
			string[] lines = System.IO.File.ReadAllLines(_fullPath);
			List<ROI> regionsOfInterest = new List<ROI>();

			foreach (var line in lines)
			{
				var fields = line.Split(' ');

				ROI roi = new ROI()
				{
					Index = Int32.Parse(fields[0]),
					Name = fields[1],
					Ident = Int32.Parse(fields[2]),
					X = Double.Parse(fields[3]),
					Y = Double.Parse(fields[4]),
					Z = Double.Parse(fields[5]),
				};

				//if (_isSpecial.ContainsKey(roi.Ident))
				//	roi.Special = true;
				
				regionsOfInterest.Add(roi);
			}

			XMax = (double)regionsOfInterest.Max(r => r.X);
			XMin = (double)regionsOfInterest.Min(r => r.X);

			YMax = (double)regionsOfInterest.Max(r => r.Y);
			YMin = (double)regionsOfInterest.Min(r => r.Y);

			ZMax = (double)regionsOfInterest.Max(r => r.Z);
			ZMin = (double)regionsOfInterest.Min(r => r.Z);

			return regionsOfInterest;
		}

		private string _fullPath;
	}
}
