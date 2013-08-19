using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using ServiceStack.Text;

namespace ProfNet.Model.Settings
{
	[DataContract]
	public class SettingsObject : DocumentViewModel
	{
		private static readonly string _settingsFile = "";
		private static SettingsObject _instance;
		private static readonly object _instanceLock = new object();

		private PerformanceSettings _performance;
		private CommonSettings _common;

		static SettingsObject()
		{
			if(App.AppDirectory != null)
				_settingsFile = Path.Combine(App.AppDirectory, "Settings.json");

			_settingsFile = "Settings.json";
		}

		private SettingsObject()
		{
			Common = new CommonSettings();
			Performance = new PerformanceSettings();
		}

		public static SettingsObject Instance
		{
			get
			{
				if(_instance == null)
				{
					lock (_instanceLock)
					{
						if(_instance == null)
						{
							SettingsObject instance = Load();
							Thread.MemoryBarrier();
							_instance = instance;
						}
					}
				}

				return _instance;
			}
		}

		[DataMember]
		public CommonSettings Common
		{
			set
			{
				_common = value;
				_common.PropertyChanged += Common_PropertyChanged;
			}
			get { return _common; }
		}

		[DataMember]
		public PerformanceSettings Performance
		{
			set
			{
				_performance = value;
				_performance.PropertyChanged += Common_PropertyChanged;
			}
			get { return _performance; }
		}

		public override string Title
		{
			get { return "Options"; }
		}

		internal static SettingsObject Load()
		{
			if(!File.Exists(_settingsFile))
				return new SettingsObject();

			return JsonSerializer.DeserializeFromString<SettingsObject>(File.ReadAllText(_settingsFile));
		}

		internal static void Save()
		{
			String result = JsonSerializer.SerializeToString(_instance);

			File.WriteAllText(_settingsFile, result);

			Workspace.Instance.Status.Status = "Saved";
		}

		private void Common_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Save();
		}
	}
}