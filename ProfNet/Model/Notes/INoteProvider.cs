using System.Collections.ObjectModel;

namespace ProfNet.Model.Notes
{
	public interface INoteProvider
	{
		ObservableCollection<Note> TryLoadNotes();
		void SaveNotes(ObservableCollection<Note> notes);
	}
}