namespace ProfNet.Model
{
	public abstract class DocumentViewModel : ViewModelBase
	{
		private RelayCommand _closeDocumentCommand;

		public RelayCommand CloseDocumentCommand
		{
			get
			{
				if (_closeDocumentCommand == null)
					_closeDocumentCommand = new RelayCommand(CloseDocument, CanCloseDocument);
				return _closeDocumentCommand;
			}
		}

		private void CloseDocument(object arg)
		{
			Workspace.Instance.CloseDocument(this);
		}

		protected virtual bool CanCloseDocument(object arg)
		{
			return true;
		}
	}
}