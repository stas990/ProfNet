namespace ProfNet.Model
{
	public abstract class AnchorableViewModel : ViewModelBase
	{
		private bool _isVisible;
		public bool IsVisible
		{
			set
			{
				_isVisible = value;
				RaisePropertyChanged("IsVisible");
			}
			get { return _isVisible; }
		}
	}
}