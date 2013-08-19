using System;
using System.Timers;

namespace ProfNet.Model
{
	public class AutoResetStatus
	{
		private string _status;
		private readonly Timer _timer;
		internal const int ResetTime = 2000;

		public AutoResetStatus()
		{
			_timer = new Timer(ResetTime);
			_timer.Elapsed += (sender, e) =>
			                  	{
			                  		_status = null;
									RaiseStatusChanged();
									_timer.Stop();
			                  	};
		}

		public event Action StatusChanged;

		public string Status
		{
			set
			{
				_status = value;
				RaiseStatusChanged();
				_timer.Stop();
				_timer.Start();
			}
			get { return _status; }
		}

		public override string ToString()
		{
			return Status;
		}

		private void RaiseStatusChanged()
		{
			if (StatusChanged != null)
				StatusChanged();
		}
	}
}