using System.IO;

namespace ProfNet.Model
{
	public interface IIOFileSystemService
	{
		//File
		string OpenFileDialog(string filter);
		Stream OpenFileRead(string file);
		bool FileExist(string file);

		//Directory
		void DeleteDirectory(string directory, bool recur);
		void CreateDirectory(string directory);
	}
}