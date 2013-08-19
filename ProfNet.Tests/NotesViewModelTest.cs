using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using ProfNet.Model;
using ProfNet.Model.Notes;

namespace ProfNet.Tests
{
	[TestFixture]
	public class NotesViewModelTest
	{
		[Test(Description = "Also include Save and Load testing")]
		public void DeleteSelectedNote()
		{
			Mock<INoteProvider> noteProvider = new Mock<INoteProvider>();
			noteProvider.Setup(x => x.TryLoadNotes()).Returns(new ObservableCollection<Note>());
			noteProvider.Setup(x => x.SaveNotes(It.IsAny<ObservableCollection<Note>>()));

			NotesViewModel notesView = new NotesViewModel(noteProvider.Object);
			notesView.Notes.Add(new Note{Header = "Note1"});
			notesView.Notes.Add(new Note { Header = "Note2" });
			notesView.Notes.Add(new Note { Header = "Note3" });

			notesView.SelectedNote = notesView.Notes.First();
			Assert.IsTrue(notesView.DeleteSelectedNoteCommand.CanExecute(null));
			notesView.DeleteSelectedNoteCommand.Execute(null);
			noteProvider.Verify(x => x.SaveNotes(It.IsAny<ObservableCollection<Note>>()));
			Assert.IsNotNull(notesView.SelectedNote);
			Assert.AreEqual("Note2", notesView.SelectedNote.Header);

			notesView.SelectedNote = notesView.Notes.Last();
			notesView.DeleteSelectedNoteCommand.Execute(null);
			Assert.IsNotNull(notesView.SelectedNote);
			Assert.AreEqual("Note2", notesView.SelectedNote.Header);

			notesView.DeleteSelectedNoteCommand.Execute(null);
			Assert.IsNull(notesView.SelectedNote);
			Assert.IsFalse(notesView.DeleteSelectedNoteCommand.CanExecute(null));
		}
	}
}