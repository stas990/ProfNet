using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using ProfNet.Model;
using ProfNet.Model.Settings;

namespace ProfNet.Tests.Settings
{
	[TestFixture]
	public class CommonSettingTests
	{
		[Test]
		public void TempFolderTest()
		{
			var environmentProvider = new Mock<IEnvironmentProvider>();
			environmentProvider.Setup(x => x.GetLogicalDrives()).Returns(new[] {"C:\\", "D:\\"});
			environmentProvider.Setup(x => x.GetTempFolder()).Returns("C:\\someFolder");

			var container = new UnityContainer();
			container.RegisterInstance(typeof(IEnvironmentProvider), environmentProvider.Object);
			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

			var commonSettings = new CommonSettings();
			Assert.AreEqual("D:\\ProfNet_Temp", commonSettings.TmpFolder);

			environmentProvider.Setup(x => x.GetLogicalDrives()).Returns(new[] { "C:\\" });
			commonSettings = new CommonSettings();
			Assert.AreEqual("C:\\someFolder\\ProfNet_Temp", commonSettings.TmpFolder);

			environmentProvider.Setup(x => x.GetTempFolder()).Returns("");
			commonSettings = new CommonSettings();
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