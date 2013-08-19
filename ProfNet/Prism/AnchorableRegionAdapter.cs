using System;
using System.Collections.Specialized;
using System.Linq;
using AvalonDock.Layout;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Regions;

namespace ProfNet.Prism
{
	internal class AnchorableRegionAdapter : RegionAdapterBase<LayoutAnchorable>
	{
		public AnchorableRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
			: base(regionBehaviorFactory)
		{
		}

		/// <exception cref="ArgumentNullException">regionTarget</exception>
		/// <exception cref="InvalidOperationException"></exception>
		protected override void Adapt(IRegion region, LayoutAnchorable regionTarget)
		{
			if (regionTarget == null)
				throw new ArgumentNullException("regionTarget");

			if (regionTarget.Content != null)
			{
				throw new InvalidOperationException();
			}

			region.ActiveViews.CollectionChanged += delegate { regionTarget.Content = region.ActiveViews.FirstOrDefault(); };

			region.Views.CollectionChanged +=
				(sender, e) =>
					{
						if (e.Action == NotifyCollectionChangedAction.Add && !region.ActiveViews.Any())
						{
							region.Activate(e.NewItems[0]);
						}
					};
		}

		[NotNull]
		protected override IRegion CreateRegion()
		{
			return new SingleActiveRegion();
		}
	}
}