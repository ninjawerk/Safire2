using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Kornea.Audio;
using Kornea.Blink;
using MahApps.Metro.Controls;
using Safire.Core;
using Safire.Library.Core;
using Bitmap = Safire.Library.Imaging.Bitmap;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Safire.GUIs.Blinkies
{
    public class StandByFSBlinky : Renderer
    {
        private readonly DispatcherTimer dpt = new DispatcherTimer();
        private bool activity;
        private ImageSource img;

        public StandByFSBlinky(BlinkGrid blinkGrid)
            : base(blinkGrid)
        {
            var mw = Application.Current.MainWindow as MainWindow;
            Mouse.AddMouseMoveHandler(mw, MainWindowMouseMove);
			(mw.Flyouts.Items[0] as Flyout).IsOpenChanged += StandByFSBlinky_IsOpenChanged;
            dpt.Tick += dpt_Tick;
            dpt.Interval = new TimeSpan(0, 0, 0, 15);

            dpt.Start();
            Player.Instance.PropertyChanged += Instance_PropertyChanged;
         }

		void StandByFSBlinky_IsOpenChanged(object sender, EventArgs e)
		{
			var flyout = mw.Flyouts.Items[0] as Flyout;
			wasOpened = flyout.IsOpen;
		}

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if(e.PropertyName=="ActiveStreamHandle")
            BlinkG.InvalidateVisual();
        }

        private bool wasOpened = false;
        private void dpt_Tick(object sender, EventArgs e)
        {
            if (activity == false)
            {
               if(!Player.Instance.NetStreamingConfigsLoaded) BlinkG.Transit_In();
                mw.ResizeMode = ResizeMode.NoResize;
                mw.ShowWindowCommands = Visibility.Hidden;
                var flyout = mw.Flyouts.Items[0] as Flyout;
                wasOpened = flyout.IsOpen;
                if (wasOpened) mw.HidePlaylist();
            }
            else
            {
                BlinkG.Transit_Out();
                mw.ShowWindowCommands = Visibility.Visible;
                if (wasOpened) 
					mw.ShowPlaylist();
            }
            activity = false;
        }

        private Point mPoint = new Point();
        MainWindow mw = App.Current.MainWindow as MainWindow;
        private void MainWindowMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mPoint != Mouse.GetPosition(mw) && activity == false)
            {

                activity = true;
                BlinkG.Transit_Out();
                mw.ResizeMode = ResizeMode.CanResize;
                if (wasOpened) mw.ShowPlaylist();
                mw.ShowWindowCommands = Visibility.Visible;
            }
            mPoint = Mouse.GetPosition(mw);
        }
 
        public override void Draw(DrawingContext dc)
        {

            if (Player.Instance.Wave == null) return;

			var imb = new ImageBrush(Bitmap.GetImage(Main.MyPath() + "\\dbak.ee"));
            imb.Stretch = Stretch.UniformToFill;
            dc.DrawRectangle(imb, new Pen(null, 0), new Rect(0, 0, BlinkG.ActualWidth, BlinkG.ActualHeight));


            double w = (BlinkG.ActualWidth / 2) - 50 - (200);
            double h = (BlinkG.ActualHeight / 2) - 100;

            //transparent rect
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(90, 0, 0, 0)),
                new Pen(new SolidColorBrush(Color.FromArgb(10, 0, 0, 0)), 0),
                new Rect(0, h, BlinkG.ActualWidth, 200));

            //Image
            ImageSource img1 = mw.AlbArt.Source;
            if (img1 == null) img1 = mw.ArtistArt.Source;

            var imr = new Rect(new Point(w, h), new Size(200, 200));
            dc.DrawImage(img1, imr);

            var tpfs = mw.FontFamily;
            var tpfu = tpfs.GetTypefaces().First();
            var tpf = new Typeface("Segoe UI Light");
           
            
                           
             //title
	        if (Core.CoreMain.CurrentTrack != null && Core.CoreMain.CurrentTrack.Title != null && !Player.Instance.NetStreamingConfigsLoaded)
	        {
		        var ft = new FormattedText(Core.CoreMain.CurrentTrack.Title,
			        CultureInfo.GetCultureInfo("en-us"),
			        FlowDirection.LeftToRight, tpf, 45, Brushes.White);
           
		        ft.Trimming =TextTrimming.CharacterEllipsis;
		        ft.MaxTextWidth = (base.BlinkG.ActualWidth/2) - 50;
		        ft.MaxTextHeight = 65;
		        dc.DrawText(ft,
			        new Point((base.BlinkG.ActualWidth) / 2, (base.BlinkG.ActualHeight) / 2 - 80));

		        //artist
		        ft = new FormattedText(Core.CoreMain.CurrentTrack.Artist,
			        CultureInfo.GetCultureInfo("en-us"),
			        FlowDirection.LeftToRight, tpf, 25, Brushes.White);
		        ft.Trimming = TextTrimming.CharacterEllipsis;
		        ft.MaxTextWidth = (base.BlinkG.ActualWidth / 2) - 50;
		        ft.MaxTextHeight = 50;
		        dc.DrawText(ft,
			        new Point((base.BlinkG.ActualWidth) / 2 + 5, (base.BlinkG.ActualHeight) / 2 - 30));

		        //album
		        ft = new FormattedText(Core.CoreMain.CurrentTrack.Album,
			        CultureInfo.GetCultureInfo("en-us"),
			        FlowDirection.LeftToRight, tpf, 16, Brushes.White);
		        ft.Trimming = TextTrimming.CharacterEllipsis;
		        ft.MaxTextWidth = (base.BlinkG.ActualWidth / 2) - 50;
		        ft.MaxTextHeight = 50;
		        dc.DrawText(ft,
			        new Point((base.BlinkG.ActualWidth) / 2 + 5, (base.BlinkG.ActualHeight) / 2 + 4));

		        //user
		        if (!string.IsNullOrEmpty(LastFm.GetRealName()) && Properties.Settings.Default.Scrobble)
		        {
			        ft = new FormattedText("\uef55 " ,
				        CultureInfo.InvariantCulture,
				        FlowDirection.LeftToRight, tpfu, 14, Brushes.White);
			        ft.Trimming = TextTrimming.CharacterEllipsis;
			        ft.MaxTextWidth = (base.BlinkG.ActualWidth / 2) - 50;
			        ft.MaxTextHeight = 50;
			        dc.DrawText(ft,
				        new Point((base.BlinkG.ActualWidth) / 2 + 5, (base.BlinkG.ActualHeight) / 2 + 82));
			        ft = new FormattedText( LastFm.GetRealName(),
				        CultureInfo.InvariantCulture,
				        FlowDirection.LeftToRight, tpf, 12, Brushes.White);
			        ft.Trimming = TextTrimming.CharacterEllipsis;
			        ft.MaxTextWidth = (base.BlinkG.ActualWidth / 2) - 50;
			        ft.MaxTextHeight = 50;
			        dc.DrawText(ft,
				        new Point((base.BlinkG.ActualWidth) / 2 + 25, (base.BlinkG.ActualHeight) / 2 + 80));
		        }
	        }
	        if (Player.Instance.NetStreamingConfigsLoaded && CoreMain.CurrentNetStreamTrack != null)
	        {
				var ft = new FormattedText(Core.CoreMain.CurrentNetStreamTrack.Title,
				   CultureInfo.GetCultureInfo("en-us"),
				   FlowDirection.LeftToRight, tpf, 45, Brushes.White);

				ft.Trimming = TextTrimming.CharacterEllipsis;
				ft.MaxTextWidth = (base.BlinkG.ActualWidth / 2) - 50;
				ft.MaxTextHeight = 65;
				dc.DrawText(ft,
					new Point((base.BlinkG.ActualWidth) / 2, (base.BlinkG.ActualHeight) / 2 - 80));

				//artist
				ft = new FormattedText(((!String.IsNullOrEmpty(CoreMain.CurrentNetStreamTrack.Artist) ? CoreMain.CurrentNetStreamTrack.Artist : CoreMain.CurrentNetStreamTrack.Bitrate.ToString())),
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight, tpf, 25, Brushes.White);
				ft.Trimming = TextTrimming.CharacterEllipsis;
				ft.MaxTextWidth = (base.BlinkG.ActualWidth / 2) - 50;
				ft.MaxTextHeight = 50;
				dc.DrawText(ft,
					new Point((base.BlinkG.ActualWidth) / 2 + 5, (base.BlinkG.ActualHeight) / 2 - 30));

				//album
				ft = new FormattedText("Radio",
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight, tpf, 16, Brushes.White);
				ft.Trimming = TextTrimming.CharacterEllipsis;
				ft.MaxTextWidth = (base.BlinkG.ActualWidth / 2) - 50;
				ft.MaxTextHeight = 50;
				dc.DrawText(ft,
					new Point((base.BlinkG.ActualWidth) / 2 + 5, (base.BlinkG.ActualHeight) / 2 + 4));
	        }
        }
    }
}