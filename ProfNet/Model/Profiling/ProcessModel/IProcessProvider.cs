using System.Diagnostics;

namespace ProfNet.Model.Profiling.ProcessModel
{
	public interface IProcessProvider
	{
		IProcess Start(ProcessStartInfo startInfo);

		IProcess GetProcessById(int id);
	}
}