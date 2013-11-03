using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using ProfNet.Model;
using ProfNet.Model.Notes;
using ProfNet.Model.Profiling.ProcessModel;
using ProfNet.Model.Settings;

namespace ProfNet.Prism
{
	internal class Bootstrapper : UnityBootstrapper
	{
		protected override void InitializeShell()
		{
			base.InitializeShell();

			Application.Current.MainWindow = (Window)Shell;
			Application.Current.MainWindow.Show();
		}

		protected override DependencyObject CreateShell()
		{
			return new MainWindow();
		}

		protected override void ConfigureContainer()
		{
			Container.RegisterType<IIOFileSystemService, IOFileSystemService>();
			Container.RegisterType<IEnvironmentProvider, EnvironmentProvider>();
			Container.RegisterType<INoteProvider, NoteProvider>();
			Container.RegisterType<IProcessProvider, ProcessProvider>();
			base.ConfigureContainer();
		}
	}
}