using System;
using Safire.SQLite;

namespace Safire.Library.TableModels
{
    //Object Relational Mapping MVVM
	[Serializable]
    public class Track
    {
        [PrimaryKey]
        public string Path { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Composer { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public double Rate { get; set; }
        public uint Listens { get; set; }
        public long Duration { get; set; }
        public uint Bitrate { get; set; }
        public uint Channels { get; set; }
        public string FileType { get; set; }
        public DateTime LastPlay { get; set; }
        public string Lyrics { get; set; }
        public uint TrackPosition { get; set; }
        public uint Year { get; set; }
        public string Meta { get; set; }
    }
}
