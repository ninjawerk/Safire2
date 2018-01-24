using System;
using Safire.SQLite;

namespace Safire.Library.TableModels
{
    public class Genre
    {
        //Basic
        [PrimaryKey]
        public string Name { get; set; }
        public double Rate { get; set; }

        //DateTimes
        public DateTime LastPlayed { get; set; }

        //Counts
        public uint Listens { get; set; }
        public uint SongCount { get; set; }
        public long Duration { get; set; }

        //Other
        public string Image { get; set; }
        public string Meta { get; set; }
    }
}