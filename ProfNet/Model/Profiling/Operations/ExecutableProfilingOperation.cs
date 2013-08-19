using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using ProfNet.Helpers;
using ProfNet.Model.Profiling.ProcessModel;
using ProfNet.Model.Settings;

namespace ProfNet.Model.Profiling.Operations
{
	public class ExecutableProfilingOperation : BaseProfilingOperation
	{
		private readonly IProcessProvider _processProvider;

		public ExecutableProfilingOperation(IProcessProvider provider = null)
		{
			if (provider == null)
				_processProvider = new ProcessProvider();
			else
				_processProvider = provider;
		}

		protected override bool StartProfilingInternal(IEnumerable<KeyValuePair<string, string>> environmentVariables)
		{
			string exePath = FileSystem.OpenFileDialog("Exe | *.exe");

			if (!string.IsNullOrEmpty(exePath))
			{
				try
				{
					ProcessStartInfo startInfo = new ProcessStartInfo(exePath) { UseShellExecute = false };
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
			Process process = sender as Process;
			if (process != null)
				process.Exited -= ProcessExited;

			StopProfiling();
		}
	}
}