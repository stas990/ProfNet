using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;

namespace ProfNet.Model.Settings
{
	[DataContract]
	public class CommonSettings : BaseSetting
	{
		private const string MissingTempExceptionMessage = "Could not found temp folder. Please set it in Tools->Options...";

		private string _tmpFolder;
		private readonly RelayCommand _selectFolder;
		private readonly IEnvironmentProvider _environmentProvider;

		public CommonSettings()
		{
			_selectFolder = new RelayCommand(SelectFolder, x => true);
			_environmentProvider = ServiceLocator.Current.GetInstance<IEnvironmentProvider>();
		}

		public RelayCommand SelectFolderCommand
		{
			get { return _selectFolder; }
		}

		[DataMember]
		public string TmpFolder
		{
			set
			{
				_tmpFolder = value;
				RaisePropertyChanged("TmpFolder");
			}
			get
			{
				if(string.IsNullOrEmpty(_tmpFolder))
				{
					string[] logicalDrives = _environmentProvider.GetLogicalDrives();

					if (logicalDrives.Any(x => x.Equals("c:\\", StringComparison.InvariantCultureIgnoreCase)))
					{
						_tmpFolder = "c:\\ProfNet_Temp";
					}
					else
					{
						string tmpPath = _environmentProvider.GetTempFolder();

						if (string.IsNullOrEmpty(tmpPath))
						{
							if(!ValidationMessages.Contains(MissingTempExceptionMessage))
								ValidationMessages.Add(MissingTempExceptionMessage);
						}
						else 
							_tmpFolder = Path.Combine(tmpPath, "ProfNet_Temp");
					}
				}
				return _tmpFolder;
			}
		}

		private bool _clearTmpFolder;

		[DataMember]
		public bool ClearTmpFolder
		{
			set
			{
				_clearTmpFolder = value;
				RaisePropertyChanged("ClearTmpFolder");
			}
			get { return _clearTmpFolder; }
		}

		public override string Header
		{
			get { return "Environment"; }
		}

		internal override void RefreshStatus()
		{
			if (!string.IsNullOrEmpty(TmpFolder))
				ValidationMessages.Remove(MissingTempExceptionMessage);
		}

		private void SelectFolder(object arg)
		{
			using (var browserDialog = new FolderBrowserDialog())
			{
				browserDialog.SelectedPath = TmpFolder;
				if(browserDialog.ShowDialog() == DialogResult.OK)
				{
					TmpFolder = browserDialog.SelectedPath;
				}
			}
		}
	}
}