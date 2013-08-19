namespace ProfNet.Model.Settings
{
	public interface IEnvironmentProvider
	{
		string GetTempFolder();

		string[] GetLogicalDrives();
	}
}