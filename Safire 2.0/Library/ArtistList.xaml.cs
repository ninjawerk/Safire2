using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Safire.Animation;
using Safire.Controls;
using Safire.Library.Core;
using Safire.Library.Cycling;
using Safire.Library.ObservableCollection;
using Safire.Library.Queries;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;
using Safire.Views;
using Xceed.Wpf.DataGrid;

namespace Safire.Library
{
	/// <summary>
	///     Interaction logic for ArtistList.xaml
	/// </summary>
	public partial class ArtistList : UserControl
	{
		private readonly XDGV_DragDropAutomation _dda = new XDGV_DragDropAutomation();
		private readonly XDG_DragDropAutomation _lb_dda = new XDG_DragDropAutomation();
		private ScrollViewer _lst_scrollView;

		public static String Artist { get; set; }

		#region Ctor

		public ArtistList()
		{
			InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(this)) return;
			var artistsViewModel = new ArtistsViewModel();
			Results.Artists = new ObservableCollection<ArtistViewModel>();
			if (Results.Artists != null)
				lst.ItemsSource = Results.Artists;
			if (Results.Artists != null) Results.Artists.CollectionChanged += Artists_CollectionChanged;
			artistsViewModel.GetArtists();

			//drags
			_dda.Register(tracklist);
			_lb_dda.Register(lst, DragDropProcessor.ArtistProcessList);
		}

		#endregion

		#region List Updating

		/// <summary>
		///     Update the list if the artist collection has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Artists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!Results.displayFreeze && EnumerationsContainer.LibraryUi == LibraryUiMode.Artist)
			{
			}
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
					noresults.Text = "No results for \"" +
									 (Application.Current.MainWindow as MainWindow).hLibrary.CurrentSearchPackage.Data + "\"";
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

		#endregion

		private void ListBoxItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			e.Handled = true;
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lst_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var art = lst.SelectedItem as ArtistViewModel;
			if (art == null) return;

			Artist = art.Name;

			artist.Text = art.Name;
			duration.Text = " " + TimeSpan.FromSeconds(art.Duration);

			var tracks = TrackQuery.GetTracksFromArtist(art.Name);
			tracklist.ItemsSource = tracks;
			albums.Text = " " + tracks.GroupBy(x => x.Album).Count() + " albums";
			rateCtl.Path = art.Name;
			rateCtl.Rate = art.Rate;
			selArtist.FadeOut();
		}


		/// <summary>
		///     Play the track
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		public void RefreshUI()
		{
			if (_lst_scrollView != null) _lst_scrollView.ScrollToVerticalOffset(0);

			//Dispatcher.BeginInvoke(new Action(() => ScrollsListBox(lst)));
		}

		public void LoadArtist(string artistName)
		{
			ArtistViewModel art = ArtistQuery.GetArtistViewModel(artistName);
			if (art == null) return;

			Artist = art.Name;

			artist.Text = art.Name;
			duration.Text = " " + TimeSpan.FromSeconds(art.Duration);

			var tracks = TrackQuery.GetTracksFromArtist(art.Name);
			tracklist.ItemsSource = tracks;
			albums.Text = " " + tracks.GroupBy(x => x.Album).Count() + " albums";
			rateCtl.Path = art.Name;
			rateCtl.Rate = art.Rate;
			selArtist.FadeOut();
		}

	 

		private void TrackFavChanged(object sender, MouseButtonEventArgs e)
		{
		

			var Rate = 2;
			var t = sender as TextBlock;

			if (t.Text == "")	 //2
			{
				t.Text = "";//5
				Rate = 5;
				t.Opacity = 1;
			}
			else
			{
				t.Text = "";
				Rate = 2;
				t.Opacity = .25;
			}

			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				db.TimeExecution = true;

				var query =
					db.Table<Artist>()
						.FirstOrDefault(
							c => c.Name == t.Tag.ToString());
				if (query != null)
				{
					query.Rate = Rate;
					try
					{
							db.Update(query);
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
				}
			}
			tracklist.InvalidateVisual();
			 tracklist.Items.Refresh();
		}
	}
}