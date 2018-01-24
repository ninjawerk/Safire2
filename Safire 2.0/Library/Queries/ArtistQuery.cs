using System;
using System.Collections;
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
	public class ArtistQuery
    {
        private static Thread tr;

        public static ArtistViewModel GetArtistViewModel(string name)
        {
            var artist = new ArtistViewModel();
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                Artist _artist;
                try
                {
                    _artist = (db.Table<Artist>().Where(
                         c => c.Name == name)).Single();
                }
                catch (Exception e)
                {
                    return null;
                }
                artist.Name = _artist.Name;
                artist.Duration = _artist.Duration;
                artist.Image = _artist.Image;
                artist.LastPlay = _artist.LastPlayed;
                artist.Listens = _artist.Listens;
                artist.SongCount = _artist.SongCount;
                artist.AlbumCount = _artist.AlbumCount;
                artist.Meta = _artist.Meta;
                artist.Rate = _artist.Rate;
                //9
            }
            return artist;
        }

        public static Artist GetArtist(string name)
        {
            Artist _artist;
            using (var db = new SQLiteConnection(Tables.DBPath))
            {

                try
                {
                    _artist = (db.Table<Artist>().Where(
                         c => c.Name == name)).Single();
                }
                catch (Exception e)
                {
                    return null;
                }

                //9
            }
            return _artist;
        }

        /// <summary>
        /// Find tracks, searching, threaded.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dp"></param>
        public static void FindArtists(SearchPackage search, Dispatcher dp)
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
            TableQuery<Artist> query = null;
            IEnumerable<ArtistViewModel> nq = null;
            LockManager lm = new LockManager(); lm.PrepareForWrite();
            int i = 0;
            if (Results.Artists == null) Results.Artists = new ObservableCollection<ArtistViewModel>();
            dp.BeginInvoke(new Action(() => Results.Artists.Clear()));
                if (!Adder.LibraryAdder.DB_InUse)
                {
                    using (var db = new SQLiteConnection(Tables.DBPath))
                    {
                        query =
                            db.Table<Artist>()
                                .OrderByDescending(c => c.Listens)
                                .Where(
                                    c => (c.Name.Contains(search.Data))

                                );
                        if (query != null)
                {
                    foreach (Artist _artist in query)
                    {

                        var artist = new ArtistViewModel()
                        {
                            Name = _artist.Name,
                            Duration = _artist.Duration,
                            Image = _artist.Image,
                            LastPlay = _artist.LastPlayed,
                            Listens = _artist.Listens,
                            SongCount = _artist.SongCount,
                            AlbumCount = _artist.AlbumCount,
                            Meta = _artist.Meta,
                            Rate = _artist.Rate
                        };
                        try
                        {
                            if (i++ > Results.previewCount)
                            {
                                Results.displayFreeze = true;
                                dp.BeginInvoke(new Action(() => Results.Artists.Add(artist)));
                            }
                            else if (i == Results.previewCount)
                            {
                                Thread.Sleep(250);
                            }
                            else
                            {
                                dp.BeginInvoke(new Action(() => Results.Artists.Add(artist)));
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
                     Adder.LibraryAdder.Artists
                     .OrderByDescending(c => c.Listens)
                     .Where(
                    c => (c.Name.ToLower().Contains(search.Data.ToLower()))

                         );
                    foreach (var  artist in nq)
                    {

                        
                        try
                        {
                            if (i++ > Results.previewCount)
                            {
                                Results.displayFreeze = true;
                                dp.BeginInvoke(new Action(() => Results.Artists.Add(artist)));
                            }
                            else if (i == Results.previewCount)
                            {
                                Thread.Sleep(250);
                            }
                            else
                            {
                                dp.BeginInvoke(new Action(() => Results.Artists.Add(artist)));
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                }
              
                //dp.BeginInvoke(new Action(() =>
                //{
                
                 
                Results.displayFreeze = false;
                //dp.BeginInvoke(new Action(() => Results.Artists.Add(null)));
                lm.RemoveLock();
            }
        }
    }
}
