using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Bridge;
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Lpfm.LastFmScrobbler;
using MahApps.Metro.Controls;
using Safire.Animation;
using Safire.Controls;
using Safire.Core.Plugins;
using Safire.Library.Adder;
using Safire.Library.Core;
using Safire.Library.ObservableCollection;
using Safire.Library.Queries;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.Properties;
using Safire.SQLite;
using Safire.Views;
using Safire.Windows;
using Xceed.Wpf.DataGrid;
using File = System.IO.File;
using Track = Safire.Library.TableModels.Track;

namespace Safire.Library
{
	/// <summary>
	///     Interaction logic for Library.xaml
	/// </summary>
	public partial class LibraryControl : UserControl
	{
		public bool ComponentInitialized = false;
		private ArtistsViewModel artistsViewModel;
		private ScrollViewer sv;
		private ObservableTracksViewModel tracksViewModel;
		XDGV_DragDropAutomation _dda = new XDGV_DragDropAutomation();

		public LibraryControl()
		{

		}
		#region Ctor-Detor
		public void Initialize()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				tracklist.MouseDoubleClick += tracklist_PreviewMouseDoubleClick;
				tracksViewModel = new ObservableTracksViewModel();

				Results.Tracks = new ObservableCollection<TrackViewModel>();

				tracklist.ReadOnly = true;

				tracklist.ItemsSource = Results.Tracks;

				if (Results.Tracks != null) Results.Tracks.CollectionChanged += Tracks_CollectionChanged;
				TrackQuery.GetTracks();

				if (Results.Tracks.Count == 0)
				{
					Tabs.FadeOut();
					grdDD.FadeIn();
				}

				Cycling.Cycler.Initialize();

				//drags
				_dda.Register(tracklist);

