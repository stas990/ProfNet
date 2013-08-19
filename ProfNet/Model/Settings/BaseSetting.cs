using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Practices.Prism.ViewModel;

namespace ProfNet.Model.Settings
{
	public abstract class BaseSetting : NotificationObject
	{
		protected BaseSetting()
		{
			ValidationMessages = new List<string>();
		}

		public bool IsValidSetting
		{
			get { return ValidationMessages.Count == 0; }
		}

		public List<string> ValidationMessages { private set; get; }
		public abstract string Header { get; }

		internal virtual void RefreshStatus()
		{

		}
	}
}