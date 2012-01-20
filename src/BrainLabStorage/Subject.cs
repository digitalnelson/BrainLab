using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainLabStorage
{
	public class Subject
	{
		public Subject()
		{
			Graphs = new Dictionary<string, Graph>();
		}

		public void AddProperty(string name, string value)
		{
			_attributes[name] = value;
		}

		public string Group { get; set; }

		public Dictionary<string, Graph> Graphs { get; private set; }

		public string Id
		{
			private set
			{ }

			get
			{
				string val = "";

				if (_attributes.TryGetValue("eventId", out val))
					return val;
				else
					return null;
			}
		}

		public string Is2929
		{
			private set
			{ }

			get
			{
				string val = "";

				if (_attributes.TryGetValue("is2929", out val))
					return val;
				else
					return null;
			}
		}

		public string GroupId
		{
			private set
			{ }

			get
			{
				string val = "";

				if (_attributes.TryGetValue("groupId", out val))
					return val;
				else
					return null;
			}
		}

		private Dictionary<string, string> _attributes = new Dictionary<string, string>();
	}
}
