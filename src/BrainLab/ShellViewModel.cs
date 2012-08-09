using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrainLab.Services;
using Caliburn.Micro;
using Ninject;

namespace BrainLab
{
	public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
	{
		[Inject]
		public ShellViewModel()
		{
			this.DisplayName = "BRAINLAB";

			LiveTiles = IoC.Get<LiveTilesViewModel>();

			Items.Add(IoC.Get<BrainLab.Sections.Regions.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.Subjects.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.Filters.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.Graph.MainViewModel>());
			Items.Add(IoC.Get<BrainLab.Sections.NBSm.MainViewModel>());
		}

		public LiveTilesViewModel LiveTiles { get; set; }
	}
}
