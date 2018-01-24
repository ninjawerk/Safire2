using System;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using System.Xml;
using Safire.Library.Imaging;
using System.Web;
namespace Safire.Core
{
    class ImageDownloader
    {
        public static BitmapImage RequestLastfm(string album, string artist, string method)
        {

            string albumImagePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                    @"\Safire\AlbumData\" + album + " - " + artist + ".jpg";
            string artistImagePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                      @"\Safire\ArtistData\" + artist + ".jpg";
            String urlString = "http://ws.audioscrobbler.com/2.0/?method=" + method +
                               "&api_key=abe2eca36a6bdf607e069023dc838694";
            urlString += "&artist=" + System.Web.HttpUtility.UrlEncode(artist);
            if (method.Equals("album.getinfo"))
            {
                urlString += "&album=" + System.Web.HttpUtility.UrlEncode(album);
                if (File.Exists(albumImagePath)) return Bitmap.GetImage(albumImagePath);
            }
            else
            {
                if (File.Exists(artistImagePath)) return Bitmap.GetImage(artistImagePath);
            }
            try
            {
                WebRequest request = WebRequest.Create(urlString);
                request.Timeout = 5000;

                using (WebResponse response = request.GetResponse())
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    // Blah blah...
                    string UR = "";
                    bool imaged = false;

                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                while (reader.MoveToNextAttribute()) // Read the attributes.
                                    if (reader.Name == "size" && reader.Value == "mega") imaged = true;
                                break;
                            case XmlNodeType.Text: //Display the text in each element.
                                bool com;
                                if (imaged)
                                {
                                    UR = reader.Value;
                                    imaged = false;
                                    if (method == "album.getinfo")
                                    {
                                        return DownloadRemoteImageFile(UR, albumImagePath);
                                    }
                                    else
                                    {
                                        return DownloadRemoteImageFile(UR, artistImagePath);
                                    }

                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        private static BitmapImage DownloadRemoteImageFile(string uri, string fileName)
        {
            if (uri == null) return null;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            var response = (HttpWebResponse)request.GetResponse();

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                 response.StatusCode == HttpStatusCode.Moved ||
                 response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                // if the remote file was found, download oit
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    var buffer = new byte[4096];
                    int bytesRead = 0;
                    do
                    {
                        if (inputStream != null) bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
            }
            return Bitmap.GetImage(fileName);
            //opt.SaveThumb(fileName);


        }
    }
}
