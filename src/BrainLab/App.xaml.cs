using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using BrainLab.Services;
using Caliburn.Micro;

namespace BrainLab
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var prefs = IoC.Get<IAppPreferences>();

			prefs.Load();

			if (string.IsNullOrEmpty(prefs.DataStorePath))
			{
				System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
				fbd.Description = "Choose location for BrainLab data store.  The study database, subject files, and reports will be stored in this location.";

				// Show open file dialog box
				System.Windows.Forms.DialogResult result = fbd.ShowDialog();

				// Process open file dialog box results
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					prefs.DataStorePath = fbd.SelectedPath;
				}
				else
					System.Windows.Application.Current.Shutdown();
			}

			base.OnStartup(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			IoC.Get<IAppPreferences>().Save();
		}
	}
}
