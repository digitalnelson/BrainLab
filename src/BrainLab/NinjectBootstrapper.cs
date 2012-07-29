using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            //_kernel.Bind<LocalStorageService>().To<LocalStorageService>().InSingletonScope();

			//_kernel.Load(Assembly.GetExecutingAssembly());
			//_kernel.Load("*.dll");
			//var assemblies
			//	= _kernel.GetModules()
			//		.Select(m => m.GetType().Assembly)
			//		.Distinct()
			//		.ToList();
			//AssemblySource.Instance.AddRange(assemblies);
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
