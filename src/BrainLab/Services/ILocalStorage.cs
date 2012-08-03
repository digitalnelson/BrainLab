using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using BrainLab.Data;
using Caliburn.Micro;

namespace BrainLab.Services
{
	public interface ILocalStorage
	{
		List<Preference> GetPreferences();
		Preference NewPreference(string Name, string Value);

		void Save();
		void Close();
	}

	public class LocalStorageServiceSqlCompact : ILocalStorage
	{
		private readonly LocalStorageContext _context;

		public LocalStorageServiceSqlCompact()
		{
			var prefs = IoC.Get<IAppPreferences>();

			Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", prefs.DataStorePath, "");
			Database.SetInitializer(new LocalStorageContextInitializer());

			_context = new LocalStorageContext();
		}

		// PREFERENCES

		public List<Preference> GetPreferences()
		{
			return _context.Preferences.ToList();
		}

		public Preference NewPreference(string name, string value)
		{
			return _context.Preferences.Add(new Preference { Name = name, Value = value });
		}

		// COMMON

		public void Save()
		{
			_context.SaveChanges();
		}

		public void Close()
		{
			_context.Dispose();
		}
	}
}
