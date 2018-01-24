using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Safire.Library.Imaging
{
	public class ImageConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				var bi = new BitmapImage();
				bi.BeginInit();
				if (value.ToString() != "")
				{
					bi.UriSource = new Uri(value.ToString());
					bi.DecodePixelHeight = 100;
							 bi.CacheOption=BitmapCacheOption.OnDemand;
					
					bi.EndInit();
					return bi;
				}
				return null;
			}
		 
				return null;
			 
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

	 
	}

	public class AlbumImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				var bi = new BitmapImage();
				bi.BeginInit();
				if (value.ToString() != "")
				{
					bi.UriSource = new Uri(value.ToString());
					bi.DecodePixelHeight = 100;
					bi.CacheOption = BitmapCacheOption.OnDemand;

					bi.EndInit();
					return bi;
				}
				return null;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
