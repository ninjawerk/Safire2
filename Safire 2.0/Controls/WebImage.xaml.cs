using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Safire.Core;
using Safire.Library;
using Image = System.Drawing.Image;

namespace Safire.Controls
{
    public enum ImageType
    {
        FileSystem,
        WebImage,
        Album,
        Artist
    }

    /// <summary>
    /// Interaction logic for WebImage.xaml
    /// </summary>
    public partial class WebImage : UserControl
    {
        public string BackText
        {
            get { return txt.Text  ; }
            set { txt.Text   = value; }
        }

        ImageType imgtyp=ImageType.FileSystem;
        public ImageType ImageTypel {
            get { return imgtyp; }
            set
            {
                imgtyp = value;
            switch (value)
            {
                case ImageType.FileSystem:
                    break;
                case ImageType.WebImage:
                    break;
                case ImageType.Album:
                    txt.Text = "";
                    break;
                case ImageType.Artist:
                    txt.Text = ""; 
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
            } }
        public WebImage()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty URLProperty =
    DependencyProperty.Register("URL", typeof(string), typeof(WebImage),
                                new UIPropertyMetadata("", OnURLChanged));

        /// <summary>
        /// Stretch type of the image
        /// </summary>
        public Stretch Strecth
        {
            get { return img.Stretch; }
            set { img.Stretch = value; }
        }

        public static int wd;
        /// <summary>
        /// On binding
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnURLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            url = (string)e.NewValue;
            ThreadPool.QueueUserWorkItem((d as WebImage).CallBack);
        }

        private static string url;
        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set
            {
                SetValue(URLProperty, value);
            }

        }

        /// <summary>
        /// Processing logic
        /// </summary>
        /// <param name="state"></param>
        public void CallBack(object state)
        {
            string ss;
            if (url == null) return;
            if (url.StartsWith("http")) ImageTypel = ImageType.WebImage;
            switch (ImageTypel)
            {
                case ImageType.FileSystem:
                    ss = url.Replace(@"file:///", "");
                    if (File.Exists(ss))
                    {
                        try
                        {
                            byte[] buffer = File.ReadAllBytes(ss);
                            var ms = new MemoryStream(buffer);
                            var image = new BitmapImage();
                            image.BeginInit();

                            image.DecodePixelWidth = 100;
  
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                            image.Freeze();
                            bps = image;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
                case ImageType.WebImage:
                    break;
                case ImageType.Album:
                    
                    ss = url.Replace(@"file:///", "");
                    ss = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Safire\AlbumData\" + ss + " - " + ArtistList.Artist + ".jpg";
                    if (File.Exists(ss))
                    {
                        try
                        {
                            byte[] buffer = File.ReadAllBytes(ss);
                            var ms = new MemoryStream(buffer);
                            var image = new BitmapImage();
                            image.BeginInit();

                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                            image.Freeze();
                            bps = image;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
                case ImageType.Artist:
                   
                 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (bps != null)
                {

                    img.Source = bps;
                    img.Visibility = Visibility.Visible;
                }
                else
                {
                    img.Visibility = Visibility.Visible;
                }
            })
);
        }


        private BitmapSource bps;

        public readonly BackgroundWorker bgw = new BackgroundWorker();
        private string _backText;

        System.Drawing.Image DownloadImage(string _URL)
        {
            System.Drawing.Image _tmpImage = null;


            // Open a connection
            if (_URL != "")
            {
                try
                {
                    HttpWebRequest _HttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(_URL);

                    _HttpWebRequest.AllowWriteStreamBuffering = true;

                    // You can also specify additional header values like the user agent or the referer: (Optional)
                    _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                    _HttpWebRequest.Referer = "http://www.google.com/";

                    // set timeout for 20 seconds (Optional)
                    _HttpWebRequest.Timeout = 20000;

                    // Request response:
                    WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                    // Open data stream:
                    Stream _WebStream = _WebResponse.GetResponseStream();

                    // convert webstream to image
                    _tmpImage = Image.FromStream(_WebStream);

                    // Cleanup
                    _WebResponse.Close();
                    _WebResponse.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;

                }
            }



            // Error


            return _tmpImage;
        }
    }
}