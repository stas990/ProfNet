using System.Threading;
using NUnit.Framework;
using ProfNet.Model;

namespace ProfNet.Tests.Model
{
	[TestFixture]
	public class AutoresetStatusTests
	{
		[Test]
		public void StatusTest()
		{
			AutoResetStatus autoResetStatus = new AutoResetStatus {Status = "Hello111"};

			Assert.AreEqual("Hello111", autoResetStatus.ToString());
			Thread.Sleep(AutoResetStatus.ResetTime + 200);

			Assert.IsNull(autoResetStatus.Status);
		}
	}
}