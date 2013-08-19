using System;

namespace ProfNet.Model.Settings
{
	internal class EnvironmentProvider : IEnvironmentProvider
	{
		public string GetTempFolder()
		{
			string tmpPath = Environment.GetEnvironmentVariable("tmp");

			if (string.IsNullOrEmpty(tmpPath))
				tmpPath = Environment.GetEnvironmentVariable("temp");

			return tmpPath;
		}

		public string[] GetLogicalDrives()
		{
			return Environment.GetLogicalDrives();
		}
	}
}