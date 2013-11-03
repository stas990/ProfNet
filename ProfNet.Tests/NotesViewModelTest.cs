using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using ProfNet.Model;
using ProfNet.Model.Notes;
using ProfNet.Model.Settings;

namespace ProfNet.Tests
{
	[TestFixture]
	public class NotesViewModelTest
	{
		[Test(Description = "Also include Save and Load testing")]
		public void DeleteSelectedNote()
		{
			var noteProvider = new Mock<INoteProvider>();
			noteProvider.Setup(x => x.TryLoadNotes()).Returns(new ObservableCollection<Note>());
			noteProvider.Setup(x => x.SaveNotes(It.IsAny<ObservableCollection<Note>>()));

			var container = new UnityContainer();
			container.RegisterInstance(typeof(INoteProvider), noteProvider.Object);
			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

			var notesView = new NotesViewModel();
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