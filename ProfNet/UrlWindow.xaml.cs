using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProfNet
{
	/// <summary>
	/// Interaction logic for UrlWindow.xaml
	/// </summary>
	public partial class UrlWindow : Window
	{
		public UrlWindow()
		{
			InitializeComponent();
		}
	

		public string Uri { get { return NameBox.Text; } }

		private void CloseCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private void OkCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void OkCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !string.IsNullOrEmpty(NameBox.Text);
		}

		private void CloseCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NameBox.Focus();
		}
	}
}
