using System.Windows;

namespace ProfNet
{
	/// <summary>
	/// Interaction logic for NewNoteWindow.xaml
	/// </summary>
	public partial class NewNoteWindow : Window
	{
		public NewNoteWindow()
		{
			InitializeComponent();
		}

		public string NoteName { get { return NameBox.Text; } }

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
