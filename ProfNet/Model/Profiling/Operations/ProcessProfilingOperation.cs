using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace ProfNet.Model.Profiling.Operations
{
	public class ProcessProfilingOperation : BaseProfilingOperation
	{
		[DllImport("ProfNet.Preformance.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
		private static extern void AttachProfiler(int pid, string targetVersion, string profilerPath, bool fConsoleMode);

		protected override bool StartProfilingInternal(IEnumerable<KeyValuePair<string, string>> environmentVariables)
		{
			ProcessExplorer processExplorer = new ProcessExplorer();
			bool? dialogResult = processExplorer.ShowDialog();
			if (dialogResult.HasValue && dialogResult.Value)
			{
				int id = processExplorer.SelectedProcess.Id;
				try
				{
					AttachProfiler(id, "v4.", Environment.CurrentDirectory + "\\ProfNet.Preformance.dll", true);
				}
				catch (Exception e)
				{
					MessageBox.Show(String.Format("Could not Attach to the {0} process. Error: {1}", id, e.Message));
				}
			}

			return true;
		}

		protected override void DetachProfilingInternal()
		{
			
		}
	}
}