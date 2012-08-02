using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrainLab
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel(DataManager dataManager)
		{
			_dataManager = dataManager;

			Groups = new ObservableCollection<Group>();
			DataTypes = new ObservableCollection<DataType>();

			Permutations = "500";
		}

		public void Load(string regionFile, string subjectFile, string dataFolder, string outputFolder)
		{
            //// Load the data files into the data manager
            //await Task.Run(delegate
            //{
            //    _dataManager.LoadROIFile(regionFile);
            //    _dataManager.LoadSubjectFile(subjectFile);
            //    _dataManager.LoadAdjFiles(dataFolder);
            //    OutputFolder = outputFolder;
            //});

            //foreach (var itm in _dataManager.Groups)
            //{
            //    var grp = new Group() { Name = itm, Selected = true };
            //    Groups.Add(grp);
            //}

            //foreach (var itm in _dataManager.DataTypes)
            //{
            //    DataType type = DataStore.GetDataType(itm);

            //    if (type==null)
            //        type =	new DataType() { Tag = itm, Threshold = "2.15", Selected = true };

            //    DataTypes.Add(type);
            //}
		}

		public void Permute()
		{
            //Stopwatch sw = new Stopwatch();

            //// TODO: Make this a task with the notify interface so we can update UI
            //// with progress towards permutations
            //await Task.Run(delegate
            //{
            //    int numOfPerms = Int32.Parse(Permutations);
            //    Dictionary<string, double> thresholds = new Dictionary<string, double>();

            //    // Allow each data source to be NBS thresholded at a different level
            //    foreach (var dt in DataTypes)
            //    {
            //        if(dt.Selected)
            //            thresholds[dt.Tag] = Double.Parse(dt.Threshold);
            //    }

            //    List<string> grps = new List<string>();
            //    foreach (var grp in Groups)
            //    {
            //        if (grp.Selected)
            //            grps.Add(grp.Name);
            //    }

            //    if (grps.Count != 2)
            //    {
            //        MessageBox.Show("Please select 2 groups.");
            //        return;
            //    }
				
            //    // Load the graphs into the comparison system
            //    _dataManager.LoadComparisons(thresholds);

            //    // Calculate our group differences
            //    _dataManager.CalculateGroupDifferences(grps[0], grps[1], thresholds);
								
            //    sw.Start();

            //    // Run permutations
            //    _dataManager.PermuteComparisons(numOfPerms, thresholds);

            //    sw.Stop();
            //});

            //PermutationDuration = sw.ElapsedMilliseconds;
		}

		//public void Save(OverlapView overlap, List<GraphView> wrkSpaceComponents)
		//{
		//	StringBuilder htmlSink = new StringBuilder("<html><body>");

		//	htmlSink.Append("<h1>BrainLab Report</h1>");
		//	htmlSink.Append("<ul>");
		//	htmlSink.AppendFormat("<li><b>Permutations:</b> {0}</li>", Permutations);
		//	foreach (var dt in DataTypes)
		//	{
		//		if (dt.Selected)
		//		{
		//			htmlSink.AppendFormat("<li><b>DataType:</b>  {0} <b>Threshold:</b>  {1}</li>", dt.Tag, dt.Threshold);
		//		}
		//	}
		//	htmlSink.Append("</ul>");

		//	overlap.Save(htmlSink, OutputFolder);
		//	foreach (var cmp in wrkSpaceComponents)
		//		cmp.Save(htmlSink, OutputFolder);

		//	htmlSink.Append("</body></html>");

		//	using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(OutputFolder, "index.html")))
		//	{
		//		sw.Write(htmlSink.ToString());
		//	}
		//}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<Group> Groups { get; set; }
		public ObservableCollection<DataType> DataTypes {get; set;}

		public string Permutations { get; set; }
		public long PermutationDuration { get; set; }

		public string OutputFolder { get; set; }

		public DataManager DataManager
		{
			get { return _dataManager; }
		}

		private DataManager _dataManager;
	}

	public class Group
	{
		public bool Selected { get; set; }
		public string Name { get; set; }
	}

	public class DataType
	{
		public bool Selected { get; set; }
		public string Tag { get; set; }
		public string Threshold { get; set; } 
	}
}
