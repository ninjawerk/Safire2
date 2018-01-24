using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Safire.Library.TableModels;
using Safire.SQLite;

namespace Safire.Library.ViewModels
{
    public class AlbumViewModel : ViewModelBase
    {
        #region Properties
        private string id = "";
        public string ID
        {
            get
            { return id; }

            set
            {
                if (id == value)
                { return; }

                id = value;
                RaisePropertyChanged("ID");
            }
        }

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

        private string artist = "";
        public string Artist
        {
            get
            { return artist; }

            set
            {
                if (artist == value)
                { return; }

                artist = value;
                RaisePropertyChanged("Artist");
            }
        }

        private string image = "";
        public string Image
        {
            get
            {  
            if (
                              File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Safire\AlbumData\" +
                                           Name + " - " + Artist +  ".jpg"))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Safire\AlbumData\" +
                                        Name + " - " + Artist + ".jpg";
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
        public string SaveAlbum()
        {
            string result = string.Empty;
            LockManager lm = new LockManager(); lm.PrepareForRead();
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {
                db.BeginTransaction();
                try
                {
                    //load the existing track
                    var existingArtist = (db.Table<Album>().Where(
                        c => c.ID == ID)).SingleOrDefault();

                    //if there is a track already
                    if (existingArtist != null)
                    {
                        existingArtist.ID = ID;
                        existingArtist.Name = Name;
                        existingArtist.Artist = Artist;
                        existingArtist.Duration = Duration;
                        existingArtist.SongCount = songCount;                        
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
                        int success = db.Insert(new Album()
                        {
                            ID = id,
                            Name = Name,
                            Duration = Duration,
                            LastPlayed = LastPlay,
                            Listens = Listens,
                            Meta = Meta,
                            Rate = Rate,
                            Artist=Artist,
                            SongCount = SongCount,
                            Image = image
                        });
                    }
                    result = "Success";
                }
                catch
                {
                    result = "This customer was not saved.";
                }
                db.Commit();
             
                lm.RemoveLock();
            }
            return result;
        }

        //**Need editing
        public string DeleteAlbum()
        {
            string result = string.Empty;
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {

                //search for tracks in the album
                var tracks = db.Table<Track>().Where(
                    p => p.Album == Name && p.Artist == Artist);

                //search for album
                var existingAlbum = db.Table<Album>().Where(
                     p => p.ID == id);
 
                db.RunInTransaction(() =>
                {
                    //delete tracks 
                    foreach (Track ar in tracks)
                    {
                         db.Delete(ar);
                    }
 

                    //delete album
                    if (db.Delete(existingAlbum) > 0)
                    {
                        result = "Success";
                    }
                    else
                    {
                        result = "This customer was not removed";
                    }
                });
            }
            return result;
        }

    }
}
