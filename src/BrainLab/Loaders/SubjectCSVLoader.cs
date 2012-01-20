using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabStorage;

namespace BrainLab.Studio.Loaders
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

			char[] splitChars = new char[] { ' ', '\t' };

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

				//// TODO: Replace this with filters
				//// Throw this one away if not 2929
				//if (!System.Convert.ToBoolean(subject.Is2929))
				//	continue;

				// TODO: Replace this with group id maps
				if (subject.GroupId == "1")
					subject.Group = "1";
				else
					subject.Group = "0";

				// Store the subject
				subjects.Add(subject);
			}

			return subjects;
		}

		private string _fullPath;
	}
}
