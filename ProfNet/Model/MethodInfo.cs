using System.Collections.Generic;

namespace ProfNet.Model
{
	public class MethodInfo
	{
		public string Name { set; get; }
		public double Duration { set; get; }
		public double StartTime { set; get; }
		public string MethodId { set; get; }
		public List<MethodInfo> Childs { set; get; }
		public string ThreadId { set; get; }

		public override string ToString()
		{
			return Name;
		}
	}
}