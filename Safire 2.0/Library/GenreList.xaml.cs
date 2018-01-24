using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Safire.Animation;
using Safire.Controls;
using Safire.Library.Core;
using Safire.Library.Cycling;
using Safire.Library.ObservableCollection;
using Safire.Library.Queries;
using Safire.Library.ViewModels;
using Xceed.Wpf.DataGrid;

namespace Safire.Library
{
    /// <summary>
    ///     Interaction logic for ArtistList.xaml
    /// </summary>
    public partial class GenreList : UserControl
    {
		readonly XDGV_DragDropAutomation _dda = new XDGV_DragDropAutomation();
		readonly XDG_DragDropAutomation _lb_dda = new XDG_DragDropAutomation();

        public GenreList()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            var genreViewModel = new Library.GenreViewModel();
            Results.Genres = new ObservableCollection<ViewModels.GenreViewModel>();
            if (Results.Genres != null)
                lst.ItemsSource = Results.Genres;
            if (Results.Genres != null) Results.Genres.CollectionChanged += Genres_CollectionChanged;
            genreViewModel.GetGenres();

            //drags
			_dda.Register(tracklist);
			_lb_dda.Register(lst, DragDropProcessor.GenreProcessList);

         }
                         
        public static String Album { get; set; }

        /// <summary>
        ///     Update the list if the artist collection has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Genres_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ScrollsListBox(lst)));
        }

        /// <summary>
        ///     Refresh the display of the listbox
        /// </summary>
        /// <param name="sender"></param>
		private void ScrollsListBox(DataGridControl sender)
        {
            try
            {
                if (sender.Items.Count < 1)
                {
					noresults.Text = "No results for \"" + (App.Current.MainWindow as MainWindow).hLibrary.CurrentSearchPackage.Data  + "\"";
                    noresults.Visibility = Visibility.Visible;
                    lst.Visibility = Visibility.Hidden;
                }
                else
                {
                    noresults.Visibility = Visibility.Hidden;
                    lst.Visibility = Visibility.Visible;
                }
                sender.Items.Refresh();
				if (_lst_scrollView == null)
				{
					_lst_scrollView = lst.GetScrollbar();
				}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

     
        private void lst_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
         
            var art = lst.SelectedItem as ViewModels.GenreViewModel;
           
            artist.Text = art.Name;
            duration.Text = " " + TimeSpan.FromSeconds(art.Duration);             
            Album = art.Name;
			selArtist.FadeOut();
            tracklist.ItemsSource = TrackQuery.GetTracksFromGenre(art.Name);
        }


        

        private void tracklist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tracklist.Tag = tracklist.SelectedIndex;
            Items.trackSource = tracklist;
            var tracky = tracklist.SelectedItem as TrackViewModel;
            if (tracky != null && File.Exists(tracky.Path))
            {
                var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, Player.Instance.Volume);
                aw.ReactorUsageLocked = true;
                aw.Play();
                aw.ReactorUsageLocked = false;
                var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
                fader.StartAndKill();
                Player.Instance.NewMedia(ref aw);
                //Player.Instance.Play();
            }
        }
		private ScrollViewer _lst_scrollView;

		public void RefreshUI()
		{

		
			if (_lst_scrollView != null) _lst_scrollView.ScrollToVerticalOffset(0);

			//Dispatcher.BeginInvoke(new Action(() => ScrollsListBox(lst)));
		}
    }
}