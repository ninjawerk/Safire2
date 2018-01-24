using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Safire.Controls.ContextMenus;
using Safire.Core;
using Safire.Library.ViewModels;
using Safire.Views;

namespace Safire.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
	public partial class TrackMenu : MetroWindow
    {
        public TrackMenu()
        {
            SupportSkinner.SetSkin(this);
            InitializeComponent();
	     
            SupportSkinner.SetSkin(this);
            SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;
			 
        }

        void SupportSkinner_OnSkinChanged()
        {
            SupportSkinner.SetSkin(this);
                          
        }

	    private TrackViewModel myTracky;
	    public void LoadFromTrack(TrackViewModel tracky)
	    {
		    myTracky = tracky;
		    txtTitle.Text = myTracky.Title;
			txtArtist.Text = myTracky.Artist;
			txtAlbum.Text = myTracky.Album;
			txtGenre.Text = myTracky.Genre;
			txtYear.Text = myTracky.Year.ToString();
			txtComposer.Text = myTracky.Composer;
			txtPath.Text = tracky.Path;
			txtFileType.Text = new FileInfo(tracky.Path).Extension;
			txtListens.Text = tracky.Listens.ToString();
			txtBitrate.Text = tracky.Bitrate  + "Kbps";
			txtChannels.Text = tracky.Channels.ToString();

			Thread tr = new Thread(() =>
			{
				var al = Artworks.GetAlbumArt(tracky);
 
					Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
					{				 
						imgArt.Source = al;
 
					}));

			});
			tr.Start();
	    }

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Close();
		}

		private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			myTracky.Title = txtTitle.Text;
			myTracky.Artist = txtArtist.Text;
			myTracky.Album = txtAlbum.Text;
			myTracky.Genre = txtGenre.Text;
			try
			{
				myTracky.Year = Convert.ToUInt32(txtYear.Text);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
			myTracky.Composer = txtComposer.Text;
			myTracky.SaveTrack();
			TaglibCore.SaveTrack(myTracky);
			Close();
			

		}

		private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
		{
		var r =	FingerPrint.GetTags(myTracky.Path);
			if (r != null)
			{
				txtTitle.Text = r.Title;
				txtArtist.Text = r.Artists[0].Name;
			}
		}
      
    }
}