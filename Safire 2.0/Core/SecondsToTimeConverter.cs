using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Safire.Core
{
  
     public class SecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            double v = System.Convert.ToDouble(value);
            return TimeSpan.FromSeconds(  v).ToString();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
