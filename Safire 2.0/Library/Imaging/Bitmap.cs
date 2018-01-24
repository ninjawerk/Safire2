using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Safire.Library.Imaging
{
    class Bitmap
    {
        public static BitmapImage GetImage(string v)
        {
            var image = new BitmapImage();
            if (System.IO.File.Exists(v))
            {

                try
                {
                    byte[] buffer = System.IO.File.ReadAllBytes(v);
                    var ms = new MemoryStream(buffer);

                    image.BeginInit();
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                }
                catch (Exception)
                {
                    
                    return null;
                }
               
            }
            return image;
        }
    }
}
