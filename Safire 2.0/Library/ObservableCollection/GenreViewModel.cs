using System.Collections.ObjectModel;
using Safire.Library.ObservableCollection;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;

namespace Safire.Library
{
    public class GenreViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModels.GenreViewModel> tracks;

        public ObservableCollection<ViewModels.GenreViewModel> Tracks
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

        public ObservableCollection<ViewModels.GenreViewModel> GetGenres()
        {
            if (Results.Genres!=null) Results.Genres.Clear();
            tracks = new ObservableCollection<ViewModels.GenreViewModel>();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                TableQuery<Genre> query = db.Table<Genre>().OrderBy(c => c.Name);
                foreach (Genre track in query)
                {
                    var etrack = new ViewModels.GenreViewModel()
                        {      
                            Duration = track.Duration,
                            Name = track.Name,
                            Image = track.Image,
                            SongCount = track.SongCount,
                            Listens = track.Listens,
                            Meta = track.Meta,
                            Rate = track.Rate,
                            LastPlay = track.LastPlayed,
                      
                        };
                    tracks.Add(etrack);
                    if (Results.Genres==null)Results.Genres =new ObservableCollection<ViewModels.GenreViewModel>();
                    Results.Genres.Add(etrack);
                }
            }
            return tracks;
        }

         

    }
}