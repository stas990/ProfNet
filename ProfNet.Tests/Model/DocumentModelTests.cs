using NUnit.Framework;
using ProfNet.Model;
using ProfNet.Model.Notes;

namespace ProfNet.Tests.Model
{
	[TestFixture]
	public class DocumentModelTests
	{
		[Test(Description = "Test basic functions")]
		public void Test()
		{
			Note note = new Note();
			Workspace.Instance.ActiveDocument = note;
			Workspace.Instance.Documents.Add(note);

			Assert.IsTrue(note.CloseDocumentCommand.CanExecute(null));
			note.CloseDocumentCommand.Execute(null);

			Assert.IsNull(Workspace.Instance.ActiveDocument);
			Assert.AreEqual(0, Workspace.Instance.Documents.Count);
		}
	}
}