using System;
using System.IO;
using System.Windows;
using ProfNet.Logging;
using ProfNet.Model;
using ProfNet.Model.Settings;
using ProfNet.Prism;

namespace ProfNet
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		internal static string AppDirectory { private set; get; }

		public static IErrorHandler ErrorHandler { internal set; get; }

		protected override void OnStartup(StartupEventArgs e)
		{
			if(ErrorHandler == null)
				ErrorHandler = new ErrorHandler();

			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			AppDirectory = Path.Combine(folderPath, Constants.ApplicationName);

			Helpers.ProfilerRegister.RegisterProfilerIfNeeded();

			if (!Directory.Exists(AppDirectory))
				Directory.CreateDirectory(AppDirectory);

			var bootstrapper = new Bootstrapper();
			bootstrapper.Run();

			if (!string.IsNullOrEmpty(SettingsObject.Instance.Common.TmpFolder) && !Directory.Exists(SettingsObject.Instance.Common.TmpFolder))
				Directory.CreateDirectory(SettingsObject.Instance.Common.TmpFolder);

			base.OnStartup(e);
		}
	}
}
