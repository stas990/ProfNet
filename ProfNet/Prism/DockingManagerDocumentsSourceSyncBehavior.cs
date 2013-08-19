using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AvalonDock;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;

namespace ProfNet.Prism
{
	public class DockingManagerDocumentsSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
	{
		public static readonly string BehaviorKey = "DockingManagerDocumentsSourceSyncBehavior";

		private readonly ObservableCollection<object> _documents = new ObservableCollection<object>();
		private DockingManager _dockingManager;
		private ReadOnlyObservableCollection<object> _readonlyDocumentsList;
		private bool _updatingActiveViewsInManagerActiveContentChanged;

		[NotNull]
		public ReadOnlyObservableCollection<object> Documents
		{
			get
			{
				if (_readonlyDocumentsList == null)
					_readonlyDocumentsList = new ReadOnlyObservableCollection<object>(_documents);

				return _readonlyDocumentsList;
			}
		}

		#region IHostAwareRegionBehavior Members

		public DependencyObject HostControl
		{
			get { return _dockingManager; }
			set { _dockingManager = value as DockingManager; }
		}

		#endregion

		/// <exception cref="InvalidOperationException"></exception>
		protected override void OnAttach()
		{
			bool itemsSourceIsSet = _dockingManager.DocumentsSource != null;

			if (itemsSourceIsSet)
			{
				throw new InvalidOperationException();
			}

			SynchronizeItems();

			_dockingManager.ActiveContentChanged += ManagerActiveContentChanged;
			Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
			Region.Views.CollectionChanged += Views_CollectionChanged;
		}

		private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				int startIndex = e.NewStartingIndex;
				foreach (object newItem in e.NewItems)
				{
					_documents.Insert(startIndex++, newItem);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (object oldItem in e.OldItems)
				{
					_documents.Remove(oldItem);
				}
			}
		}

		private void SynchronizeItems()
		{
			BindingOperations.SetBinding(_dockingManager, DockingManager.DocumentsSourceProperty, new Binding("Documents") {Source = this});
			foreach (object view in Region.Views)
			{
				_documents.Add(view);
			}
		}

		private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_updatingActiveViewsInManagerActiveContentChanged)
			{
				return;
			}

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (_dockingManager.ActiveContent != null
				    && _dockingManager.ActiveContent != e.NewItems[0]
				    && Region.ActiveViews.Contains(_dockingManager.ActiveContent))
				{
					Region.Deactivate(_dockingManager.ActiveContent);
				}

				_dockingManager.ActiveContent = e.NewItems[0];
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Contains(_dockingManager.ActiveContent))
			{
				_dockingManager.ActiveContent = null;
			}
		}

		private void ManagerActiveContentChanged(object sender, EventArgs e)
		{
			try
			{
				_updatingActiveViewsInManagerActiveContentChanged = true;

				if (_dockingManager == sender)
				{
					object activeContent = _dockingManager.ActiveContent;
					foreach (var item in Region.ActiveViews.Where(it => it != activeContent))
					{
						Region.Deactivate(item);
					}


					if (Region.Views.Contains(activeContent) && !Region.ActiveViews.Contains(activeContent))
					{
						Region.Activate(activeContent);
					}
				}
			}
			finally
			{
				_updatingActiveViewsInManagerActiveContentChanged = false;
			}
		}
	}
}