namespace ProfNet.Model.Profiling
{
	public class ProfilingResults : DocumentViewModel
	{
		private MethodInfosViewModel _methodInfos;

		public MethodInfosViewModel MethodInfos
		{
			set
			{
				_methodInfos = value;
				RaisePropertyChanged("MethodInfos");
			}
			get { return _methodInfos; }
		}

		public override string Title
		{
			get { return "Result"; }
		}
	}
}