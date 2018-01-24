using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Safire.Library.ObservableCollection;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;

namespace Safire.Library.Queries
{
    public class AlbumQuery
    {
        private static Thread tr;
        public static AlbumViewModel GetAlbumViewModel(string name)
        {
            var artist = new AlbumViewModel();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                Album _album;
                try
                {
                    _album = (db.Table<Album>().Where(
                        c => c.Name == name)).Single();
                }
                catch (Exception e)
                {
                    return null;
                }

                artist.Name = _album.Name;
                artist.Duration = _album.Duration;
                artist.Image = _album.Image;
                artist.LastPlay = _album.LastPlayed;
                artist.Listens = _album.Listens;
                artist.SongCount = _album.SongCount;
                artist.Artist = _album.Artist;
                artist.Meta = _album.Meta;
                artist.Rate = _album.Rate;
                //9
            }
            return artist;
        }

        public static Album GetAlbum(string name)
        {
            Album _album;
            using (var db = new SQLiteConnection(Tables.DBPath))
            {

                try
                {
                    _album = (db.Table<Album>().Where(
                        c => c.Name == name)).Single();
                }
                catch (Exception e)
                {
                    return null;
                }

            }
            return _album;
        }
        /// <summary>
        /// Find albums, searching, threaded.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dp"></param>
        public static void FindAlbums(SearchPackage search, Dispatcher dp)
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
            TableQuery<Album> query = null;
            IEnumerable<AlbumViewModel> nq = null;
            LockManager lm = new LockManager(); lm.PrepareForWrite();
            int i = 0;
            if (Results.Albums == null) Results.Albums = new ObservableCollection<AlbumViewModel>();
            dp.BeginInvoke(new Action(() => Results.Albums.Clear()));
            if (!Adder.LibraryAdder.DB_InUse)
            {
                using (var db = new SQLiteConnection(Tables.DBPath))
                {
                    query =
                        db.Table<Album>()
                            .OrderByDescending(c => c.Listens)
                            .Where(
                                c => (c.Name.Contains(search.Data)));

                    if (query != null)
                    {
                        foreach (Album _album in query)
                        {

                            var album = new AlbumViewModel()
                            {
                                Name = _album.Name,
                                Duration = _album.Duration,
                                Image = _album.Image,
                                LastPlay = _album.LastPlayed,
                                Listens = _album.Listens,
                                SongCount = _album.SongCount,
                                Artist = _album.Artist,
                                Meta = _album.Meta,
                                Rate = _album.Rate
                            };
                            try
                            {
                                if (i++ > Results.previewCount)
                                {
                                    Results.displayFreeze = true;
                                    dp.BeginInvoke(new Action(() => Results.Albums.Add(album)));
                                }
                                else if (i == Results.previewCount)
                                {
                                    Thread.Sleep(250);
                                }
                                else
                                {
                                    dp.BeginInvoke(new Action(() => Results.Albums.Add(album)));
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
                   Adder.LibraryAdder.Albums
                     .OrderByDescending(c => c.Listens)
                     .Where(
                         c => (c.Name.ToLower().Contains(search.Data.ToLower())));
                foreach (var album in nq)
                {


                    try
                    {
                        if (i++ > Results.previewCount)
                        {
                            Results.displayFreeze = true;
                            dp.BeginInvoke(new Action(() => Results.Albums.Add(album)));
                        }
                        else if (i == Results.previewCount)
                        {
                            Thread.Sleep(250);
                        }
                        else
                        {
                            dp.BeginInvoke(new Action(() => Results.Albums.Add(album)));
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
            // dp.BeginInvoke(new Action(() => Results.Albums.Add(null)));


            lm.RemoveLock();
        }
    }
}
