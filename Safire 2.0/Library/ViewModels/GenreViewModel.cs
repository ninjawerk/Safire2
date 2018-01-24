using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Safire.Library.TableModels;
using Safire.SQLite;

namespace Safire.Library.ViewModels
{
   public class GenreViewModel : ViewModelBase
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

         
        private string image = "";
        public string Image
        {
            get
            {
                if (
                                  File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Safire\AlbumData\" +
                                               Name + " - " + Name + ".jpg"))
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Safire\GenreData\" +
                                            Name + " - " + Name + ".jpg";
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
        public string SaveGenre()
        {
            string result = string.Empty;
            LockManager lm = new LockManager(); lm.PrepareForRead();
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {
                db.BeginTransaction();
                try
                {
                    //load the existing track
                    var existingGenre = (db.Table<Genre>().Where(
                        c => c.Name == Name)).SingleOrDefault();

                    //if there is a track already
                    if (existingGenre != null)
                    {
                      
                        existingGenre.Name = Name;
                    
                        existingGenre.Duration = Duration;
                        existingGenre.SongCount = songCount;
                        existingGenre.LastPlayed = LastPlay;
                        existingGenre.Listens = Listens;
                        existingGenre.Image = image;
                        existingGenre.Meta = Meta;
                        existingGenre.Rate = Rate;

                        int success = db.Update(existingGenre);
                    }
                    else
                    {
                        //no track exists, make a new one
                        int success = db.Insert(new Genre()
                        {
                    
                            Name = Name,
                            Duration = Duration,
                            LastPlayed = LastPlay,
                            Listens = Listens,
                            Meta = Meta,
                            Rate = Rate,
                      
                            SongCount = SongCount,
                            Image = image
                        });
                    }
                    result = "Success";
                }
                catch
                {
                    result = "This Genre was not saved.";
                }
                db.Commit();

                lm.RemoveLock();
            }
            return result;
        }

        //**Need editing
        public string DeleteGenre()
        {
            string result = string.Empty;
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {

                //search for tracks in the genre
                var tracks = db.Table<Track>().Where(
                    p => p.Genre == Name);

                //search for genre
                var existingGenre= db.Table<Genre>().Where(
                     p => p.Name == Name);

                db.RunInTransaction(() =>
                {
                    //delete tracks 
                    foreach (Track ar in tracks)
                    {
                        db.Delete(ar);
                    }


                    //delete genre
                    if (db.Delete(existingGenre) > 0)
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
