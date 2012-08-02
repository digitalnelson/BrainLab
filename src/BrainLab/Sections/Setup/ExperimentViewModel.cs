using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Events;
using BrainLab.Services;
using Caliburn.Micro;
using Ninject;

namespace BrainLab.Sections.Setup
{
	public class ExperimentViewModel : Screen, IHandle<DataTypeSelection>, IHandle<GroupAssignment>
	{
		readonly IRegionService _regionService;
		readonly ISubjectService _subjectService;
		readonly IComputeService _computeService;
		readonly IEventAggregator _eventAggregator;

		[Inject]
		public ExperimentViewModel(IRegionService regionService, ISubjectService subjectService, IComputeService computeService, IEventAggregator eventAggregator)
		{
			_regionService = regionService;
			_subjectService = subjectService;
			_computeService = computeService;
			_eventAggregator = eventAggregator;

			this.DisplayName = "SOURCES";

			Regions = new BindableCollection<RegionViewModel>();
			Groups = new BindableCollection<GroupViewModel>();
			Subjects = new BindableCollection<SubjectViewModel>();
			DataTypes = new BindableCollection<DataTypeViewModel>();
			DataFiles = new BindableCollection<DataFileViewModel>();

			_eventAggregator.Subscribe(this);

			// TODO: Load prefs
		}

		public void OpenRegionFile()
		{
			// Configure open file dialog box
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.FileName = "AAL"; // Default file name
			dlg.DefaultExt = ".txt"; // Default file extension
			dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

			if (!string.IsNullOrEmpty(RegionFile))
				dlg.InitialDirectory = System.IO.Path.GetFullPath(RegionFile);

			// Show open file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true)
			{
				// Open document
				RegionFile = dlg.FileName;
			}
		}

		public void LoadRegions()
		{
			_regionService.Load(RegionFile);

			var regions = _regionService.GetRegionsByIndex();
			foreach (var region in regions)
			{
				var rgn = IoC.Get<RegionViewModel>();
				rgn.Region = region;

				Regions.Add(rgn);
			}
		}

		public void OpenSubjectFile()
		{
			// Configure open file dialog box
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.FileName = "Subjects"; // Default file name
			dlg.DefaultExt = ".txt"; // Default file extension
			dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

			if (!string.IsNullOrEmpty(SubjectFile))
				dlg.InitialDirectory = System.IO.Path.GetFullPath(SubjectFile);

			// Show open file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true)
			{
				// Open document
				SubjectFile = dlg.FileName;
			}
		}

		public void LoadSubjects()
		{
			Subjects.Clear();
			Groups.Clear();

			_subjectService.LoadSubjectFile(SubjectFile);

			var subjects = _subjectService.GetSubjects();
			foreach (var subject in subjects)
			{
				var svm = IoC.Get<SubjectViewModel>();
				svm.Subject = subject;

				Subjects.Add(svm);
			}

			var groups = _subjectService.GetGroups();
			foreach (var group in groups)
			{
				var grp = IoC.Get<GroupViewModel>();
				grp.Title = group;

				Groups.Add(grp);
			}
		}

		public void OpenDataFolder()
		{
			// Configure open file dialog box
			var dlg = new System.Windows.Forms.FolderBrowserDialog();

			if (!string.IsNullOrEmpty(DataFolder))
				dlg.SelectedPath = System.IO.Path.GetFullPath(DataFolder);
			else
				dlg.SelectedPath = System.IO.Path.GetDirectoryName(SubjectFile);

			// Show open file dialog box
			var result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				// Open document
				this.DataFolder = dlg.SelectedPath;
			}
		}

		public async void LoadData()
		{
			DataFiles.Clear();
			DataTypes.Clear();

			await Task.Run(delegate
            {
				_subjectService.LoadSubjectData(DataFolder, Regions.Count);  // TODO: Get the ROI number from somewhere else?!?

				var dataTypes = _subjectService.GetDataTypes();
				foreach (var dataType in dataTypes)
				{
					var dt = IoC.Get<DataTypeViewModel>();
					dt.Title = dataType;

					DataTypes.Add(dt);
				}

				var subjects = _subjectService.GetSubjects();
				foreach (var subject in subjects)
				{
					foreach (var graph in subject.Graphs)
					{
						var df = IoC.Get<DataFileViewModel>();
						df.Title = graph.Value.DataSource;
						df.SubjectId = subject.SubjectId;

						DataFiles.Add(df);
					}
				}
			});
		}

		public string RegionFile { get { return _regionFile; } set { _regionFile = value; NotifyOfPropertyChange(() => RegionFile); LoadRegions(); } } private string _regionFile;
		public BindableCollection<RegionViewModel> Regions { get; private set; }

		public string SubjectFile { get { return _subjectFile; } set { _subjectFile = value; NotifyOfPropertyChange(() => SubjectFile); LoadSubjects(); } } private string _subjectFile;
		public BindableCollection<GroupViewModel> Groups { get; private set; }
		public BindableCollection<SubjectViewModel> Subjects { get; private set; }

		public string DataFolder { get { return _dataFolder; } set { _dataFolder = value; NotifyOfPropertyChange(() => DataFolder); LoadData(); } } private string _dataFolder;
		public BindableCollection<DataTypeViewModel> DataTypes { get; private set; }
		public BindableCollection<DataFileViewModel> DataFiles { get; private set; }
	}
}
