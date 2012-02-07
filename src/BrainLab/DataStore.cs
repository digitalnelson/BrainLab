using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Embedded;

namespace BrainLab.Studio
{
	public class DataStore
	{
		static DataStore()
		{
			//if (!Directory.Exists("Data"))
			//	Directory.CreateDirectory("Data");

			_eds = new EmbeddableDocumentStore() { DataDirectory = "Data" };
			_eds.Initialize();
			_sess = _eds.OpenSession();

			LoadAppPrefs();
		}

		public static void LoadAppPrefs()
		{
			var prefs = _sess.Query<AppPrefs>().ToArray();
			if (prefs != null && prefs.Length > 0)
			{
				AppPrefs = prefs[0];
			}
			else
			{
				AppPrefs = new AppPrefs();
				AppPrefs.WindowLocation = new WindowLocation();
				_sess.Store(AppPrefs);
			}
		}

		public static void Save()
		{
			_sess.SaveChanges();
		}

		public static void Close()
		{
			_sess.Dispose();
		}

		public static AppPrefs AppPrefs { get; set; }

		private static EmbeddableDocumentStore _eds;
		private static IDocumentSession _sess;
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
		public WindowLocation WindowLocation { get; set; }
	}
}
