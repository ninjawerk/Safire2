using System;
using Safire.SQLite;

namespace Safire.Library.TableModels
{
    public class Album
    {
        //Basic
        [PrimaryKey]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
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