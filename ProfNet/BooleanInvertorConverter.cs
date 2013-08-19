using System;
using System.Globalization;
using System.Windows.Data;

namespace ProfNet
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class BooleanInvertorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(targetType == typeof(bool))
			{
				return !(bool) value;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}