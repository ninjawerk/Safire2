using System;
using System.IO;
using System.Windows.Media;
using Safire.Core;
using Safire.Library.Core;
using Safire.Library.ViewModels;
using Bitmap = Safire.Library.Imaging.Bitmap;

namespace Safire.Views
{
    class Artworks
    {
        public static ImageSource GetAlbumArt()
        {
            if (CoreMain.CurrentTrack.Artist == "Unknown Artist" | CoreMain.CurrentTrack.Album == "Unknown Album"  ) Bitmap.GetImage("resources\\unknown.png");
            ImageSource ims = Bitmap.GetImage(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                        @"\Safire\AlbumData\" + CoreMain.CurrentTrack.Album + " - " + CoreMain.CurrentTrack.Artist + ".jpg"); ;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                        @"\Safire\AlbumData\" + CoreMain.CurrentTrack.Album + " - " + CoreMain.CurrentTrack.Artist + ".jpg"))
            {
                return ims;
            }
            else
            {
                TaglibCore tgl = new TaglibCore();
                tgl.loadfile(CoreMain.CurrentTrack.Path);
                var p = tgl.getArtFrame();
                if (p != null)
                    return p;
                else
                {
                    if (Properties.Settings.Default.GetAlbumArt)
                    {
                        var t = ImageDownloader.RequestLastfm(CoreMain.CurrentTrack.Album, CoreMain.CurrentTrack.Artist,
                                        "album.getinfo");
                        if (t != null) return t;
                    }

					return Bitmap.GetImage(Main.MyPath() + "\\resources\\unknown.png");
                }



            }


        }
		public static ImageSource GetAlbumArt(TrackViewModel tracky)
		{
			if (tracky.Artist == "Unknown Artist" | tracky.Album == "Unknown Album") Bitmap.GetImage("resources\\unknown.png");
			ImageSource ims = Bitmap.GetImage(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
										@"\Safire\AlbumData\" + tracky.Album + " - " + tracky.Artist + ".jpg"); ;
			if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
										@"\Safire\AlbumData\" + tracky.Album + " - " + tracky.Artist + ".jpg"))
			{
				return ims;
			}
			else
			{
				TaglibCore tgl = new TaglibCore();
				tgl.loadfile(tracky.Path);
				var p = tgl.getArtFrame();
				if (p != null)
					return p;
				else
				{
					if (Properties.Settings.Default.GetAlbumArt)
					{
						var t = ImageDownloader.RequestLastfm(tracky.Album, tracky.Artist,
										"album.getinfo");
						if (t != null) return t;
					}

					return Bitmap.GetImage(Main.MyPath() + "\\resources\\unknown.png");
				}



			}


		}
        public static ImageSource GetArtistArt()
        {
            if (CoreMain.CurrentTrack.Artist == "Unknown Artist") return null;
            ImageSource ims = Bitmap.GetImage(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                        @"\Safire\ArtistData\" + CoreMain.CurrentTrack.Artist + ".jpg"); ;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                            @"\Safire\ArtistData\" + CoreMain.CurrentTrack.Artist + ".jpg"))
            {
                return ims;
            }
            else
            {
                if(Properties.Settings.Default.GetArtistImages)
                {
                    var t = ImageDownloader.RequestLastfm(CoreMain.CurrentTrack.Album, CoreMain.CurrentTrack.Artist,
                        "artist.getinfo");
                    if (t != null) return t;
                }
                TaglibCore tgl = new TaglibCore();
                tgl.loadfile(CoreMain.CurrentTrack.Path);
                return tgl.getArtFrame();

            }


        }
    }
}
