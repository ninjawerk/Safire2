using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace Safire.Core
{
    internal class Thumbnail
    {
        public void SaveThumb(string path)
        {
            if (File.Exists(path) && !File.Exists(path + ".thumb"))
            {
                try
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        int width, height;
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = fs;
                        bi.EndInit();

                        width = (int) bi.Width;
                        height = (int) bi.Height;
                        double ratio = 0;
                        if (width > height)
                        {
                            width = 170;
                            height = (int) (height*ratio);
                        }
                        else if (height > width)
                        {
                            height = 170;
                            width = (int) (width*ratio);
                        }
                        else
                        {
                            width = 170;
                            height = 170;
                        }
                        var ci = new BitmapImage();
                        ci.BeginInit();
                        ci.StreamSource = fs;
                        ci.DecodePixelWidth = width;
                        ci.DecodePixelHeight = height;
                        ci.EndInit();

                        var encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(ci));
                        using (var fss = new FileStream(path + ".thumb", FileMode.Create))
                        {
                            encoder.Save(fss);
                        }
                    }
                }
                catch
                    (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void ConvertImages()
        {
             string[] files = Directory.GetFiles(CoreMain.MyPath() + "\\My Library", "*.jpg", SearchOption.AllDirectories);
             int i = 0;
            foreach (string file in files)
            {
                 SaveThumb(file);
                i++;
             }
        }
    }
}