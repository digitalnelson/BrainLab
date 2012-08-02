using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainLab.Services
{
	public class DataStore
	{
		static DataStore()
		{
			//if (!Directory.Exists("Data"))
			//	Directory.CreateDirectory("Data");
			long ticks = DateTime.Now.Ticks;
			//_eds = new EmbeddableDocumentStore() { DataDirectory = "Data" };
			//_eds.Initialize();
			//_sess = _eds.OpenSession();

			long ticks2 = DateTime.Now.Ticks;
			TimeSpan ts = new TimeSpan(ticks2 - ticks);

			//_dataTypes = new Dictionary<string, DataType>();
			//_dataTypes.Add("DTI", new DataType() { Tag = "DTI", Selected = true, Threshold = "2.15" });
			//_dataTypes.Add("fMRI", new DataType() { Tag = "fMRI", Selected = true, Threshold = "3.225" });

			LoadAppPrefs();
		}

		public static void LoadAppPrefs()
		{
			//var prefs = _sess.Query<AppPrefs>().ToArray();
			//if (prefs != null && prefs.Length > 0)
			//{
			//	AppPrefs = prefs[0];
			//}
			//else
			//{
			//	AppPrefs = new AppPrefs();
			//	AppPrefs.WindowLocation = new WindowLocation();
			//	//_sess.Store(AppPrefs);
			//}
		}

		public static DataType GetDataType(string tag)
		{
			if (_dataTypes.ContainsKey(tag))
				return _dataTypes[tag];
			else
				return null;
		}

		public static void Save()
		{
			//_sess.SaveChanges();
		}

		public static void Close()
		{
			//_sess.Dispose();
		}

		public static AppPrefs AppPrefs { get; set; }

		//private static EmbeddableDocumentStore _eds;
		//private static IDocumentSession _sess;
		private static Dictionary<string, DataType> _dataTypes;
	}

	public class WindowLocation
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
	}

	public class AppPrefs
	{
		public string Id  {get; set;}
		public string RoiFilePath {get; set;}
		public string SubjectfilePath {get; set;}
		public string DataFileDir {get; set;}
		public string OutputDir { get; set; }
		public WindowLocation WindowLocation { get; set; }
	}
}
