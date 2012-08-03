using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BrainLab.Services;
using Caliburn.Micro;
using Ninject;

namespace BrainLab
{
	public class NinjectBootstrapper : Bootstrapper<ShellViewModel>
	{
		protected override void Configure()
		{
			_kernel = new StandardKernel();

			_kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
			_kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
			_kernel.Bind<IShell>().To<ShellViewModel>().InSingletonScope();
			_kernel.Bind<IAppPreferences>().To<AppPreferencesIS>().InSingletonScope();
			_kernel.Bind<ILocalStorage>().To<LocalStorageServiceSqlCompact>().InSingletonScope();
			_kernel.Bind<ISubjectService>().To<SubjectService>().InSingletonScope();
			_kernel.Bind<IRegionService>().To<RegionService>().InSingletonScope();
			_kernel.Bind<IComputeService>().To<ComputeService>().InSingletonScope();
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			return _kernel.Get(serviceType, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return _kernel.GetAll(serviceType);
		}

		private IKernel _kernel;
	}
}
