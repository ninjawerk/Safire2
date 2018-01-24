using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Safire.Library.Imaging
{
	class ImageVisibility		 :IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && String.IsNullOrEmpty(value.ToString())) return Visibility.Hidden;
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	class IconVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return Visibility.Visible;
			if (  String.IsNullOrEmpty(value.ToString())) return Visibility.Visible;
			return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
