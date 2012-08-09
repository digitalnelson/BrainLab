using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace BrainLab.Sections.Subjects
{
	public class DataFileViewModel : Screen
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string SubjectId { get { return _inlSubjectId; } set { _inlSubjectId = value; NotifyOfPropertyChange(() => SubjectId); } } private string _inlSubjectId;
		public string EventId { get { return _inlEventId; } set { _inlEventId = value; NotifyOfPropertyChange(() => EventId); } } private string _inlEventId;
	}
}
