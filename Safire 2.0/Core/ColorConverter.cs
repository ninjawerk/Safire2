using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Safire.Library;
using Colors = Safire.Animation.Colors;

namespace Safire.Core
{
    public class IntToColorConverter : IValueConverter
    {
        public static long seed = 1;

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var mval = (string) value;

            var rand = new Random((int) seed++);

            var r = (byte) rand.Next(80);

            var g = r;
            var b = g;

           
          

            if (
                File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                            @"\Safire\AlbumData\" +
                            mval + " - " + ArtistList.Artist + ".jpg"))
            {
                try
                {
                    BitmapImage IMAGE;
                    return
                        new SolidColorBrush(
                            Colors.CalculateAverageColor(
                                new BitmapImage(
                                    new Uri(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                            @"\Safire\AlbumData\" +
                                            mval + " - " + ArtistList.Artist + ".jpg"))));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}