using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrainLab.Services
{
	public interface IAppPreferences
	{
		string DataStorePath {get; set;}
		string RegionPath { get; set; }
		string SubjectPath { get; set; }
		string DataPath { get; set; }

		void Load();
		void Save();
	}

	public class AppPreferencesIS : IAppPreferences
	{
		private string _fileName = "AppPrefs.txt";

		public string DataStorePath { get; set; }
		public string RegionPath { get; set; }
		public string SubjectPath { get; set; }
		public string DataPath { get; set; }

		public void Load()
		{
			// Restore application-scope property from isolated storage
			IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
			try
			{
				using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(_fileName, FileMode.Open, storage))
				using (StreamReader reader = new StreamReader(stream))
				{
					// Restore each application-scope property individually
					while (!reader.EndOfStream)
					{
						string[] keyValue = reader.ReadLine().Split(new char[] { ',' });

						switch (keyValue[0])
						{
							case "DataStorePath":
								DataStorePath = keyValue[1];
								break;
							case "RegionPath":
								RegionPath = keyValue[1];
								break;
							case "SubjectPath":
								SubjectPath = keyValue[1];
								break;
							case "DataPath":
								DataPath = keyValue[1];
								break;
							default:
								Application.Current.Properties[keyValue[0]] = keyValue[1];
								break;
						}
					}
				}
			}
			catch (FileNotFoundException)
			{}
		}
		
		public void Save()
		{
			// Persist application-scope property to isolated storage
			IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
			using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(_fileName, FileMode.Create, storage))
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.WriteLine("{0},{1}", "DataStorePath", DataStorePath);
				writer.WriteLine("{0},{1}", "RegionPath", RegionPath);
				writer.WriteLine("{0},{1}", "SubjectPath", SubjectPath);
				writer.WriteLine("{0},{1}", "DataPath", DataPath);

				// Persist each application-scope property individually
				foreach (string key in Application.Current.Properties.Keys)
				{
					writer.WriteLine("{0},{1}", key, Application.Current.Properties[key]);
				}
			}
		}
	}
}
