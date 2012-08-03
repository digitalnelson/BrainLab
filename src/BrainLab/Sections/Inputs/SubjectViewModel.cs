using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainLabLibrary;
using Caliburn.Micro;

namespace BrainLab.Sections.Inputs
{
	public class SubjectViewModel : Screen
	{
		public string SubjectId { get { return Subject.SubjectId; } private set { } }
		public string GroupId { get { return Subject.GroupId; } private set { } }

		public Subject Subject { get; set; }
	}
}
