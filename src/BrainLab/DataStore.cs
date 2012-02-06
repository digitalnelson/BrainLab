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
			if(!Directory.Exists("Data"))
				Directory.CreateDirectory("Data");

			_eds = new EmbeddableDocumentStore() { DataDirectory = "Data" };
			_eds.Initialize();

			LoadAppPrefs();
		}

		private static void InternalSave(IDocumentSession session)
		{
			session.Store(AppPrefs);
			session.SaveChanges();
		}

		public static void LoadAppPrefs()
		{
			using (var session = _eds.OpenSession())
			{
				var prefs = session.Load<AppPrefs>();
				if (prefs != null && prefs.Length > 0)
				{
					AppPrefs = prefs[0];
				}
				else
				{
					AppPrefs = new AppPrefs();
					AppPrefs.WindowLocation = new WindowLocation();

					InternalSave(session);
				}
			}
		}

		public static void SaveAppPrefs()
		{
			using (var session = _eds.OpenSession())
			{
				InternalSave(session);
			}
		}

		public static AppPrefs AppPrefs { get; set; }

		private static EmbeddableDocumentStore _eds;
	}

	public class WindowLocation
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int X { get; set; }
	}

	public class AppPrefs
	{
		public int Id  {get; set;}
		public string RoiFilePath {get; set;}
		public string SubjectfilePath {get; set;}
		public string DataFileDir {get; set;}
		public WindowLocation WindowLocation { get; set; }
	}
}
