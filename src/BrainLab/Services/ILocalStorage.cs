using System;

namespace BrainLab.Services
{
	public interface ILocalStorage
	{
		//Study[] LoadStudies();
		//Study NewStudy();
		//void DeleteStudy(Study study);

		//Subject[] LoadSubjects();
		//Subject[] LoadSubjects(Study study);
		//Subject NewSubject(Study study);
		//void DeleteSubject(Subject subj);

		//Group NewGroup(Study study);
		//Group[] LoadGroups(Study study);
		//void DeleteGroup(Group grp);

		//Task NewTask(Study study);
		//Task[] LoadTasks(Study study);
		//void DeleteTask(Task tsk);

		//Collection NewCollection(Subject subject);
		//Collection[] LoadCollections(Subject subject);
		//void DeleteCollection(Collection collection);

		void Save();
		void Close();
	}
}
