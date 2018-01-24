using System.Windows.Controls;
using Safire.Views;

namespace Safire
{
    /// <summary>
    /// Interaction logic for SearchResults.xaml
    /// </summary>
    public partial class SearchResults : UserControl
    {
        public SearchResults()
        {
            this.InitializeComponent();
           
        }
        public void Initialize()
        {
            Library.ObservableCollection.Results.Artists.CollectionChanged += Artists_CollectionChanged;
            Library.ObservableCollection.Results.Albums.CollectionChanged += Albums_CollectionChanged;
             Library.ObservableCollection.Results.Tracks.CollectionChanged += Tracks_CollectionChanged;
        }

        void Tracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
             songs.Text = Library.ObservableCollection.Results.Tracks.Count + " songs";
        }

        void Albums_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            albums.Text = Library.ObservableCollection.Results.Albums.Count + " albums";
        }

        void Artists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            artists.Text = Library.ObservableCollection.Results.Artists.Count + " artists";
        }

        private void btnArtist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EnumerationsContainer.LibraryUi = LibraryUiMode.Artist;
            (App.Current.MainWindow as MainWindow).hLibrary.RefreshUi();
        }

        private void btnSongs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EnumerationsContainer.LibraryUi = LibraryUiMode.Songs;
            (App.Current.MainWindow as MainWindow).hLibrary.RefreshUi();
        }

        private void btnAlbums_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EnumerationsContainer.LibraryUi = LibraryUiMode.Album;
            (App.Current.MainWindow as MainWindow).hLibrary.RefreshUi();
        }

        public void Refresh()
        {
            search.Text = "Search results for \"" + (App.Current.MainWindow as MainWindow).txtSearch.Text + "\"";
        }
    }
}