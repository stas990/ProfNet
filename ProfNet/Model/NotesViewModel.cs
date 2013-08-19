using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace ProfNet.Model
{
	[DataContract]
	internal class NotesViewModel : AnchorableViewModel
	{
		private readonly RelayCommand _addNewNoteCommand;
		private readonly RelayCommand _deleteNoteCommand;
		private ObservableCollection<Note> _notes;
		private Note _selectedNote;

		public NotesViewModel()
		{
			_addNewNoteCommand = new RelayCommand(AddNewNote, x => true);
			_deleteNoteCommand = new RelayCommand(DeleteSelectedNote, x => SelectedNote != null);
		}

		public override string Title
		{
			get { return "Notes"; }
		}
		
		[DataMember]
		public ObservableCollection<Note> Notes
		{
			get
			{
				if(_notes == null)
					_notes = TryLoad();

				return _notes;
			}
		}

		public Note SelectedNote
		{
			get { return _selectedNote; }
			set
			{
				_selectedNote = value;
				RaisePropertyChanged("SelectedNote");
			}
		}

		public RelayCommand NewNoteCommand
		{
			get { return _addNewNoteCommand; }
		}

		public RelayCommand DeleteSelectedNoteCommand
		{
			get { return _deleteNoteCommand; }
		}

		private void AddNewNote(object arg)
		{
			NewNoteWindow newNoteWindow = new NewNoteWindow();
			bool? dialogResult = newNoteWindow.ShowDialog();
			if(dialogResult.HasValue && dialogResult.Value)
				Notes.Add(new Note{Header = newNoteWindow.NoteName});
		}

		private void DeleteSelectedNote(object arg)
		{
			int index = Notes.IndexOf(SelectedNote);

			if (Notes.Count > 1)
			{
				Note nextSelectedNote;
				if (index == 0)
					nextSelectedNote = Notes[1];
				else if (index == Notes.Count - 1)
					nextSelectedNote = Notes[index - 1];
				else
					nextSelectedNote = Notes[index+1];

				Notes.Remove(SelectedNote);
				SelectedNote = nextSelectedNote;
			}
			else
			{
				Notes.Remove(SelectedNote);
				SelectedNote = null;
			}

		}

		private void Save()
		{
			string notesFile = Path.Combine(App.AppDirectory, "Notes.json");
			File.WriteAllText(notesFile, JsonSerializer.SerializeToString(_notes));
		}

		private ObservableCollection<Note> TryLoad()
		{
			ObservableCollection<Note> result;

			string notesFile = Path.Combine(App.AppDirectory, "Notes.json");
			if(File.Exists(notesFile))
				result = JsonSerializer.DeserializeFromString<ObservableCollection<Note>>(File.ReadAllText(notesFile));
			else
				result = new ObservableCollection<Note>();

			foreach (Note note in result)
				note.PropertyChanged += NotePropertyChanged;

			result.CollectionChanged += ResultCollectionChanged;

			return result;
		}

		private void NotePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Save();
		}

		private void ResultCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var oldItem in e.OldItems)
				{
					Note removedNote = oldItem as Note;

					if (removedNote != null)
						removedNote.PropertyChanged -= NotePropertyChanged;
				}
				Save();
			}

			if(e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var oldItem in e.NewItems)
				{
					Note removedNote = oldItem as Note;

					if (removedNote != null)
						removedNote.PropertyChanged += NotePropertyChanged;
				}
				Save();
			}
		}
	}
}