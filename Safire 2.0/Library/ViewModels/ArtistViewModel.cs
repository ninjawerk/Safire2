using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Safire.Library.TableModels;
using Safire.SQLite;

namespace Safire.Library.ViewModels
{
    public class ArtistViewModel : ViewModelBase
    {
        #region Properties

        private string name = "";
        public string Name
        {
            get
            { return name; }

            set
            {
                if (name == value)
                { return; }

                name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string image = "";
        public string Image
        {
            get
            {
                if (
                               File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Safire\ArtistData\" +
                                            Name + ".jpg"))
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Safire\ArtistData\" +
                                            Name + ".jpg";
                }
                return null;
            }

            set
            {
                if (image == value)
                { return; }

                image = value;
                RaisePropertyChanged("Image");
            }
        }

        private double rate = 2;
        public double Rate
        {
            get
            { return rate; }

            set
            {
                if (rate == value)
                { return; }

                rate = value;

                RaisePropertyChanged("Rate");
            }
        }

        private uint listens = 0;
        public uint Listens
        {
            get
            { return listens; }

            set
            {
                if (listens == value)
                { return; }

                listens = value;

                RaisePropertyChanged("Listens");
            }
        }

        private uint songCount = 0;
        public uint SongCount
        {
            get
            { return songCount; }

            set
            {
                if (songCount == value)
                { return; }

                songCount = value;

                RaisePropertyChanged("SongCount");
            }
        }

        private uint albumCount = 0;
        public uint AlbumCount
        {
            get
            { return albumCount; }

            set
            {
                if (albumCount == value)
                { return; }

                albumCount = value;

                RaisePropertyChanged("AlbumCount");
            }
        }


        private long duration = 0;
        public long Duration
        {
            get
            { return duration; }

            set
            {
                if (duration == value)
                { return; }

                duration = value;

                RaisePropertyChanged("Duration");
            }
        }


        private DateTime lastPlay = DateTime.Now;
        public DateTime LastPlay
        {
            get
            { return lastPlay; }

            set
            {
                if (lastPlay == value)
                { return; }

                lastPlay = value;

                RaisePropertyChanged("LastPlay");
            }
        }

        private string meta = string.Empty;
        public string Meta
        {
            get
            { return meta; }

            set
            {
                if (meta == value)
                { return; }

                meta = value;

                RaisePropertyChanged("Meta");
            }
        }


        #endregion "Properties"

        public string SaveArtist()
        {
            string result = string.Empty;
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {
                db.BeginTransaction();

                try
                {
                    //load the existing track
                    var existingArtist = (db.Table<Artist>().Where(
                        c => c.Name == Name)).SingleOrDefault();

                    //if there is a track already
                    if (existingArtist != null)
                    {
                        existingArtist.Name = Name;

                        existingArtist.Duration = Duration;
                        existingArtist.SongCount = songCount;
                        existingArtist.AlbumCount = albumCount;

                        existingArtist.LastPlayed = LastPlay;
                        existingArtist.Listens = Listens;
                        existingArtist.Image = image;
                        existingArtist.Meta = Meta;
                        existingArtist.Rate = Rate;


                        int success = db.Update(existingArtist);
                    }
                    else
                    {
                        //no track exists, make a new one
                        int success = db.Insert(new Artist()
                        {
                            Name = Name,
                            Duration = Duration,
                            LastPlayed = LastPlay,
                            Listens = Listens,
                            Meta = Meta,
                            Rate = Rate,
                            AlbumCount = AlbumCount,
                            SongCount = SongCount,
                            Image = image
                        });
                    }
                    result = "success";
                }
                catch
                {
                    result = "This customer was not saved.";
                }
                db.Commit();

            }
            return result;
        }

        //**Need editing
        public string DeleteArtist()
        {
            string result = string.Empty;
            LockManager lm = new LockManager(); lm.PrepareForRead();
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {

                //search for tracks with the artist
                var existingTrack = db.Table<Track>().Where(
                    p => p.Artist == Name);

                //search for albums by artist
                var album = db.Table<Album>().Where(
                    p => p.Artist == Name);

                //search for artist
                var existingArtist = (db.Table<Artist>().Where(
                    c => c.Name == name)).Single();

                db.RunInTransaction(() =>
                {
                    //delete tracks 
                    foreach (Track ar in existingTrack)
                    {
                        db.Delete(ar);
                    }

                    //delete albums
                    foreach (Album ar in album)
                    {
                        db.Delete(ar);
                    }

                    //delete artist
                    if (db.Delete(existingArtist) > 0)
                    {
                        result = "Success";
                    }
                    else
                    {
                        result = "This customer was not removed";
                    }
                });
            }
            lm.RemoveLock();
            return result;
        }

    }
}
