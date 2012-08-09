using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Data;
using BrainLab.Events;
using BrainLab.Services;
using Caliburn.Micro;
using Ninject;

namespace BrainLab.Sections.Subjects
{
	public class MainViewModel : Screen
	{
		readonly IAppPreferences _appPreferences;
		readonly IRegionService _regionService;
		readonly ISubjectService _subjectService;
		readonly IComputeService _computeService;
		readonly IEventAggregator _eventAggregator;

		[Inject]
		public MainViewModel(IAppPreferences appPreferences, IRegionService regionService, ISubjectService subjectService, IComputeService computeService, IEventAggregator eventAggregator)
		{
			_appPreferences = appPreferences;
			_regionService = regionService;
			_subjectService = subjectService;
			_computeService = computeService;
			_eventAggregator = eventAggregator;

			this.DisplayName = "SUBJECTS";

			Subjects = new BindableCollection<SubjectViewModel>();
			DataFiles = new BindableCollection<DataFileViewModel>();

			_eventAggregator.Subscribe(this);
		}

		protected override async void OnActivate()
		{
			if (SubjectFile == null)
			{
				await Task.Run(delegate
				{
					if (SubjectFile != _appPreferences.SubjectPath)
						SubjectFile = _appPreferences.SubjectPath;

					if (DataFolder != _appPreferences.DataPath)
						DataFolder = _appPreferences.DataPath;
				});
			}

			base.OnActivate();
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

			try
			{
				_subjectService.LoadSubjectFile(SubjectFile);

				var subjects = _subjectService.GetSubjects();
				foreach (var subject in subjects)
				{
					var svm = IoC.Get<SubjectViewModel>();
					svm.Subject = subject;

					Subjects.Add(svm);
				}
			}
			catch (Exception)
			{ }
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

			await Task.Run(delegate
            {
				try
				{
					_subjectService.LoadSubjectData(DataFolder, _regionService.GetNodeCount());  // TODO: Get the ROI number from somewhere else?!?

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
				}
				catch (Exception)
				{ }
			});
		}

		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);
		}

		public string SubjectFile { get { return _subjectFile; } set { _subjectFile = value; NotifyOfPropertyChange(() => SubjectFile); LoadSubjects(); _appPreferences.SubjectPath = _subjectFile; } } private string _subjectFile;
		public BindableCollection<SubjectViewModel> Subjects { get; private set; }

		public string DataFolder { get { return _dataFolder; } set { _dataFolder = value; NotifyOfPropertyChange(() => DataFolder); LoadData(); _appPreferences.DataPath = _dataFolder; } } private string _dataFolder;
		public BindableCollection<DataFileViewModel> DataFiles { get; private set; }
	}
}
