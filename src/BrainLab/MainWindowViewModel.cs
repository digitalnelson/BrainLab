using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainLab.Studio
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel(DataManager dataManager)
		{
			_dataManager = dataManager;

			Groups = new ObservableCollection<Group>();
			DataTypes = new ObservableCollection<DataType>();

			Permutations = "10000";
		}

		public async Task Load(string regionFile, string subjectFile, string dataFolder)
		{
			// Load the data files into the data manager
			await Task.Run(delegate
			{
				_dataManager.LoadROIFile(regionFile);
				_dataManager.LoadSubjectFile(subjectFile);
				_dataManager.LoadAdjFiles(dataFolder);
			});

			foreach (var itm in _dataManager.Groups)
			{
				var grp = new Group() { Name = itm, Selected = true };
				Groups.Add(grp);
			}

			foreach (var itm in _dataManager.DataTypes)
			{
				var type = new DataType() { Tag = itm, Threshold = "2.00", Selected = true };
				DataTypes.Add(type);
			}
		}

		public async Task Permute()
		{
			Stopwatch sw = new Stopwatch();

			// TODO: Make this a task with the notify interface so we can update UI
			// with progress towards permutations
			await Task.Run(delegate
			{
				int numOfPerms = Int32.Parse(Permutations);
				Dictionary<string, double> thresholds = new Dictionary<string, double>();

				// Allow each data source to be NBS thresholded at a different level
				foreach (var dt in DataTypes)
				{
					if(dt.Selected)
						thresholds[dt.Tag] = Double.Parse(dt.Threshold);
				}
				
				// Load the graphs into the comparison system
				_dataManager.LoadComparisons();

				// Calculate our group differences
				_dataManager.CalculateGroupDifferences("c", "p", thresholds); // TODO: Make the group choosing configurable
								
				sw.Start();

				// Run permutations
				_dataManager.PermuteComparisons(numOfPerms, 29, thresholds);  // TODO: Make the subject size dynamic based on groups chosen

				sw.Stop();
			});

			PermutationDuration = sw.ElapsedMilliseconds;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<Group> Groups { get; set; }
		public ObservableCollection<DataType> DataTypes {get; set;}

		public string Permutations { get; set; }
		public long PermutationDuration { get; set; }

		private DataManager _dataManager;
	}

	class Group
	{
		public bool Selected { get; set; }
		public string Name { get; set; }
	}

	class DataType
	{
		public bool Selected { get; set; }
		public string Tag { get; set; }
		public string Threshold { get; set; } 
	}
}
