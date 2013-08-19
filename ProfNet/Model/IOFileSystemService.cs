
using System.IO;
using Microsoft.Win32;

namespace ProfNet.Model
{
	public class IOFileSystemService : IIOFileSystemService
	{
		public string OpenFileDialog(string filter)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog {Multiselect = false, Filter = filter};
			if (openFileDialog.ShowDialog().Value)
			{
				return openFileDialog.FileName;
			}

			return null;
		}

		public Stream OpenFileRead(string file)
		{
			return File.OpenRead(file);
		}

		public bool FileExist(string file)
		{
			return File.Exists(file);
		}

		public void DeleteDirectory(string directory, bool recur)
		{
			Directory.Delete(directory, recur);
		}

		public void CreateDirectory(string directory)
		{
			Directory.CreateDirectory(directory);
		}
	}
}