using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProfNet.Helpers;

namespace ProfNet
{
	/// <summary>
	/// Interaction logic for ProcessExplorer.xaml
	/// </summary>
	public partial class ProcessExplorer
	{
		private readonly RelayCommand _okCommand;
		private readonly RelayCommand _cancelCommand;
		private Task _loadProcessesTask;
		private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

		public ProcessExplorer()
		{
			_okCommand = new RelayCommand(Ok, x => MainView.SelectedItem != null);
			_cancelCommand = new RelayCommand(Cancel, x => true);
			InitializeComponent();
			DataContext = this;
		}

		public ObservableCollection<ProcessStruct> Processes { private set; get; }

		public RelayCommand OkCommand
		{
			get { return _okCommand; }
		}

		public RelayCommand CancelCommand
		{
			get { return _cancelCommand; }
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Action action = delegate
			                	{
			                		IEnumerable<ProcessStruct> tmp = Process.GetProcesses().OrderBy(x => x.ProcessName)
			                			.Select(x => new ProcessStruct
			                			             	{
			                			             		Name = x.ProcessName,
			                			             		Id = x.Id,
			                			             		FileName = GetExecutablePath(x),
			                			             		CommandLine = GetCommandLine(x)
			                			             	}).ToArray();

			                		Action<IEnumerable<ProcessStruct>> ctor =
			                			x =>
			                				{
			                					Processes = new ObservableCollection<ProcessStruct>(x);
			                					MainView.ItemsSource = Processes;
												MainView.Visibility = Visibility.Visible;
												progressBar.Visibility = Visibility.Collapsed;
			                				};

			                		Dispatcher.BeginInvoke(ctor, tmp);
			                	};

			_loadProcessesTask = Task.Factory.StartNew(action, _tokenSource.Token);
		}

		public ProcessStruct SelectedProcess { set; get; }

		private void Ok(object arg)
		{
			DialogResult = true;
			Close();
		}

		private void Cancel(object arg)
		{
			DialogResult = false;
			Close();
		}

		private static string GetCommandLine(Process process)
		{
			StringBuilder result = new StringBuilder();

			string wmiQuery = string.Format("select CommandLine from Win32_Process where Name='{0}.exe'", process.ProcessName);
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery))
			{
				using (ManagementObjectCollection retObjectCollection = searcher.Get())
				{
					foreach (ManagementObject retObject in retObjectCollection)
						result.Append(retObject["CommandLine"]);
				}
			}
			return result.ToString();
		}

		private static string GetExecutablePath(Process Process)
		{
			//If running on Vista or later use the new function
			if (Environment.OSVersion.Version.Major >= 6)
			{
				return GetExecutablePathAboveVista(Process.Id);
			}

			return Process.MainModule.FileName;
		}

		private static string GetExecutablePathAboveVista(int ProcessId)
		{
			var buffer = new StringBuilder(1024);
			IntPtr hprocess = ProcessHelper.OpenProcess(ProcessHelper.ProcessAccessFlags.QueryInformation, false, ProcessId);
			if (hprocess != IntPtr.Zero)
			{
				try
				{
					int size = buffer.Capacity;
					if (ProcessHelper.QueryFullProcessImageName(hprocess, 0, buffer, out size))
					{
						return buffer.ToString();
					}
				}
				finally
				{
					ProcessHelper.CloseHandle(hprocess);
				}
			}
			
			return string.Empty;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if(_loadProcessesTask != null && !_loadProcessesTask.IsCompleted)
				_tokenSource.Cancel(false);
		}

		public struct ProcessStruct
		{
			public string Name { set; get; }
			public int Id { set; get; }
			public string FileName { set; get; }
			public string CommandLine { set; get; }
		}
	}
}
