using System;
using System.Linq;
using Safire;
using Safire.Library.ObservableCollection;
using Safire.Library.TableModels;
using Safire.SQLite;

namespace Safire.Library.ViewModels
{
	[Serializable]
    public class TrackViewModel : ViewModelBase
    {

	    public TrackViewModel()
	    {
		    
	    }
        #region Properties

        private string path = "";
        public string Path
        {
            get
            { return path; }

            set
            {
                if (path == value)
                { return; }

                path = value;
                RaisePropertyChanged("Path");
            }
        }

        private string title = string.Empty;
        public string Title
        {
            get
            { return title; }

            set
            {
                if (title == value)
                { return; }

                title = value;

                RaisePropertyChanged("Title");
            }
        }

        private string artist = string.Empty;
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

        private string composer = string.Empty;
        public string Composer
        {
            get
            { return composer; }

            set
            {
                if (composer == value)
                { return; }

                composer = value;

                RaisePropertyChanged("Composer");
            }
        }

        private string album = string.Empty;
        public string Album
        {
            get
            { return album; }

            set
            {
                if (album == value)
                { return; }

                album = value;

                RaisePropertyChanged("Album");
            }
        }

        private string genre = string.Empty;
        public string Genre
        {
            get
            { return genre; }

            set
            {
                if (genre == value)
                { return; }

                genre = value;

                RaisePropertyChanged("Genre");
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

        private uint bitrate = 0;
        public uint Bitrate
        {
            get
            { return bitrate; }

            set
            {
                if (bitrate == value)
                { return; }

                bitrate = value;

                RaisePropertyChanged("Bitrate");
            }
        }

        private uint channels = 0;
        public uint Channels
        {
            get
            { return channels; }

            set
            {
                if (channels == value)
                { return; }

                channels = value;

                RaisePropertyChanged("Frequency");
            }
        }

        private string fileType = string.Empty;
        public string FileType
        {
            get
            { return fileType; }

            set
            {
                if (fileType == value)
                { return; }

                fileType = value;

                RaisePropertyChanged("FileType");
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


        private string lyrics = string.Empty;
        public string Lyrics
        {
            get
            { return lyrics; }

            set
            {
                if (lyrics == value)
                { return; }

                lyrics = value;

                RaisePropertyChanged("Lyrics");
            }
        }

        private uint trackPosition = 0;
        public uint TrackPosition
        {
            get
            { return trackPosition; }

            set
            {
                if (trackPosition == value)
                { return; }

                trackPosition = value;

                RaisePropertyChanged("TrackPosition");
            }
        }

        private uint year = 0;
        public uint Year
        {
            get
            { return year; }

            set
            {
                if (year == value)
                { return; }

                year = value;

                RaisePropertyChanged("Year");
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

        public string SaveTrack()
        {
            string result = string.Empty;
            LockManager lm = new LockManager(); lm.PrepareForRead();
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {
                db.BeginTransaction();
                try
                {
                    //load the existing track
                    var existingTrack = (db.Table<Track>().Where(
                        c => c.Path == Path)).SingleOrDefault();

                    //if there is a track already
                    if (existingTrack != null)
                    {
                        existingTrack.Path = Path;
                        existingTrack.Title = Title;
                        existingTrack.Artist = Artist;
                        existingTrack.Album = Album;
                        existingTrack.Bitrate = Bitrate;
                        existingTrack.Composer = Composer;
                        existingTrack.Duration = Duration;
                        existingTrack.FileType = FileType;
                        existingTrack.Channels = Channels;
                        existingTrack.Genre = Genre;
                        existingTrack.LastPlay = LastPlay;
                        existingTrack.Listens = Listens;
                        existingTrack.Lyrics = Lyrics;
                        existingTrack.Meta = Meta;
                        existingTrack.Rate = Rate;
                        existingTrack.TrackPosition = TrackPosition;
                        existingTrack.Year = Year;

                        int success = db.Update(existingTrack);
                    }
                    else
                    {
                        //no track exists, make a new one
                        int success = db.Insert(new Track()
                        {
                            Path = Path,
                            Title = Title,
                            Artist = Artist,
                            Album = Album,
                            Bitrate = Bitrate,
                            Composer = Composer,
                            Duration = Duration,
                            FileType = FileType,
                            Channels = Channels,
                            Genre = Genre,
                            LastPlay = LastPlay,
                            Listens = Listens,
                            Lyrics = Lyrics,
                            Meta = Meta,
                            Rate = Rate,
                            TrackPosition = TrackPosition,
                            Year = Year,
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
            lm.RemoveLock();
            return result;
        }

        //**Need editing
        public string DeleteTrack()
        {
            string result = string.Empty;
           
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {
                
                //search for artist
                var artist = db.Table<Artist>().Where(
                    p => p.Name ==Title) ;

                //search for album
                var album = db.Table<Album>().Where(
                    p => p.ID ==Album  );

                //search for track
                var existingTrack = (db.Table<Track>().Where(
                    c => c.Path == path)).Single();

                db.RunInTransaction(() =>
                {
                     
                    //delete artist record if this is the last song
                    foreach (Artist ar in artist)
                    {
                        if (ar.SongCount == 1) db.Delete(ar);
                    }

                    //delete album record if this is the last song
                    foreach (Album ar in album)
                    {
                        if (ar.SongCount == 1) db.Delete(ar);
                    }

                    if (db.Delete(existingTrack) > 0)
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

        public static string FindTrack(SearchPackage search)
        {
            string result = string.Empty;
            using (var db = new SQLite.SQLiteConnection(Tables.DBPath))
            {

                //search for track
                var existingTrack = (db.Table<TrackViewModel>().Where(
                    c => c.Title == search.Data));

                Results.Tracks.Clear();

                foreach (var trackViewModel in existingTrack)
                {
                    Results.Tracks.Add(trackViewModel);
                }


            }
            return result;
        }

		public Track ToTrack()
		{
			var existingTrack = new Track();
			existingTrack.Path = Path;
			existingTrack.Title = Title;
			existingTrack.Artist = Artist;
			existingTrack.Album = Album;
			existingTrack.Bitrate = Bitrate;
			existingTrack.Composer = Composer;
			existingTrack.Duration = Duration;
			existingTrack.FileType = FileType;
			existingTrack.Channels = Channels;
			existingTrack.Genre = Genre;
			existingTrack.LastPlay = LastPlay;
			existingTrack.Listens = Listens;
			existingTrack.Lyrics = Lyrics;
			existingTrack.Meta = Meta;
			existingTrack.Rate = Rate;
			existingTrack.TrackPosition = TrackPosition;
			existingTrack.Year = Year;
			return existingTrack;
		}
    }
}
