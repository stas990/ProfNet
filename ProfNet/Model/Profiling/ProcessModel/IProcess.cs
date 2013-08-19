using System;
using System.Diagnostics;

namespace ProfNet.Model.Profiling.ProcessModel
{
	public interface IProcess : IDisposable
	{
		ProcessStartInfo StartInfo { get; }
		bool EnableRaisingEvents { set; get; }
		event EventHandler Exited;
		bool HasExited { get; }
		void Kill();
	}
}