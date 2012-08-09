using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLab.Data;
using BrainLab.Events;
using BrainLab.Services;
using BrainLab.Viz;
using BrainLabStorage;
using Caliburn.Micro;
using Ninject;
using OxyPlot;

namespace BrainLab.Sections.Regions
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

			this.DisplayName = "REGIONS";

			Regions = new BindableCollection<RegionViewModel>();

			_eventAggregator.Subscribe(this);
		}

		protected override async void OnActivate()
		{
			if (RegionFile == null)
			{
				await Task.Run(delegate
				{
					if (RegionFile != _appPreferences.RegionPath)
						RegionFile = _appPreferences.RegionPath;
				});
			}

			base.OnActivate();
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

		private List<RegionalViewModel> _rvms;

		public void LoadRegions()
		{
			Regions.Clear();

			try
			{
				_regionService.Load(RegionFile);

				_rvms = new List<RegionalViewModel>();

				var regions = _regionService.GetRegionsByIndex();
				foreach (var region in regions)
				{
					var rgn = IoC.Get<RegionViewModel>();
					rgn.Region = region;

					Regions.Add(rgn);

					RegionalViewModel rvm = new RegionalViewModel
					{
						ROI = region,
					};

					_rvms.Add(rvm);
				}

				AXPlotModel = LoadPlotModel(_rvms, r => r.X, r => r.Y);
				SGPlotModel = LoadPlotModel(_rvms, r => (100 - r.Y), r => r.Z);
				CRPlotModel = LoadPlotModel(_rvms, r => r.X, r => r.Z);
			}
			catch (Exception)
			{
			}
		}

		protected class RegionalViewModel
		{
			public ROI ROI { get; set; }
		}

		protected PlotModel LoadPlotModel(List<RegionalViewModel> rsvms, Func<ROI, double> horizSelector, Func<ROI, double> vertSelector)
		{
			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);
			model.PlotAreaBorderColor = OxyColors.White;

			var ba = new LinearAxis(AxisPosition.Bottom) { AbsoluteMaximum = 100, AbsoluteMinimum = 0, Maximum = 100, Minimum = 0, AxislineColor = OxyColors.White, TextColor = OxyColors.White, MajorGridlineColor = OxyColors.White, TicklineColor = OxyColors.White };
			var la = new LinearAxis(AxisPosition.Left) { AbsoluteMaximum = 100, AbsoluteMinimum = 0, Maximum = 100, Minimum = 0, AxislineColor = OxyColors.White, TextColor = OxyColors.White, TicklineColor = OxyColors.White };

			model.Axes.Add(ba);
			model.Axes.Add(la);

			var s1 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.White),
			};

			foreach (var rsvm in rsvms)
			{
				s1.Points.Add(new BrainDataPoint(horizSelector(rsvm.ROI), vertSelector(rsvm.ROI), rsvm.ROI));
			}

			model.Series.Add(s1);

			return model;
		}

		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);
		}

		public string RegionFile { get { return _regionFile; } set { _regionFile = value; NotifyOfPropertyChange(() => RegionFile); LoadRegions(); _appPreferences.RegionPath = _regionFile; } } private string _regionFile;
		public BindableCollection<RegionViewModel> Regions { get; private set; }

		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;
	}
}
