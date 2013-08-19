using System.Windows;
using System.Windows.Controls;
using ProfNet.Model;

namespace ProfNet
{
	public class ContainerStyleSelector : StyleSelector
	{
		public Style DocumentStyle { set; get; }

		public Style AnchorDocumentStyle { set; get; }

		public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
		{
			if (item is AnchorableViewModel)
				return AnchorDocumentStyle;

			if (item is ViewModelBase)
				return DocumentStyle;

			return base.SelectStyle(item, container);
		}
	}
}