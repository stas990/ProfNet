using System.Runtime.Serialization;

namespace ProfNet.Model.Settings
{
	[DataContract]
	public class PerformanceSettings : BaseSetting
	{
		private double _threshold;

		public PerformanceSettings()
		{
			Threshold = 1;
		}

		[DataMember]
		public double Threshold
		{
			get { return _threshold; }
			set
			{
				_threshold = value;
				RaisePropertyChanged("Threshold");
			}
		}

		public override string Header
		{
			get { return "Performance"; }
		}
	}
}