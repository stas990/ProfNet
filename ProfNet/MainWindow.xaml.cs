using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AvalonDock.Layout.Serialization;
using ProfNet.Model;
using ProfNet.Model.Notes;

namespace ProfNet
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private const string LayoutConfigName = "Layout.config";

		public MainWindow()
		{
			InitializeComponent();
			DataContext = Workspace.Instance;
		}
		
		private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ListBox listBox = (ListBox)sender;

			UIElement elem = (UIElement)listBox.InputHitTest(e.GetPosition(listBox));
			while (elem != listBox)
			{
				ListBoxItem listBoxItem = elem as ListBoxItem;
				if (listBoxItem != null)
				{
					Note selectedItem = listBoxItem.Content as Note;

					if (selectedItem == null)
						return;

					if (!Workspace.Instance.Documents.Contains(selectedItem))
						Workspace.Instance.Documents.Add(selectedItem);
					else
						Workspace.Instance.ActiveDocument = selectedItem;
				}
				elem = (UIElement)VisualTreeHelper.GetParent(elem);
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
			using (var stream = new StreamWriter(Path.Combine(App.AppDirectory, LayoutConfigName)))
				serializer.Serialize(stream);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
			serializer.LayoutSerializationCallback += (s, args) =>
			                                          	{
															if (args.Model.Title == "Notes")
															{
																args.Content = Workspace.Instance.Notes;
															}

															if(args.Model.ContentId == Constants.NoteContentId)
															{
																args.Content = Workspace.Instance.Notes.Notes.FirstOrDefault(x => x.Header == args.Model.Title);
																if(args.Content != null)
																{
																	Workspace.Instance.Documents.Add(args.Content);
																}
															}
			                                          	};

			string configName = Path.Combine(App.AppDirectory, LayoutConfigName);

			if (File.Exists(configName))
			{
				using (var stream = new StreamReader(Path.Combine(App.AppDirectory, LayoutConfigName)))
					serializer.Deserialize(stream);
			}
		}
	}
}
