using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;

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
	}
}