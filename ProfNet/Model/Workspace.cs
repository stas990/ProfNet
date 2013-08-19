using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ProfNet.Helpers;
using ProfNet.Model.Profiling;
using ProfNet.Model.Profiling.Operations;
using ProfNet.Model.Settings;
using ProfNet.Model.Notes;

namespace ProfNet.Model
{
	class Workspace : INotifyPropertyChanged
	{
		private NotesViewModel _notes;
		private object _activeDocument;
		private AutoResetStatus _status;
		private readonly Dispatcher _dispatcher;
		
		private readonly RelayCommand _registerProfilerCommand;
		private readonly RelayCommand _showOptionsCommand;
		private readonly RelayCommand _startProfilingCommand;
		private readonly RelayCommand _stopProfilingCommand;

		private static Workspace _instance;
		private static readonly object _instanceLock = new object();

		public static Workspace Instance
		{
			get
			{
				if(_instance == null)
				{
					lock (_instanceLock)
					{
						if(_instance == null)
						{
							Workspace instance = new Workspace();
							Thread.MemoryBarrier();
							_instance = instance;
						}
					}
				}
				return _instance;
			}
		}

		private Workspace()
		{
			_registerProfilerCommand = new RelayCommand(RegisterProfiler);
			_showOptionsCommand = new RelayCommand(ShowOptions);
			_startProfilingCommand = new RelayCommand(StartProfiling, x => ProfilingProcess == null);
			_stopProfilingCommand = new RelayCommand(StopProfiling, x => ProfilingProcess != null);

			Documents = new ObservableCollection<object>();

			Status = new AutoResetStatus();
			Status.StatusChanged += () => RaisePropertyChanged("Status");

			if (Application.Current != null)
				_dispatcher = Application.Current.Dispatcher;
			else
				_dispatcher = Dispatcher.CurrentDispatcher;
		}

		public RelayCommand RegisterProfilerCommand { get { return _registerProfilerCommand; } }

		public RelayCommand ShowOptionCommand { get { return _showOptionsCommand; } }

		public RelayCommand StartProfilingCommand { get { return _startProfilingCommand; } }

		public RelayCommand StopProfilingCommand { get { return _stopProfilingCommand; } }

		public object ActiveDocument
		{
			set
			{
				_activeDocument = value;
				RaisePropertyChanged("ActiveDocument");
			}
			get { return _activeDocument; }
		}

		public ObservableCollection<object> Documents { private set; get; }

		public IEnumerable<AnchorableViewModel> Tools
		{
			get
			{
				return new AnchorableViewModel[]{_notes};
			}
		}

		public NotesViewModel Notes
		{
			get
			{
				if (_notes == null)
					_notes = new NotesViewModel();
				return _notes;
			}
		}

		public AutoResetStatus Status
		{
			set
			{
				_status = value;
				RaisePropertyChanged("Status");
			}
			get { return _status; }
		}

		public BaseProfilingOperation ProfilingProcess { set; get; }
		
		public event PropertyChangedEventHandler PropertyChanged = delegate {};

		public void CloseDocument(object document)
		{
			if (Documents.Contains(document))
				Documents.Remove(document);

			if (ActiveDocument == document)
				ActiveDocument = null;
		}

		public NetFrameworkRuntime SelectedRuntime { set; get; }

		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		private void StartProfiling(object arg)
		{
			ProfilingOperations profiling = (ProfilingOperations)arg;

			switch (profiling)
			{
				case ProfilingOperations.Executable:
					ProfilingProcess = new ExecutableProfilingOperation();
					break;

				case ProfilingOperations.Process:
					ProfilingProcess = new ProcessProfilingOperation();
					break;

				case ProfilingOperations.IIS:
					ProfilingProcess = new IISProfilingOperation();
					break;

				default:
					return;
			}

			RaisePropertyChanged("ProfilingProcess");
			ProfilingProcess.ProfilingFinished += ProfilingProcessProfilingFinished;
			ProfilingProcess.StartProfiling();
		}

		private void StopProfiling(object arg)
		{
			ProfilingProcess.DetachProfiler();
		}

		private void RegisterProfiler(object arg)
		{
			ProfilerRegister.RegisterProfiler();
		}

		private void ShowOptions(object arg)
		{
			SettingsObject settings = Documents.FirstOrDefault(x => x is SettingsObject) as SettingsObject;

			if(settings == null)
				Documents.Add(SettingsObject.Instance);

			ActiveDocument = SettingsObject.Instance;
		}

		private void ProfilingProcessProfilingFinished()
		{
			if (ProfilingProcess.Results.MethodInfos != null)
			{
				Action<object> add = Documents.Add;
				_dispatcher.Invoke(add, ProfilingProcess.Results);
				RaisePropertyChanged("Documents");
				ActiveDocument = ProfilingProcess.Results;
			}

			ProfilingProcess.ProfilingFinished -= ProfilingProcessProfilingFinished;
			ProfilingProcess = null;
		}
	}
}