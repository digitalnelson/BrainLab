using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using BrainLab.Data;

namespace BrainLab.Services
{
	public class LocalStorageContext : DbContext
	{
		public DbSet<Preference> Preferences { get; set; }
	}

	public class LocalStorageContextInitializer : DropCreateDatabaseIfModelChanges<LocalStorageContext>
	{
		protected override void Seed(LocalStorageContext context)
		{
			//var studies = new List<Study>
			//{
			//	new Study { StudyId = 1, Title = "SZ_169"},
			//	new Study { StudyId = 1, Title = "BlastPTSD"},
			//};
			//studies.ForEach(s => context.Studies.Add(s));

			//var groups = new List<Group>
			//{
			//	new Group(){ GroupId = 1, Title = "Controls", Study = studies[0] },
			//	new Group(){ GroupId = 2, Title = "Probands", Study = studies[0] },
			//	new Group(){ GroupId = 3, Title = "Controls", Study = studies[1] },
			//	new Group(){ GroupId = 4, Title = "PTSD", Study = studies[1] },
			//	new Group(){ GroupId = 5, Title = "Blast", Study = studies[1] },
			//	new Group(){ GroupId = 6, Title = "Blast+PTSD", Study = studies[1] },
			//};
			//groups.ForEach(g => context.Groups.Add(g));

			//var tasks = new List<Task>
			//{
			//	new Task(){ Title = "Eyes Closed FA", Study = studies[0], Duration = 60 },
			//	new Task(){ Title = "Inspection FA", Study = studies[0], Duration = 60 },
			//	new Task(){ Title = "Search FA", Study = studies[0], Duration = 60 },
			//	new Task(){ Title = "Eyes Closed FT", Study = studies[0], Duration = 60 },
			//	new Task(){ Title = "Inspection FT", Study = studies[0], Duration = 60 },
			//	new Task(){ Title = "Search FT", Study = studies[0], Duration = 60 },

			//	new Task(){ Title = "Eyes Closed FA", Study = studies[1], Duration = 60 },
			//	new Task(){ Title = "Inspection FA", Study = studies[1], Duration = 60 },
			//	new Task(){ Title = "Search FA", Study = studies[1], Duration = 60 },
			//};
			//tasks.ForEach(t => context.Tasks.Add(t));

			//var subjects = new List<Subject>
			//{
			//	new Subject() { ExternalId = "100", FirstName = "Subject", LastName = "One", Birthdate = "1980", Study = studies[0], Group = groups[0] },
			//	new Subject() { ExternalId = "101", FirstName = "Subject", LastName = "Two", Birthdate = "1981", Study = studies[0], Group = groups[1] },
			//	new Subject() { ExternalId = "102", FirstName = "Subject", LastName = "Three", Birthdate = "1982", Study = studies[1], Group = groups[2] },
			//	new Subject() { ExternalId = "103", FirstName = "Subject", LastName = "Four", Birthdate = "1983", Study = studies[1], Group = groups[3] },
			//};
			//subjects.ForEach(s => context.Subjects.Add(s));

			//var cinderella = new Princess { Name = "Cinderella" };
			//var sleepingBeauty = new Princess { Name = "Sleeping Beauty" };
			//var snowWhite = new Princess { Name = "Snow White" };

			//new List<Unicorn>
			//{
			//	new Unicorn { Name = "Binky" , Princess = cinderella },
			//	new Unicorn { Name = "Silly" , Princess = cinderella },
			//	new Unicorn { Name = "Beepy" , Princess = sleepingBeauty },
			//	new Unicorn { Name = "Creepy" , Princess = snowWhite }
			//}.ForEach(u => context.Unicorns.Add(u));

			//var efCastle = new Castle
			//{
			//	Name = "The EF Castle",
			//	Location = new Location
			//	{
			//		City = "Redmond",
			//		Kingdom = "Rainier",
			//		ImaginaryWorld = new ImaginaryWorld
			//		{
			//			Name = "Magic Unicorn World",
			//			Creator = "ADO.NET"
			//		}
			//	},
			//};

			//new List<LadyInWaiting>
			//{
			//	new LadyInWaiting { Princess = cinderella,
			//						Castle = efCastle,
			//						FirstName = "Lettice",
			//						Title = "Countess" },
			//	new LadyInWaiting { Princess = sleepingBeauty,
			//						Castle = efCastle,
			//						FirstName = "Ulrika",
			//						Title = "Lady" },
			//	new LadyInWaiting { Princess = snowWhite,
			//						Castle = efCastle,
			//						FirstName = "Yolande",
			//						Title = "Duchess" }
			//}.ForEach(l => context.LadiesInWaiting.Add(l));
		}
	}

}
