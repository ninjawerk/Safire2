using System;
using System.Windows.Threading;
using Kornea.Audio;
using Safire.Core;
using Safire.Library.Queries;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;

namespace Safire.Library.Core
{
    public class UpdateListens
    {
        public event EventHandler IncrementListens;

        private long ticks=0;
        private static UpdateListens _instance = null;
        private DispatcherTimer dpt = new DispatcherTimer();

        public void Dummy()
        {
        }

        public static UpdateListens Instance
        {
            get
            {
                if (_instance == null) _instance = new UpdateListens();
                return _instance;
            }
        }

        public UpdateListens()
        {
            Player.Instance.PropertyChanged += Instance_PropertyChanged;
            dpt.Tick += dpt_Tick;
            dpt.Interval = new TimeSpan(0, 0, 0, 1);
            dpt.Start();
        }

        void dpt_Tick(object sender, System.EventArgs e)
        {
            ticks++;
            if (incremented | Player.Instance.NetStreamingConfigsLoaded)return;
            if (Player.Instance.Wave != null && ticks > Player.Instance.Wave.Duration / 2
                | ticks > 4000)
            {
                incremented = true;
                EventHandler handler = IncrementListens;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
                //track update
                Track existingTrack = TrackQuery.GetTrack(CoreMain.CurrentTrack.Path);
                if (existingTrack != null)
                {
                    existingTrack.Listens ++;
                    TrackQuery.SaveTrack(existingTrack);
                }

                //artistUpdate
                ArtistViewModel existingArtist = ArtistQuery.GetArtistViewModel(CoreMain.CurrentTrack.Artist);
                if (existingArtist != null)
                {
                    existingArtist.Listens++;
                    existingArtist.SaveArtist();
                }

                //albumUpdate
                AlbumViewModel existingAlbum = AlbumQuery.GetAlbumViewModel(CoreMain.CurrentTrack.Album);
                if (existingAlbum != null)
                {
                    existingAlbum.Listens++;
                    existingAlbum.SaveAlbum();
                }

                //a;bumUpdate
                ViewModels.GenreViewModel existingGenre = GenreQuery.GetGenreViewModel(CoreMain.CurrentTrack.Genre);
                if (existingGenre != null)
                {
                    existingGenre.Listens++;
                    existingGenre.SaveGenre();
                }

                //scrobble
                
                LastFm.ScrobbleTrack(LastFm.CurrentTrack);

            }
        }

        private bool incremented = false;
        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.ToString() == "ActiveStreamHandle")
            {
                ticks = 0;
                incremented = false;
            }
        }
    }
}
