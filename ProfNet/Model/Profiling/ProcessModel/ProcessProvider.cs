using System.Diagnostics;

namespace ProfNet.Model.Profiling.ProcessModel
{
	internal class ProcessProvider : IProcessProvider
	{
		public IProcess Start(ProcessStartInfo startInfo)
		{
			return new ProcessWrapper(Process.Start(startInfo));
		}

		public IProcess GetProcessById(int id)
		{
			return new ProcessWrapper(Process.GetProcessById(id));
		}
	}
}