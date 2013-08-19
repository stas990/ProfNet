using NUnit.Framework;
using ProfNet.Model.Settings;

namespace ProfNet.Tests.Settings
{
	[TestFixture]
	public class SettingsObjectTest
	{
		[Test]
		public void SaveLoadTest()
		{
			SettingsObject.Instance.Common.ClearTmpFolder = true;
			SettingsObject settingsObject = SettingsObject.Load();
			Assert.AreEqual(true, settingsObject.Common.ClearTmpFolder);

			SettingsObject.Instance.Common.ClearTmpFolder = false;
			settingsObject = SettingsObject.Load();
			Assert.AreEqual(false, settingsObject.Common.ClearTmpFolder);
		}
	}
}