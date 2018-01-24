using System;
using System.Collections.ObjectModel;
using Safire.Library.ObservableCollection;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;

namespace Safire.Library
{
    public class ArtistsViewModel : ViewModelBase
    {
        private ObservableCollection<ArtistViewModel> tracks;

        public ObservableCollection<ArtistViewModel> Tracks
        {
            get { return tracks; }

            set
            {
                tracks = value;
                RaisePropertyChanged("Artists");
            }
        }

        public void NotifyChanges()
        {
            RaisePropertyChanged("Artists");
        }

        public ObservableCollection<ArtistViewModel> GetArtists()
        {
            tracks = new ObservableCollection<ArtistViewModel>();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                TableQuery<Artist> query = db.Table<Artist>().OrderBy(c => c.Name);
                foreach (Artist track in query)
                {
                    var etrack = new ArtistViewModel()
                        {

                            Duration = track.Duration,
                            Name = track.Name,
                            Image = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                      @"\Safire\ArtistData\" + track.Name + ".jpg.thumb",
                            AlbumCount = track.AlbumCount,
                            SongCount = track.SongCount,
                            Listens = track.Listens,
                            Meta = track.Meta,
                            Rate = track.Rate,
                            LastPlay = track.LastPlayed
                        };
                    tracks.Add(etrack);
                    Results.Artists.Add(etrack);
                }
            }
            return tracks;
        }
        public ObservableCollection<ArtistViewModel> GetArtists(string Artist)
        {
            tracks = new ObservableCollection<ArtistViewModel>();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                TableQuery<Artist> query = db.Table<Artist>().OrderBy(c => c.Name).Where(c=>c.Name==Artist);
                foreach (Artist track in query)
                {
                    var etrack = new ArtistViewModel()
                    {

                        Duration = track.Duration,
                        Name = track.Name,
                        Image = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                  @"\Safire\ArtistData\" + track.Name + ".jpg.thumb",
                        AlbumCount = track.AlbumCount,
                        SongCount = track.SongCount,
                        Listens = track.Listens,
                        Meta = track.Meta,
                        Rate = track.Rate,
                        LastPlay = track.LastPlayed
                    };
                    tracks.Add(etrack);
                    Results.Artists.Add(etrack);
                }
            }
            return tracks;
        }
    }
}