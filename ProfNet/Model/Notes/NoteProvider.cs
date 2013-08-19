using System.Collections.ObjectModel;
using System.IO;
using ServiceStack.Text;

namespace ProfNet.Model.Notes
{
	public class NoteProvider : INoteProvider
	{
		private const string NotesFileName = "Notes.json";

		public ObservableCollection<Note> TryLoadNotes()
		{
			ObservableCollection<Note> result;

			string notesFile = Path.Combine(App.AppDirectory, NotesFileName);
			if (File.Exists(notesFile))
				result = JsonSerializer.DeserializeFromString<ObservableCollection<Note>>(File.ReadAllText(notesFile));
			else
				result = new ObservableCollection<Note>();

			return result;
		}

		public void SaveNotes(ObservableCollection<Note> notes)
		{
			string notesFile = Path.Combine(App.AppDirectory, NotesFileName);
			File.WriteAllText(notesFile, JsonSerializer.SerializeToString(notes));
		}
	}
}