using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using ProfNet.Model.Profiling.ProcessModel;

namespace ProfNet.Model.Profiling.Operations
{
	public class ExecutableProfilingOperation : BaseProfilingOperation
	{
		private readonly IProcessProvider _processProvider;

		public ExecutableProfilingOperation()
		{
			_processProvider = ServiceLocator.Current.GetInstance<IProcessProvider>();
		}

		protected override bool StartProfilingInternal(IEnumerable<KeyValuePair<string, string>> environmentVariables)
		{
			string exePath = FileSystem.OpenFileDialog("Exe | *.exe");

			if (!string.IsNullOrEmpty(exePath))
			{
				try
				{
					var startInfo = new ProcessStartInfo(exePath) { UseShellExecute = false };
					foreach (KeyValuePair<string, string> environmentVariable in environmentVariables)
						startInfo.EnvironmentVariables.Add(environmentVariable.Key, environmentVariable.Value);

					IProcess process = _processProvider.Start(startInfo);

					process.EnableRaisingEvents = true;
					process.Exited += ProcessExited;
				}
				catch (Exception ex)
				{
					MessageBox.Show(String.Format("Could not start process: {0}. Exception: {1}", exePath, ex));
					return false;
				}

				return true;
			}

			return false;
		}

		protected override void DetachProfilingInternal()
		{
			if (ProcessId != 0)
			{
				IProcess process = _processProvider.GetProcessById(ProcessId);
				if (!process.HasExited)
				{
					process.Kill();
					process.Dispose();
				}
			}
		}

		private void ProcessExited(object sender, EventArgs e)
		{
			var process = sender as Process;
			if (process != null)
				process.Exited -= ProcessExited;

			StopProfiling();
		}
	}
}