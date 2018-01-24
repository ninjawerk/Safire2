using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Safire.Library.ObservableCollection;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;

namespace Safire.Library.Queries
{
    public class GenreQuery
    {
        private static Thread tr;
        public static ViewModels.GenreViewModel GetGenreViewModel(string name)
        {
            var genre = new ViewModels.GenreViewModel();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                Genre _album;
                try
                {
                    _album = (db.Table<Genre>().Where(
                        c => c.Name == name)).Single();
                }
                catch (Exception e)
                {
                    return null;
                }

                genre.Name = _album.Name;
                genre.Duration = _album.Duration;
                genre.Image = _album.Image;
                genre.LastPlay = _album.LastPlayed;
                genre.Listens = _album.Listens;
                genre.SongCount = _album.SongCount;

                genre.Meta = _album.Meta;
                genre.Rate = _album.Rate;
                //9
            }
            return genre;
        }
        public static Genre GetGenre(string name)
        {
            Genre _genre;
            using (var db = new SQLiteConnection(Tables.DBPath))
            {

                try
                {
                    _genre = (db.Table<Genre>().Where(
                        c => c.Name == name)).Single();
                }
                catch (Exception e)
                {
                    return null;
                }


                //9
            }
            return _genre;
        }
        /// <summary>
        /// Find albums, searching, threaded.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dp"></param>
        public static void FindGenres(SearchPackage search, Dispatcher dp)
        {
            var o = new object[] { search, dp };
            if (tr == null) tr = new Thread(Start);
            while (tr.IsAlive)
            {
                tr.Abort();
            }
            tr = null;
            if (tr == null) tr = new Thread(Start);
            tr.Start(o);
        }


        private static void Start(object o)
        {
            var ob = o as object[];
            var search = (SearchPackage)ob[0];
            var dp = (Dispatcher)ob[1];
            TableQuery<Genre> query = null;
            IEnumerable<ViewModels.GenreViewModel> nq;
            LockManager lm = new LockManager(); lm.PrepareForWrite();
            int i = 0;
            if (Results.Genres == null) Results.Genres = new ObservableCollection<ViewModels.GenreViewModel>();
            dp.BeginInvoke(new Action(() => Results.Genres.Clear()));
            if (!Adder.LibraryAdder.DB_InUse)
            {
                using (var db = new SQLiteConnection(Tables.DBPath))
                {
                    query =
                        db.Table<Genre>()
                            .OrderByDescending(c => c.Listens)
                            .Where(
                                c => (c.Name.Contains(search.Data))

                            );
                    if (query != null)
                    {
                        foreach (Genre _album in query)
                        {

                            var genre = new ViewModels.GenreViewModel()
                            {
                                Name = _album.Name,
                                Duration = _album.Duration,
                                Image = _album.Image,
                                LastPlay = _album.LastPlayed,
                                Listens = _album.Listens,
                                SongCount = _album.SongCount,

                                Meta = _album.Meta,
                                Rate = _album.Rate
                            };
                            try
                            {
                                if (i++ > Results.previewCount)
                                {
                                    Results.displayFreeze = true;
                                    dp.BeginInvoke(new Action(() => Results.Genres.Add(genre)));
                                }
                                else if (i == Results.previewCount)
                                {
                                    Thread.Sleep(250);
                                }
                                else
                                {
                                    dp.BeginInvoke(new Action(() => Results.Genres.Add(genre)));
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                }
            }
            else
            {
                nq =
              Adder.LibraryAdder.Genres
                .OrderByDescending(c => c.Listens)
                .Where(
                    c => (c.Name.ToLower().Contains(search.Data.ToLower()))

                         );
                foreach (var genre in nq)
                {


                    try
                    {
                        if (i++ > Results.previewCount)
                        {
                            Results.displayFreeze = true;
                            dp.BeginInvoke(new Action(() => Results.Genres.Add(genre)));
                        }
                        else if (i == Results.previewCount)
                        {
                            Thread.Sleep(250);
                        }
                        else
                        {
                            dp.BeginInvoke(new Action(() => Results.Genres.Add(genre)));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            //dp.BeginInvoke(new Action(() =>
            //{


            Results.displayFreeze = false;
            // dp.BeginInvoke(new Action(() => Results.Genres.Add(null)));


            lm.RemoveLock();
        }
    }
}
