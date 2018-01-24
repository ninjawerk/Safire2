using System.Collections.ObjectModel;
using Safire.Library.ViewModels;

namespace Safire.Library.ObservableCollection
{
    public sealed class Results
    {
        public static int previewCount = -1;
        public static bool displayFreeze = false;
        public static SearchPackage TrackSearch = new SearchPackage {SearchType = SearchType.trArtist, Data = ""};
        public static SearchPackage ArtistSearch = new SearchPackage {SearchType = SearchType.arName};
        public static SearchPackage AlbumSearch = new SearchPackage {SearchType = SearchType.alName};
        public static SearchPackage GenreSearch = new SearchPackage { SearchType = SearchType.geName };

        public static ObservableCollection<TrackViewModel> Tracks = null;
        public static ObservableCollection<ArtistViewModel>  Artists = null;
        public static ObservableCollection<AlbumViewModel> Albums = null;
        public static ObservableCollection<ViewModels.GenreViewModel> Genres = null;

    }
}