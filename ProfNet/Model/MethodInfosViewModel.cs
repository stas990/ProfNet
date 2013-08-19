using System.Collections.Generic;

namespace ProfNet.Model
{
	public class MethodInfosViewModel : ViewModelBase
	{
		public IEnumerable<MethodInfo> Methods { set; get; } 

		public override string Title
		{
			get { return "Methods"; }
		}
	}
}