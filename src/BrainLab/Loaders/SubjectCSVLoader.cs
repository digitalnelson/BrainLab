using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabStorage;

namespace BrainLab.Loaders
{
	class SubjectCSVLoader
	{
		public SubjectCSVLoader(string fullPath)
		{
			_fullPath = fullPath;
		}

		public List<Subject> LoadSubjectFile()
		{
			// Read in all the lines
			string[] lines = System.IO.File.ReadAllLines(_fullPath);

			char[] splitChars = new char[] { '\t' };

			// Pull out the headers
			var headers = lines[0].Split(splitChars);

			List<Subject> subjects = new List<Subject>();

			foreach (var line in lines.Skip(1))
			{
				// Create a subject
				var subject = new Subject();

				// Load up the properties
				var fields = line.Split(splitChars);
				for (int i = 0; i < fields.Length; i++)
					subject.AddProperty(headers[i], fields[i]);

				if (!Groups.ContainsKey(subject.GroupId))
					Groups[subject.GroupId] = subject.GroupId;

				subject.Group = subject.GroupId;

				// Store the subject
				subjects.Add(subject);
			}

			return subjects;
		}

		public Dictionary<string, string> Groups = new Dictionary<string, string>();

		private string _fullPath;
	}
}
