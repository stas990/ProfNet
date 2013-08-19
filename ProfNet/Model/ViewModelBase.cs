using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.ViewModel;

namespace ProfNet.Model
{
	public abstract class ViewModelBase : NotificationObject
	{
		[CanBeNull]
		public abstract string Title { get; }

		[CanBeNull]
		public virtual Uri Icon { get { return null; } }

		[CanBeNull]
		public virtual string ContentId { get { return ""; } }

		[NotifyPropertyChangedInvocator]
		protected override void RaisePropertyChanged(string propertyName)
		{
			base.RaisePropertyChanged(propertyName);
		}
	}
}