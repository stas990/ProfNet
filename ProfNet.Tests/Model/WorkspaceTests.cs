using NUnit.Framework;
using ProfNet.Model;

namespace ProfNet.Tests.Model
{
	[TestFixture]
	public class WorkspaceTests
	{
		[TestFixtureTearDown]
		public void TearDown()
		{
			Workspace.Instance.Documents.Clear();
			Workspace.Instance.ActiveDocument = null;
		}

		[Test]
		public void CommandsTest()
		{
			Assert.IsTrue(Workspace.Instance.ShowOptionCommand.CanExecute(null));
			Workspace.Instance.ShowOptionCommand.Execute(null);
			Assert.IsNotNull(Workspace.Instance.ActiveDocument);
			Assert.AreEqual(1, Workspace.Instance.Documents.Count);


		}
	}
}