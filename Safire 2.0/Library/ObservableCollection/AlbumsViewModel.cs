using System.Collections.ObjectModel;
using Safire.Library.ObservableCollection;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;

namespace Safire.Library
{
    public class AlbumsViewModel : ViewModelBase
    {
        private ObservableCollection<AlbumViewModel> tracks;

        public ObservableCollection<AlbumViewModel> Tracks
        {
            get { return tracks; }

            set
            {
                tracks = value;
                RaisePropertyChanged("Albums");
            }
        }

        public void NotifyChanges()
        {
            RaisePropertyChanged("Albums");
        }

        public ObservableCollection<AlbumViewModel> GetAlbums()
        {
            
            tracks = new ObservableCollection<AlbumViewModel>();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                TableQuery<Album> query = db.Table<Album>().OrderBy(c => c.Name);
                foreach (Album track in query)
                {
                    var etrack = new AlbumViewModel()
                        {
                            Duration = track.Duration,
                            Name = track.Name,
                            Image = track.Image,
                            SongCount = track.SongCount,
                            Listens = track.Listens,
                            Meta = track.Meta,
                            Rate = track.Rate,
                            LastPlay = track.LastPlayed,
                            Artist = track.Artist,
                            ID = track.ID
                        };
                    tracks.Add(etrack);
                    
                    Results.Albums.Add(etrack);
                }
            }
            return tracks;
        }

        public ObservableCollection<AlbumViewModel> GetAlbumsFromArtist(string artist)
        {
           // if (Results.Albums != null) Results.Albums.Clear();
            tracks = new ObservableCollection<AlbumViewModel>();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                TableQuery<Album> query = db.Table<Album>().OrderBy(c => c.Name).Where(f=>f.Artist==artist);
                foreach (Album track in query)
                {
                    var etrack = new AlbumViewModel()
                    {
                        Duration = track.Duration,
                        Name = track.Name,
                        Image = track.Image,
                        SongCount = track.SongCount,
                        Listens = track.Listens,
                        Meta = track.Meta,
                        Rate = track.Rate,
                        LastPlay = track.LastPlayed,
                        Artist = track.Artist,
                        ID = track.ID
                    };
                    tracks.Add(etrack);
                 //   if (Results.Albums == null) Results.Albums = new ObservableCollection<AlbumViewModel>();
                //    Results.Albums.Add(etrack);
                }
            }
            return tracks;
        }

    }
}