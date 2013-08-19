using Moq;
using NUnit.Framework;
using ProfNet.Model.Settings;

namespace ProfNet.Tests.Settings
{
	[TestFixture]
	public class CommonSettingTests
	{
		[Test]
		public void TempFolderTest()
		{
			Mock<IEnvironmentProvider> environmentProvider = new Mock<IEnvironmentProvider>();
			environmentProvider.Setup(x => x.GetLogicalDrives()).Returns(new[] {"C:\\", "D:\\"});
			environmentProvider.Setup(x => x.GetTempFolder()).Returns("C:\\someFolder");

			CommonSettings commonSettings = new CommonSettings(environmentProvider.Object);
			Assert.AreEqual("D:\\ProfNet_Temp", commonSettings.TmpFolder);

			environmentProvider.Setup(x => x.GetLogicalDrives()).Returns(new[] { "C:\\" });
			commonSettings = new CommonSettings(environmentProvider.Object);
			Assert.AreEqual("C:\\someFolder\\ProfNet_Temp", commonSettings.TmpFolder);

			environmentProvider.Setup(x => x.GetTempFolder()).Returns("");
			commonSettings = new CommonSettings(environmentProvider.Object);
			Assert.IsNullOrEmpty(commonSettings.TmpFolder);
			Assert.IsFalse(commonSettings.IsValidSetting);
			Assert.IsNotEmpty(commonSettings.ValidationMessages);
			Assert.IsNotEmpty(commonSettings.ValidationMessages[0]);

			commonSettings.TmpFolder = "SomeFolder";
			commonSettings.RefreshStatus();
			Assert.IsTrue(commonSettings.IsValidSetting);
			Assert.IsEmpty(commonSettings.ValidationMessages);
		}
	}
}