				//LoadPlugins
				foreach (var v in PluginCore.Instance().LibraryComponents)
				{
					if (v.Type == ComponentType.Progressive)
						v.StateChangedEvent += ComponentOnStateChangedEvent;
					//Add static all type ones

					if (v.Type == ComponentType.Static &&
						(v.Category == LibraryComponentCategory.All))
					{
						TreeViewItem item = new TreeViewItem();
						item.Header = v.Caption;
						item.Tag = v;
						List<TreeViewItem> str = new List<TreeViewItem>();
						if (v.Children != null)
							foreach (var va in v.Children)
							{
								str.Add(new TreeViewItem() { Header = va, Tag = v, Padding = new Thickness(0, 8, 0, 8) });
							}
						item.ItemsSource = str;
						plugTree.Items.Add(item);

					}

				}
				plugTree.SelectedItemChanged += plugTree_SelectedItemChanged;
				//RefreshPlugs();
				dpt.Tick += dpt_Tick;
				dpt.Interval = new TimeSpan(00, 0, 0, 0, 100);
			}
		}
		#endregion
		#region Plugins
		private int noresDelay = 0;
		void dpt_Tick(object sender, EventArgs e)
		{
			if (noresDelay++ > 5 && Results.Tracks.Count == 0)
			{
				noresults.Text = "No results for \"" + (App.Current.MainWindow as MainWindow).hLibrary.CurrentSearchPackage.Data + "\"";
				noresults.FadeIn();
				dpt.Stop();
			}
		}

		void plugTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var sel = plugTree.SelectedItem as TreeViewItem;
			if (sel != null)
			{
				var t = sel.Tag as LibraryComponent;
				t.ChildrenToggled(sel.Header.ToString());
				//library togs

				if (t.Caption == " My Library" && grdDD.Visibility == Visibility.Hidden)
				{
					Tabs.FadeIn();
					Container.FadeOut();
					switch (sel.Header.ToString())
					{
						case " Songs":
							EnumerationsContainer.LibraryUi = LibraryUiMode.Songs;
							break;
						case " Artists":
							EnumerationsContainer.LibraryUi = LibraryUiMode.Artist;
							break;
						case " Albums":
							EnumerationsContainer.LibraryUi = LibraryUiMode.Album;
							break;
						case " Genres":
							EnumerationsContainer.LibraryUi = LibraryUiMode.Genre;
							break;
					}
					RefreshUi();
				}
				else
				{
					Tabs.FadeOut();
					Container.FadeIn();
					LibraryComponent lc = sel.Tag as LibraryComponent;

					if (!lc.Initialized) lc.Initialize();
					if (!Container.Children.Contains(lc.ComponentUI))
					{
						if (lc.ComponentUI != null) Container.Children.Add(lc.ComponentUI);
					}
					foreach (Control v in Container.Children)
					{
						if (v != lc.ComponentUI)
						{
							v.FadeOut();
						}
					}
					lc.ComponentUI.FadeIn();
				}
			}
		}


		private void ComponentOnStateChangedEvent(LibraryComponent sender, LibraryComponentState state)
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
			{
				//add active items
				if (state == LibraryComponentState.Active)
				{
					TreeViewItem item = new TreeViewItem();
					item.Header = ((LibraryComponent)sender).Caption;
					item.Tag = sender;

					plugTree.Items.Add(item);

					return;
				}
				//remove inactive items
				for (int index = 0; index < plugTree.Items.Count; index++)
				{
					Control vi = (Control)plugTree.Items[index];
					var libraryComponent = vi.Tag as LibraryComponent;
					if (libraryComponent != null)
					{
						if (sender == libraryComponent)
						{
							switch (state)
							{
								case LibraryComponentState.Active:

									break;
								case LibraryComponentState.Inactive:
									plugTree.Items.Remove(vi);
									libraryComponent.ComponentUI.FadeOut();
									break;
								case LibraryComponentState.Hidden:

									break;
								case LibraryComponentState.Static:

									break;
								default:
									throw new ArgumentOutOfRangeException("state");
							}
						}
					}
				}

			}));
		}

		//TreeViewItem GeTile(LibraryComponent lc)
		//{

		//	TreeViewItem tv = new TreeViewItem();
		//				   tv.items
		//	nTile.Width = 95;
		//	nTile.Height = 95;

		//	Grid gd = new Grid();
		//	gd.Height = 85;
		//	gd.Width = 95;
		//	if (lc.Type == ComponentType.Static)
		//	{
		//		TextBlock ic = new TextBlock();
		//		ic.Text = lc.Icon;
		//		ic.TextAlignment = TextAlignment.Center;
		//		ic.VerticalAlignment = VerticalAlignment.Top;
		//		ic.HorizontalAlignment = HorizontalAlignment.Center;
		//		ic.Margin = new Thickness(0, 10, 0, 0);
		//		ic.FontSize = 40;
		//		ic.FontFamily = App.Current.MainWindow.FontFamily;
		//		gd.Children.Add(ic);
		//	}
		//	else
		//	{
		//		ProgressRing pg = new ProgressRing();
		//		pg.Width = 35;
		//		pg.Height = 35;
		//		pg.VerticalAlignment = VerticalAlignment.Top;
		//		pg.HorizontalAlignment = HorizontalAlignment.Center;
		//		pg.IsActive = true;
		//		pg.Margin = new Thickness(0, 10, 0, 10);
		//		gd.Children.Add(pg);
		//	}

		//	TextBlock ic2 = new TextBlock();
		//	ic2.Text = lc.Caption;
		//	ic2.TextAlignment = TextAlignment.Center;
		//	ic2.VerticalAlignment = VerticalAlignment.Bottom;
		//	ic2.HorizontalAlignment = HorizontalAlignment.Center;
		//	ic2.FontSize = 10;
		//	ic2.Margin = new Thickness(0, 10, 0, 10);
		//	gd.Children.Add(ic2);

		//	//nTile.SetResourceReference(BackgroundProperty, "GrayBrush8");
		//	nTile.Content.Children.Add( gd);
		//	 nTile.Background = null;
		//	nTile.Tag = lc;
		//	return nTile;
		//}

		public void ShowLibrary()
		{
			foreach (Control v in Container.Children)
			{
				v.FadeOut();
			}
			Tabs.FadeIn();
			Container.FadeOut();
		}

		public void RefreshPlugs()
		{
			////Remove Un-Related
			//for (int index = 0; index < plugTree.Items.Count; index++)
			//{
			//	var ct = plugTree.Items[index] as Control;
			//	if (ct != null)
			//	{
			//		var libraryComponent = ct.Tag as LibraryComponent;
			//		if (libraryComponent != null && libraryComponent.Category != LibraryComponentCategory.All)
			//		{
			//			if (libraryComponent.Category.ToString() != EnumerationsContainer.LibraryUi.ToString())
			//			{
			//				plugTree.Items.Remove(ct);
			//			}
			//		}
			//	}
			//}

			////Add Releated
			//var libraryComponents = PluginCore.Instance().LibraryComponents;
			//if (libraryComponents != null)
			//	foreach (var v in libraryComponents)
			//	{
			//		if (v.Type == ComponentType.Static &&
			//			(v.Category.ToString() == EnumerationsContainer.LibraryUi.ToString()))
			//		{
			//			if (!v.Initialized) v.Initialize();
			//			ToggleTile t = GeTile(v);
			//			plugs.Children.Add(t);
			//			t.MouseLeftButtonUp += t_Click;
			//			t.Opacity = 0;
			//			t.FadeIn();

			//		}
			//	}
		}
		#endregion
		#region Cell Edit

		private void tracklist_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{

			var tracky = tracklist.SelectedItem as TrackViewModel;
			if (tracky != null)
			{

				switch (e.Column.Header.ToString())
				{
					case "Song":
						tracky.Title = (e.EditingElement as TextBox).Text;
						break;
					case "Artist":
						tracky.Artist = (e.EditingElement as TextBox).Text;
						break;
					case "Album":
						tracky.Album = (e.EditingElement as TextBox).Text;
						break;
					case "Genre":
						tracky.Genre = (e.EditingElement as TextBox).Text;
						break;
				}

				tracky.SaveTrack();
				if (!TagSavePending.Contains(tracky)) TagSavePending.Add(tracky);
				SaveTags(tracky);



			}
		}

		private List<TrackViewModel> TagSavePending = new List<TrackViewModel>();

		private void SaveTags(TrackViewModel tracky)
		{

			int code = -1;
			try
			{
				foreach (var c in TagSavePending)
				{
					try
					{
						using (TagLib.File tlFile = TagLib.File.Create(c.Path))
						{
							tlFile.Tag.Title = c.Title;
							tlFile.Tag.Album = c.Album;
							tlFile.Tag.Performers = new string[] { c.Artist };
							tlFile.Tag.Genres = new[] { c.Genre };
							tlFile.Save();
						}
						TagSavePending.Remove(c);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}

			}
			catch (Exception ex)
			{


			}


		}

		private bool _editAllowed = false;
		private int _pfocusedRow;

		private void tracklist_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
		{
			//if (!_editAllowed)
			//{
			//	e.Cancel = true;
			//}
			//if (_pfocusedRow != tracklist.SelectedIndex) e.Cancel = true;
			//_editAllowed = false;
		}

		#endregion

		/// <summary>
		///     Automatically refresh the UI according to the UI Mode in the Views EnumerationContainer
		/// </summary>
		public void RefreshUi()
		{
			switch (EnumerationsContainer.LibraryUi)
			{
				case LibraryUiMode.Songs:
					Tabs.SelectedIndex = 0;
					break;
				case LibraryUiMode.Artist:
					Tabs.SelectedIndex = 1;
					break;
				case LibraryUiMode.Album:
					Tabs.SelectedIndex = 2;
					break;
				case LibraryUiMode.Genre:
					Tabs.SelectedIndex = 3;
					break;
				case LibraryUiMode.Playlist:
					Tabs.SelectedIndex = 4;
					break;

			}

		}

		/// <summary>
		///     Calculate the number of items to display on the previewtrack list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Results.previewCount = (int)(tracklist.ActualHeight / 10);
		}

		/// <summary>
		///     Open song when the track is double clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tracklist_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			tracklist.Tag = tracklist.SelectedIndex;
			Cycling.Items.trackSource = tracklist;

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

			}

		}

		/// <summary>
		/// Reduce scroll speed to 1 row
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void scrollView_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var d = (sender as ScrollViewer);
			d.ScrollToVerticalOffset((d.VerticalOffset + ((e.Delta < 0) ? +1 : -1)));
			e.Handled = true;
		}

		#region Update the lists realtime

		/// <summary>
		///     Update the list if the tracks collection has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Tabs.Visibility == Visibility.Hidden)
			{
				Tabs.FadeIn();
				grdDD.FadeOut();
			}
			//if (!Results.displayFreeze && EnumerationsContainer.LibraryUi == LibraryUiMode.Songs)
			Dispatcher.BeginInvoke(new Action(() => Scrolls(tracklist)));

		}
		DispatcherTimer dpt = new DispatcherTimer();
		private void Scrolls(DataGridControl sender)
		{
			try
			{
				if (sender.Items.Count < 1)
				{
					if (dpt.IsEnabled == false) dpt.Start();
					noresDelay = 0;
					noresults.Text = "Searching";
					noresults.Visibility = Visibility.Visible;
					tracklist.Visibility = Visibility.Hidden;
				}
				else
				{
					noresults.Visibility = Visibility.Hidden;
					tracklist.Visibility = Visibility.Visible;
				}
				//this is for the tracklist scrolling
				//since i want to scrll 1 row at a time and no jumping
				//added the handlers here coz at initialize the list is empty and 
				//no scrollbars are available :3
				if (_tracklist_scrollView == null)
				{
					_tracklist_scrollView = tracklist.GetScrollbar();
					//if (_tracklist_scrollView != null) _tracklist_scrollView.PreviewMouseWheel += scrollView_MouseWheel;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private ScrollViewer _tracklist_scrollView;

		/// <summary>
		///     Add songs to the library
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAddSongs_Click(object sender, RoutedEventArgs e)
		{
			LibraryAdder.Instance.FromFolder();

			LibraryAdder.Instance.PropertyChanged += LibraryAdder_PropertyChanged;
		}

		private void LibraryAdder_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() =>
				{
					var mw = Application.Current.MainWindow as MainWindow;

					switch (e.PropertyName)
					{
						case "Initialized":
							mw.SetProgressCaption(true, "Searching");
							break;
						case "Completed":
							if (mw != null) mw.SetProgressVisibility(false);
							break;
						case "$Progress":
							mw.SetProgressCaption(true, "Songs added: " + LibraryAdder.Instance.cval);
							break;
					}
				}));
		}

		#endregion

		public SearchPackage CurrentSearchPackage;
		private SearchPackage trackPackage;
		private SearchPackage artPackage;
		private SearchPackage albPackage;
		private SearchPackage genPackage;
		private int prevTabIndex = -1;
		private void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Tabs.SelectedIndex != prevTabIndex)
			{
				switch (Tabs.SelectedIndex)
				{
					case 0:
						if (trackPackage != CurrentSearchPackage)
						{
							TrackQuery.FindTracks(CurrentSearchPackage, Dispatcher);
							trackPackage = CurrentSearchPackage;
						}
						EnumerationsContainer.LibraryUi = LibraryUiMode.Songs;
						//Dispatcher.BeginInvoke(new Action(() => Scrolls(tracklist)));

						break;
					case 1:
						if (artPackage != CurrentSearchPackage)
						{
							ArtistQuery.FindArtists(CurrentSearchPackage, Dispatcher);
							artPackage = CurrentSearchPackage;
							artistList.RefreshUI();
						}
						EnumerationsContainer.LibraryUi = LibraryUiMode.Artist;

						break;
					case 2:
						if (albPackage != CurrentSearchPackage)
						{
							AlbumQuery.FindAlbums(CurrentSearchPackage, Dispatcher);
							albPackage = CurrentSearchPackage;
							albumLst.RefreshUI();
						}
						EnumerationsContainer.LibraryUi = LibraryUiMode.Album;

						break;
					case 3:
						if (genPackage != CurrentSearchPackage)
						{
							GenreQuery.FindGenres(CurrentSearchPackage, Dispatcher);
							albPackage = CurrentSearchPackage;
							genreLst.RefreshUI();

						}
						EnumerationsContainer.LibraryUi = LibraryUiMode.Genre;
						break;
				}

				RefreshPlugs();
				prevTabIndex = Tabs.SelectedIndex;
			}
		}

		private void tracklist_Sorting(object sender, System.Windows.Controls.DataGridSortingEventArgs e)
		{
			switch (e.Column.Header.ToString())
			{
				case "Songs":
					EnumerationsContainer.LibraryTrackFilter = TrackFilter.Songs;
					break;
				case "Artist":
					EnumerationsContainer.LibraryTrackFilter = TrackFilter.Artist;
					break;
				case "Album":
					EnumerationsContainer.LibraryTrackFilter = TrackFilter.Album;
					break;
				case "Genre":
					EnumerationsContainer.LibraryTrackFilter = TrackFilter.Genre;
					break;
				case "Bitrate":
					EnumerationsContainer.LibraryTrackFilter = TrackFilter.Bitrate;
					break;
				case "":
					EnumerationsContainer.LibraryTrackFilter = TrackFilter.Rate;
					break;
			}
			e.Handled = true;
			if (CurrentSearchPackage != null) TrackQuery.FindTracks(CurrentSearchPackage, Dispatcher);
			else
			{
				var mySearchType = SearchType.trYear;
				if (Settings.Default.SearchSongs) mySearchType |= SearchType.trTitle;
				if (Settings.Default.SearchArtists) mySearchType |= SearchType.trArtist;
				if (Settings.Default.SearchAlbums) mySearchType |= SearchType.trAlbum;
				if (Settings.Default.SearchGenres) mySearchType |= SearchType.trGenre;
				TrackQuery.FindTracks(
			 new SearchPackage
			 {
				 Data = "",
				 SearchType = mySearchType
			 }, Dispatcher);
			}
		}


		private void TrackFavChanged(object sender, MouseButtonEventArgs e)
		{
			tracklist.InvalidateVisual();

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
					db.Table<Track>()
						.FirstOrDefault(
							c => c.Path == t.Tag.ToString());
				if (query != null)
				{
					query.Rate = Rate;
					try
					{
						TrackQuery.SaveTrack(query);
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
				}
			}


		}
		#region Context Menus
		private void CellMouseRightButtonUpHandler(object sender, MouseButtonEventArgs e)
		{
			tracklist.SelectedItem = ((DataCell)sender).ParentRow.DataContext;
		}

		private void MenuShowInformation(object sender, RoutedEventArgs e)
		{
			var tracky = tracklist.SelectedItem as TrackViewModel;
			TrackMenu tm = new TrackMenu();
			tm.LoadFromTrack(tracky);
			tm.Show();
		}

		private void MenuShowInFolder(object sender, RoutedEventArgs e)
		{
			var tracky = tracklist.SelectedItem as TrackViewModel;
			if (tracky != null && File.Exists(tracky.Path))
			{
				FileInfo fi = new FileInfo(tracky.Path);
				string argument = @"/select, " + tracky.Path;
				System.Diagnostics.Process.Start("explorer.exe", argument);
			}
		}

		private void MenuRemoveFromLibrary(object sender, RoutedEventArgs e)
		{
			var tracky = tracklist.SelectedItem as TrackViewModel;
			if (Player.Instance.Wave != null && Player.Instance.Wave.Path == tracky.Path)
			{
				Player.Instance.Wave.Stop();
				Player.Instance.Wave.Dispose();
			}
			if (tracky != null && File.Exists(tracky.Path))
			{
				FileInfo fi = new FileInfo(tracky.Path);
				tracky.DeleteTrack();
				Results.Tracks.Remove(tracky);
			}
		}

		private void MenuDeleteFile(object sender, RoutedEventArgs e)
		{
			var tracky = tracklist.SelectedItem as TrackViewModel;
			if (Player.Instance.Wave != null && Player.Instance.Wave.Path == tracky.Path)
			{
				Player.Instance.Wave.Stop();
				Player.Instance.Wave.Dispose();
			}
			if (tracky != null && File.Exists(tracky.Path))
			{
				FileInfo fi = new FileInfo(tracky.Path);
				tracky.DeleteTrack();
				File.Delete(tracky.Path);
				Results.Tracks.Remove(tracky);
			}
		}

		private void MenuPlaySong(object sender, RoutedEventArgs e)
		{
			tracklist.Tag = tracklist.SelectedIndex;
			Cycling.Items.trackSource = tracklist;

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

			}
		}
		#endregion
	}
}