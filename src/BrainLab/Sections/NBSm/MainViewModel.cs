using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Ninject;

namespace BrainLab.Sections.NBSm
{
	public class MainViewModel : Screen
	{
		[Inject]
		public MainViewModel()
		{
			this.DisplayName = "NBSm";

			OverlapComponent = IoC.Get<OverlapViewModel>();

			ModalityComponents = new BindableCollection<GraphViewModel>();

			ModalityComponents.Add(IoC.Get<GraphViewModel>());
			ModalityComponents.Add(IoC.Get<GraphViewModel>());
		}

		public OverlapViewModel OverlapComponent { get; set; }
		public BindableCollection<GraphViewModel> ModalityComponents { get; set; }
	}
}
