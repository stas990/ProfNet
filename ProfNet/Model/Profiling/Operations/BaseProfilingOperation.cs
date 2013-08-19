using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using ProfNet.Helpers;
using ProfNet.Model.Settings;
using ServiceStack.Text;

namespace ProfNet.Model.Profiling.Operations
{
	public abstract class BaseProfilingOperation : INotifyPropertyChanged
	{
		public event Action ProfilingFinished = delegate{};
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected BaseProfilingOperation()
		{
			Results = new ProfilingResults();

			Messanger = new NamedPipeMessanger();
			Action<string> changeStatus = x => Workspace.Instance.Status.Status = x;
			Messanger.MessageRecived += x =>
			                            	{
												if (Application.Current != null)
													Application.Current.Dispatcher.Invoke(changeStatus, x);
												else
													changeStatus(x);
			                            	};
			Messanger.SpecificMessageRecived += SpecificMessageRecived;

			FileSystem = new IOFileSystemService();
		}

		public void StartProfiling()
		{
			if (SettingsObject.Instance.Common.ClearTmpFolder)
			{
				FileSystem.DeleteDirectory(SettingsObject.Instance.Common.TmpFolder, true);
				FileSystem.CreateDirectory(SettingsObject.Instance.Common.TmpFolder);
			}

			if (StartProfilingInternal(CreateEnvironmentVariables()))
				Messanger.StartListen();
			else
				StopProfiling();

		}

		public void DetachProfiler()
		{
			DetachProfilingInternal();
			StopProfiling();
		}

		protected void StopProfiling()
		{
			Messanger.StopListen();
			string resultPath = Path.Combine(WorkingDirectory, "result.txt");

			if (FileSystem.FileExist(resultPath))
			{
				using (Stream stream = FileSystem.OpenFileRead(resultPath))
					Results.MethodInfos = new MethodInfosViewModel { Methods = JsonSerializer.DeserializeFromStream<List<MethodInfo>>(stream) };
			}

			ProfilingFinished();
		}

		protected abstract bool StartProfilingInternal(IEnumerable<KeyValuePair<string,string>> environmentVariables);
		protected abstract void DetachProfilingInternal();

		public ProfilingResults Results { set; get; }

		internal IIOFileSystemService FileSystem { set; get; }

		protected string WorkingDirectory { private set; get; }

		protected NamedPipeMessanger Messanger { private set; get; }

		protected int ProcessId { private set; get; }

		protected void RaisePropertyChanged(string property)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		private void SpecificMessageRecived(NamedPipeMessanger.SpecificMessageId messageId, string messageText)
		{
			switch (messageId)
			{
				case NamedPipeMessanger.SpecificMessageId.Failed:
					StopProfiling();
					App.ErrorHandler.Handling(messageText);
					break;

				case NamedPipeMessanger.SpecificMessageId.ProcessId:
					ProcessId = int.Parse(messageText.Trim());
					break;
			}
		}

		private IEnumerable<KeyValuePair<string,string>> CreateEnvironmentVariables()
		{
			Dictionary<string,string> result = new Dictionary<string, string>();

			string timeStamp = DateTime.Now.ToString("yyMMd_HHmmss");
			string directory = Path.Combine(SettingsObject.Instance.Common.TmpFolder, timeStamp);
			FileSystem.CreateDirectory(directory);
			WorkingDirectory = directory;

			result.Add(Constants.TmpFolderName, WorkingDirectory);
			result.Add(Constants.Cor_Enable_Profiling, "1");
			result.Add(Constants.Cor_Profiler, String.Format("{{{0}}}", ProfilerRegister.ProfilerGuid));
			result.Add(Constants.Threshold, SettingsObject.Instance.Performance.Threshold.ToString(CultureInfo.InvariantCulture));
			result.Add(Constants.RuntimeVersionName, Workspace.Instance.SelectedRuntime.ToString());

			return result;
		}
	}
}