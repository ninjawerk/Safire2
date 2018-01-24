using System;

namespace Safire.Library.ObservableCollection
{
    [Flags]
    public enum SearchType
    {
        trTitle=0x101,
        trArtist = 0x1,
        trAlbum = 0x2,
        trPath = 0x3,
        trBitRate = 0x4,
        trComposer = 0x5,
        trGenre = 0x6,
        trLyrics = 0x7,
        trYear = 0x8,
        trRate = 0x9,
        arName = 0x10,
        arRate = 0x11,
        alName = 0x12,
        alRate = 0x13,
        geName = 0x14,
        geRate = 0x15
    }
}