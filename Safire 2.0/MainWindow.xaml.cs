using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bridge;
using MahApps.Metro.Controls.Dialogs;
using Safire.Animation;
using Safire.Controls.Window;
using Safire.Core;
using Safire.Core.Playlists;
using Safire.Core.Plugins;
using Safire.Core.Services.TopArtists;
using Safire.Core.Updater;
using Safire.Fx;
using Safire.GUIs.Blinkies;
using Safire.Library.Adder;
using Safire.Library.Core;
using Safire.Library.Cycling;
using Safire.Library.Imaging;
using Safire.Library.ObservableCollection;
using Safire.Library.Queries;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.Properties;
using Safire.SettingsPages;
using Safire.SQLite;
using Safire.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Kornea.Windows;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Safire.Wizards;
using Un4seen.Bass;
using Application = System.Windows.Application;
using Control = System.Windows.Controls.Control;
using DataFormats = System.Windows.DataFormats;
using Image = System.Windows.Controls.Image;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ListView = System.Windows.Controls.ListView;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Playlist = Bridge.Playlist;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Safire
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		private readonly DispatcherTimer dpt = new DispatcherTimer();
		private readonly PlaylistManager playlistManager = new PlaylistManager();
		public Visibility PlaylistPrevState = Visibility.Hidden;
		private Accent currentAccent;
		private Bridge.Playlist prevPlaylist;
		private bool s = false;
		private RecentSongs recentSongs;
		#region Ctor - Detor

		private List<Bridge.Playlist> playlists;

		public MainWindow()
		{
			RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
			RenderOptions.SetCachingHint(this, CachingHint.Cache);
			RenderOptions.SetClearTypeHint(this, ClearTypeHint.Auto);

			RenderOptions.SetCacheInvalidationThresholdMaximum(this, 5);
			RenderOptions.SetCacheInvalidationThresholdMinimum(this, 1);
			SupportSkinner.SetSkin(this);
			SupportSkinner.TriggerSkinChanges(Settings.Default.AccentIndex, Settings.Default.BaseIndex);
			bool authorized = Settings.Default.AuTe;
			if (!authorized)
				new Welcome().ShowDialog();

			authorized = Settings.Default.AuTe;
			if (!authorized) App.Current.Shutdown();
			// new Kornea.Audio.Reactor.ReactorList().Show();
			Unloaded += MainWindow_Unloaded;


			Player.Instance.Volume = 1.0f; //Volume to full
			InitializeComponent();
			Main.Initialize();
			//fsBlink.PrepareFrame(new StandByFSBlinky(fsBlink));
			ShowToggle();
			ShowFxToggle();

			//Themes
			SupportSkinner.SetSkin(this);
			SupportSkinner.TriggerSkinChanges(Settings.Default.AccentIndex, Settings.Default.BaseIndex);

			//Set Player notif Property handler
			Player.Instance.PropertyChanged += Instance_PropertyChanged;

			//Create the timer for updating
			dpt.Interval = new TimeSpan(0, 0, 0, 0, 100);
			dpt.Tick += dpt_Tick;
			dpt.IsEnabled = true;
			dpt.Start();
			BackgroundImage.Source = Bitmap.GetImage(Main.MyPath() + "\\dbak.ee");

			//Hotkeys
			HookManager.KeyUp += HookManager_KeyUp;

			//Drag Drops
			Drop += MainWindow_Drop;
			AllowDrop = true;

			//
			Loaded += MainWindow_Loaded;

			//Load Plugins
			PluginCore.Instance().PluginsLoaded += MainWindow_PluginsLoaded;
			PluginCore.Instance().Compose();

			//Recent Songs
			recentSongs = new RecentSongs();
			recentSongs.UpdateRecentSongs += recentSongs_UpdateRecentSongs;


			updbgw.DoWork += bgw_DoWork;
			updbgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
			updbgw.RunWorkerAsync();
		}
		BackgroundWorker updbgw = new BackgroundWorker();
		/// <summary>
		/// update recent, & most & charts(once)
		/// </summary>
		void recentSongs_UpdateRecentSongs()
		{
			Artists.Artist art = null;
			ImageSource charts;
			ImageSource[] ims = new ImageSource[4];
			ImageSource[] mostims = new ImageSource[4];
			var tracks = recentSongs.tk.ToArray();
			BackgroundWorker b = new BackgroundWorker();
			List<Track> MostListened = null;

			//charts
			charts = chartArt.Source;

			b.DoWork += (sender, args) =>
			{
				int i = 0;
				//recent
				foreach (var track in tracks)
				{
					if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
									 @"\Safire\AlbumData\" + tracks[i].Album + " - " + tracks[i].Artist +
									 ".jpg"))
					{
						ims[i] = Bitmap.GetImage(Main.MyPath() + "\\resources\\unknown.png");
					}
					else
						ims[i] = Bitmap.GetImage(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
												   @"\Safire\AlbumData\" + tracks[i].Album + " - " + tracks[i].Artist + ".jpg");

					i++;
				}
				i = 0;

				//most listened
				using (var db = new SQLiteConnection(Tables.DBPath))
				{


					MostListened = db.Table<Track>()
				 .OrderByDescending(c => c.Listens).Take(4).ToList();
				}
				foreach (var track in MostListened)
				{
					if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
									 @"\Safire\AlbumData\" + track.Album + " - " + track.Artist +
									 ".jpg"))
					{
						mostims[i] = Bitmap.GetImage(Main.MyPath() + "\\resources\\unknown.png");
					}
					else
						mostims[i] = Bitmap.GetImage(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
												 @"\Safire\AlbumData\" + track.Album + " - " + track.Artist + ".jpg");

					i++;
				}

				//
				if (charts == null)
				{
					var rnd = new Random();
					var artists = Core.Services.TopArtists.Artists.GetTopArtists();
					if (artists != null)
					{

						art = artists.artist.Where(c => c.image[4] != null).ElementAt(rnd.Next(9));
						var tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
									  @"\Safire\Cache\m" + System.IO.Path.GetFileName(art.mbid);

						if (File.Exists(tString))
							charts = Bitmap.GetImage(tString);
						else
						{
							using (WebClient client = new WebClient())
							{
								client.DownloadFile(art.image[4].text, tString);
							}
							charts = Bitmap.GetImage(tString);
						}
					}

				}


			};

			b.RunWorkerCompleted += (sender, args) =>
			{
				int id = 0;

				//RECENT
				foreach (Tile c in recentWraps.Children)
				{

					var img = (c.Content as Grid).FindChildren<Image>().First();
					var tb = (c.Content as Grid).FindChildren<TextBlock>().First();
					c.BeginAnimation(UIElement.OpacityProperty, null);
					c.Opacity = 0;
					if (id < tracks.Count())
					{
						c.Tag = tracks[id];
						tb.Text = " " + tracks[id].Title;
						img.Source = ims[id];
						c.FadeIn();
					}
					else
						c.FadeOutC();
					id++;
				}

				//MOST
				id = 0;
				foreach (Tile c in mostWraps.Children)
				{
					var img = (c.Content as Grid).FindChildren<Image>().First();
					var tb = (c.Content as Grid).FindChildren<TextBlock>().First();
					c.BeginAnimation(UIElement.OpacityProperty, null);
					c.Opacity = 0;
					if (id < MostListened.Count())
					{

						c.Tag = MostListened.ElementAt(id);
						tb.Text = " " + MostListened.ElementAt(id).Title;
						img.Source = mostims[id];
						c.FadeIn();
					}
					else
						c.FadeOutC();
					id++;
				}
				if (chartArt.Source == null)
				{
					if (charts != null) { chartArt.Source = charts; chartsGrd.FadeIn(); }
					else chartsGrd.FadeOutC();

					if (art != null) icoChart.Text = "  " + art.name;
				}
				else chartsGrd.FadeIn();
				pancanvas.FadeIn();
			};
			b.RunWorkerAsync();
		}


		void MainWindow_PluginsLoaded()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				var libraryComponents = PluginCore.Instance().LibraryComponents;
				if (libraryComponents != null)
					foreach (var libraryComponent in libraryComponents)
					{
						if (libraryComponent.Type == ComponentType.Progressive) libraryComponent.Initialize();
					}

				//Scrobbler
				LastFm.Initialize();

				//playlists
				playlists = playlistManager.GetAllPlaylists().ToList();
				foreach (Playlist source in playlists)
				{
					playlistName.Items.Add(source.Name);
				}

				Playlist m3 = playlistManager.GetFirst();
				if (m3 != null)
				{
					foreach (TrackViewModel v in PlaylistManager.TranslateList(m3.Read()))
					{
						playlist.Items.Add(v);
					}

					playlistName.Text = m3.Name;
				}
				if (playlists.Count == 0) AddPlaylist(null, null);
				playlistName.SelectedIndex = Properties.Settings.Default.PlaylistID;


				float fval = (float)(Math.Pow(cvol.Value, 5) / Math.Pow(100, 5));

				Player.Instance.Volume = fval;

			}));
		}
		#region Updater
		void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!updatesAvailable) return;


			MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

			var result = UserRequest.ShowDialogBox("New updates available! Do you want to update now?",
			"Yes", "Not now", "Never", "UPDATE");

			if (result != ConfirmResult.Auxiliary)

				switch (result)
				{
					case ConfirmResult.Negative:

						break;
					case ConfirmResult.Affirmative:
						const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.

						ProcessStartInfo info = new ProcessStartInfo(CoreMain.MyPath() + @"\upd.exe");
						info.UseShellExecute = true;
						info.Verb = "runas";
						try
						{
							Process.Start(info);
							App.Current.Shutdown();
						}
						catch (Win32Exception ex)
						{
							if (ex.NativeErrorCode == ERROR_CANCELLED)
								this.ShowMessageAsync("Error", "Update was cancelled!");
						}
						break;
					case ConfirmResult.Auxiliary:
						Settings.Default.checkUpdates = false;
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
		}

		private bool updatesAvailable = false;
		void bgw_DoWork(object sender, DoWorkEventArgs e)
		{
			Definition myDefinition;
			if (Connectivity.CheckForInternetConnection() && Settings.Default.checkUpdates)
			{

				Definition cdef = Util.BinaryDeSerializeObject<Definition>(CoreMain.MyPath() + @"\myDef.bin");
				string xml;
				try
				{
					using (var w = new WebClient())
					{
						xml = w.DownloadString("http://www.lyracloud.com/Leyon/def.xml");
					}

					myDefinition = Util.Deserialize<Definition>(xml);
					if ((cdef != null && cdef.Version < (myDefinition.Version)))
					{
						updatesAvailable = true;
					}


				}
				catch
				{
				}
			}
		}
		#endregion
		void MainWindow_Drop(object sender, System.Windows.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				Thread nt = new Thread(o =>
				{
					foreach (var file in files)
					{
						if (Directory.Exists(file))
						{
							var mySearchGuid = new LibraryAdder().FromFolder(file, (tk, guid) =>
							{

							});
						}
						else if (File.Exists(file))
						{
							var tk = LibraryAdder.AddFile(file);

						}
					}
				});
				nt.Start();
			}
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				var cm1 = Environment.GetCommandLineArgs()[1];
				if (!string.IsNullOrEmpty(cm1))
				{
					var aw = new AudioWave(cm1, OutputMode.DirectSound, Player.Instance.Volume);
					aw.ReactorUsageLocked = true;
					aw.Play();
					aw.ReactorUsageLocked = false;
					var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
					fader.StartAndKill();

					Player.Instance.NewMedia(ref aw);
				}
			}
		}

		void HookManager_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Play:
					Player.Instance.Play();
					break;

				case Keys.Pause:
					Player.Instance.Pause();
					break;

				case Keys.MediaPlayPause:
					if (Player.Instance.Wave.StreamStatus == StreamStatus.CanPause) Player.Instance.Pause();
					else if (Player.Instance.Wave.StreamStatus == StreamStatus.CanPlay) Player.Instance.Play();

					break;

				case Keys.MediaStop:
					Player.Instance.Stop();
					break;

				case Keys.MediaNextTrack:
					Cycler.NextSong();
					break;

				case Keys.MediaPreviousTrack:
					Cycler.PrevSong();
					break;
			}
		}

		private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
		{
			Settings.Default.Save();
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			bool authorized = Settings.Default.AuTe;
			if (!authorized) return;
			// Enhance.Save();
			Settings.Default.EQValues = String.Join(",", FxHolder.EqValues.Select(i => i.ToString()).ToArray());

			CommitPlaylistChanges();
			try
			{
				Settings.Default.Save();
				var tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
		@"\Safire\recent.bin";
				SerialData.AbsBinarySerializeObject(tString, recentSongs.tk);
				Player.Instance.Stop();
				Player.Instance.ReleaseSystem();
				TaglibCore.WritePendingTracks();
				Application.Current.Shutdown();

			}
			catch
			{
			}
		}

		#endregion

		#region Nav

		/// <summary>
		///     Automatically refresh the UI according to the UI Mode in the Views EnumerationContainer
		/// </summary>
		public void RefreshUi()
		{
			switch (EnumerationsContainer.MainWindowUi)
			{
				case MainWindowUiMode.Library:
					//contEnhancer.FadeOut();
					hLibrary.FadeIn();
					Modes.AeroMode = false;
					break;
				case MainWindowUiMode.Play:
					//contEnhancer.FadeOut();
					hLibrary.FadeOut();
					Modes.AeroMode = true;
					break;
				case MainWindowUiMode.Fx:
					//contEnhancer.FadeIn();
					hLibrary.FadeOut();
					Modes.AeroMode = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				var metroTabItem = MainNav.SelectedItem as MetroTabItem;
				if (metroTabItem != null)
					switch (metroTabItem.Header.ToString())
					{
						case "Play":

							EnumerationsContainer.MainWindowUi = MainWindowUiMode.Play;
							break;

						case "Library":
							EnumerationsContainer.MainWindowUi = MainWindowUiMode.Library;
							if (!hLibrary.ComponentInitialized)
							{
								hLibrary.InitializeComponent();
								hLibrary.Initialize();
								hLibrary.ComponentInitialized = true;
							}
							break;
						case "Fx":
							EnumerationsContainer.MainWindowUi = MainWindowUiMode.Fx;
							if (!FxPanel.ComponentInitialized)
							{
								FxPanel.Initialize();
							}
							break;
					}
			}

			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
			RefreshUi();
		}

		#endregion

		#region Controls

		/// <summary>
		///     Seeker value changed, change the position of the song
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void seeker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (seeker.IsMouseOver & Mouse.LeftButton == MouseButtonState.Pressed)
			{
				if (Player.Instance.Wave != null) Player.Instance.Wave.Position = seeker.Value;
			}
		}


		/// <summary>
		///     Volume slider changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Player.Instance.Wave != null)
			{
				float fval = (float)(Math.Pow(cvol.Value, 5) / Math.Pow(100, 5));
				Player.Instance.Wave.Volume = fval;
				Player.Instance.Volume = fval;
			}
		}


		/// <summary>
		///     Update the UI from data, Timer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dpt_Tick(object sender, EventArgs e)
		{
			if (Player.Instance.Wave != null && !Player.Instance.NetStreamingConfigsLoaded)
			{
				seeker.Maximum = Player.Instance.Wave.Duration;
				seeker.Value = Player.Instance.Wave.Position;
			}
			//if (Player.Instance.Wave != null  )
			//{
			//	byte[]  b = new  byte[1024] ;
			//	var t = Bass.BASS_ChannelGetData(Player.Instance.Wave.Handle, b,1024);
			//	int total = 0;
			//	foreach (var bb in b)
			//	{
			//		total += bb;
			//	}
			//	var avg = total/1024.0;
			//	if (avg > volProg.Maximum) volProg.Maximum = avg;
			//	volProg.Value = avg;
			//}
		}

		/// <summary>
		///     blink grid back arrow click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BlinkBack(object sender, RoutedEventArgs e)
		{
			blinkGrid.Transit_Out();
		}

		/// <summary>
		///     Album art image click, bring up blinkgrid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AlbArt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			blinkGrid.PrepareFrame(new ImageViewer(blinkGrid, Artworks.GetAlbumArt()));
			blinkGrid.Transit_In();
		}

		/// <summary>
		///     Artist image click, bring up blink grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ArtistArt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			blinkGrid.PrepareFrame(new ImageViewer(blinkGrid, Artworks.GetArtistArt()));
			blinkGrid.Transit_In();
		}

		private void togScrobbleClick(object sender, RoutedEventArgs e)
		{
			Settings.Default.afx = !Settings.Default.afx;
			ShowFxToggle();
		}

		#endregion

		#region Progress Captions

		public void SetProgressCaption(bool show, string caption)
		{
			progPanel.Visibility = (show) ? Visibility.Visible : Visibility.Hidden;
			pgCaption.Text = caption;
			pgRing.IsActive = show;
		}

		public void SetProgressVisibility(bool show)
		{
			progPanel.Visibility = (show) ? Visibility.Visible : Visibility.Hidden;
			pgRing.IsActive = show;
		}

		#endregion

		#region Searching

		/// <summary>
		///     Search on Textbox text changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
		{


			var mySearchType = SearchType.trYear;
			if (Settings.Default.SearchSongs) mySearchType |= SearchType.trTitle;
			if (Settings.Default.SearchArtists) mySearchType |= SearchType.trArtist;
			if (Settings.Default.SearchAlbums) mySearchType |= SearchType.trAlbum;
			if (Settings.Default.SearchGenres) mySearchType |= SearchType.trGenre;

			Results.TrackSearch.SearchType = mySearchType;


			if (!hLibrary.ComponentInitialized)
			{
				hLibrary.InitializeComponent();
				hLibrary.Initialize();
				hLibrary.ComponentInitialized = true;

				hLibrary.RefreshUi();
			}
			else hLibrary.RefreshUi();
			//Bring the libraryview into list mode



			SearchPackage myPackage = new SearchPackage()
				{
					Data = txtSearch.Text,
					SearchType = mySearchType
				};

			hLibrary.CurrentSearchPackage = myPackage;
			if (EnumerationsContainer.LibraryUi == LibraryUiMode.Songs)
			{
				TrackQuery.FindTracks(
				   myPackage, Dispatcher);

			}

			if (EnumerationsContainer.LibraryUi == LibraryUiMode.Artist)
			{
				ArtistQuery.FindArtists(
					myPackage, Dispatcher);
				hLibrary.artistList.RefreshUI();
			}

			if (EnumerationsContainer.LibraryUi == LibraryUiMode.Album)
			{
				AlbumQuery.FindAlbums(
					myPackage, Dispatcher);
				hLibrary.albumLst.RefreshUI();
			}

			if (EnumerationsContainer.LibraryUi == LibraryUiMode.Genre)
			{
				GenreQuery.FindGenres(myPackage, Dispatcher);
				hLibrary.genreLst.RefreshUI();
			}

			MainNav.SelectedIndex = 1;
		}

		/// <summary>
		///     Textbox search, key down on the go searching
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtSearch_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				var mySearchType = SearchType.trYear;
				if (Settings.Default.SearchSongs) mySearchType |= SearchType.trTitle;
				if (Settings.Default.SearchArtists) mySearchType |= SearchType.trArtist;
				if (Settings.Default.SearchAlbums) mySearchType |= SearchType.trAlbum;
				if (Settings.Default.SearchGenres) mySearchType |= SearchType.trGenre;

				//Bring the libraryview into list mode




				Results.TrackSearch.SearchType = mySearchType;

				if (EnumerationsContainer.LibraryUi == LibraryUiMode.Songs) TrackQuery.FindTracks(
			  new SearchPackage
			  {
				  Data = txtSearch.Text,
				  SearchType = mySearchType
			  }, Dispatcher);

				if (EnumerationsContainer.LibraryUi == LibraryUiMode.Artist) ArtistQuery.FindArtists(
					  new SearchPackage
					  {
						  Data = txtSearch.Text,
						  SearchType = mySearchType
					  }, Dispatcher);

				if (EnumerationsContainer.LibraryUi == LibraryUiMode.Album) AlbumQuery.FindAlbums(
					new SearchPackage
					{
						Data = txtSearch.Text,
						SearchType = mySearchType
					}, Dispatcher);

				if (EnumerationsContainer.LibraryUi == LibraryUiMode.Genre) GenreQuery.FindGenres(
					new SearchPackage
					{
						Data = txtSearch.Text,
						SearchType = mySearchType
					}, Dispatcher);
			}
		}

		private void SearchMenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (!MnuSearchAlbums.IsChecked && !MnuSearchSongs.IsChecked && !MnuSearchArtists.IsChecked &&
				!MnuSearchGenres.IsChecked)
				MnuSearchSongs.IsChecked = true;
		}

		#endregion

		#region FlyOuts

		private void PlayListFlyOut(object sender, RoutedEventArgs e)
		{
			ToggleFlyout(0);
		}

		public void ToggleFlyout(int index)
		{
			var flyout = Flyouts.Items[index] as Flyout;
			if (flyout == null)
			{
				return;
			}

			flyout.IsOpen = !flyout.IsOpen;
		}

		public void ShowPlaylist()
		{
			var flyout = Flyouts.Items[0] as Flyout;
			if (flyout == null)
			{
				return;
			}

			flyout.IsOpen = true;
		}

		public void HidePlaylist()
		{
			var flyout = Flyouts.Items[0] as Flyout;
			if (flyout == null)
			{
				return;
			}

			flyout.IsOpen = false;
		}

		#endregion

		#region Playlists

		private bool _playlistNameChanged;
		private string playlistNewName = "";

		/// <summary>
		///     Play song when playlist is double clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Playlist_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//for cycler
			playlist.Tag = playlist.SelectedIndex;
			Items.trackSource = playlist;

			var tracky = playlist.SelectedItem as TrackViewModel;
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

		/// <summary>
		///     Save Playlist
		/// </summary>
		public void CommitPlaylistChanges()
		{
			if (playlists != null && (prevPlaylist == null && playlists.Count > 0))
			{
				prevPlaylist = playlists.First();
			}

			if (prevPlaylist == null)
				//reflect name changes
				if (_playlistNameChanged)
				{
					string fileName;
					var fin = new FileInfo(prevPlaylist.Path);
					string directory = fin.DirectoryName;
					if (prevPlaylist.Name != "")
					{
						fileName = fin.Name.Replace("." + fin.Extension, "").Replace(prevPlaylist.Name, playlistNewName);
					}
					else
						fileName = playlistNewName + "#G#" +
								   Guid.NewGuid() +
								   @".m3u";
					try
					{

						File.Delete(prevPlaylist.Path);
						prevPlaylist.Path = directory + "\\" + fileName;
					}
					catch
					{

					}

					_playlistNameChanged = false;
				}

			//save the playlist
			if (prevPlaylist != null)
			{
				var m3 = PluginCore.Instance().GetPlaylist("m3u");
				m3.Path = prevPlaylist.Path;

				var tr = new List<TrackViewModel>();
				m3.ID = (prevPlaylist).ID;
				m3.Name = (prevPlaylist).Name;
				foreach (object v in playlist.Items)
				{
					tr.Add(v as TrackViewModel);
				}

				//@@@@@@@@@@@@@@@@@@@@@@@@@
				playlistManager.Save(m3, tr);
				//@@@@@@@@@@@@@@@@@@@@@@@@@
			}
		}

		/// <summary>
		///     add a new playlist on '+' click
		///     *increment Untitled playlist by +1
		///     *if playlist file by same name exists, use name + GUID
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddPlaylist(object sender, RoutedEventArgs e)
		{
			string newName = playlistManager.NewUntitledPlaylist();
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\Safire\\" + newName +
						  @".m3u";

			//if file exists add GUID
			if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\Safire\\" + newName +
							@".m3u"))
			{
				path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\Safire\\" + newName + "#G#" +
					   Guid.NewGuid() +
					   @".m3u";
			}
			var m3 =
				  PluginCore.Instance().GetPlaylist("m3u");
			m3.Path = path;
			m3.Name = new FileInfo(path).Name.Replace(".m3u", "");
			playlistManager.Save(m3, new List<TrackViewModel>());
			playlist.ItemsSource = null;
			playlists.Add(m3);

			playlistName.Items.Add(m3.GetFileName());
			playlistName.Items.Refresh();
			playlistName.SelectedItem = m3.GetFileName();
		}

		/// <summary>
		///     Removes playlist.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RemovePlaylist(object sender, RoutedEventArgs e)
		{
			if (prevPlaylist != null) //playlist selected
			{
				playlistManager.RemovePlaylist(prevPlaylist);
				try
				{
					if (playlistName.Items.Count > 0)
					{
						playlistName.Items.RemoveAt(playlists.IndexOf(prevPlaylist));
						playlists.Remove(prevPlaylist);
					}
				}
				catch //because Indexof can throw error
				{
				}
			}
			else //nothing selected,,delete first
			{
				Playlist m = playlists.First();
				playlistManager.RemovePlaylist(m);
				if (playlistName.Items.Count > 0) playlistName.Items.RemoveAt(playlists.IndexOf(m));
				playlists.Remove(m);
			}

			//load the first playlist
			playlist.Items.Clear();
			if (playlists.Count == 0) return;
			Playlist m3 = (playlists.First());
			if (m3 != null)
			{
				playlist.Items.Clear();
				foreach (TrackViewModel v in PlaylistManager.TranslateList(m3.Read()))
				{
					playlist.Items.Add(v);
				}
				playlistName.Items.Refresh();
				try
				{
					playlistName.SelectedValue = m3.Name;
				}
				catch
				{
				}
			}
		}

		/// <summary>
		///     playlist changed, read and load new playlist
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void playlistName_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//not -1 aka not selected
			//list not empty
			//and not selecting the samething
			Properties.Settings.Default.PlaylistID = playlistName.SelectedIndex;
			if (playlistName.SelectedIndex != -1 && playlists.Count > playlistName.SelectedIndex &&
				prevPlaylist != playlists[playlistName.SelectedIndex])
			{
				if (prevPlaylist != null) CommitPlaylistChanges(); //save current
				else
				{
					prevPlaylist = playlists.First(); //nothing selected, save first
					CommitPlaylistChanges();
				}
				prevPlaylist = playlists[playlistName.SelectedIndex]; //set prev to selected playlist

				//clear and load
				Playlist m3 = (prevPlaylist);
				if (m3 != null)
				{
					playlist.Items.Clear();
					foreach (TrackViewModel v in PlaylistManager.TranslateList(m3.Read()))
					{
						playlist.Items.Add(v);
					}

					playlistName.Items.Refresh();
				}
			}
		}

		/// <summary>
		///     Playlist Name changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Playlist_TextChanged(object sender, TextCompositionEventArgs textCompositionEventArgs)
		{

			_playlistNameChanged = true;
			//set the prevPlaylist to current one. only nessacary if nothing was selected. so when selection changes it will be committed
			if (playlistName.SelectedIndex != -1 && playlists.Count > playlistName.SelectedIndex &&
				prevPlaylist != playlists[playlistName.SelectedIndex])
			{
				prevPlaylist = playlists[playlistName.SelectedIndex];

			}
			if (prevPlaylist != null)
			{
				Debug.WriteLine(playlistName.Text + textCompositionEventArgs.Text);
				playlistNewName = PlaylistManager.RemoveIllegalChars(playlistName.Text + textCompositionEventArgs.Text);
				if (playlistName.Items.Count > playlists.IndexOf(prevPlaylist))
					if (playlistName.Items[playlists.IndexOf(prevPlaylist)] != null)
						playlistName.Items[playlists.IndexOf(prevPlaylist)] = playlistNewName;
			}
			else if (playlistName.Items.Count > 0)
			{
				playlistNewName = PlaylistManager.RemoveIllegalChars(playlistName.Text + textCompositionEventArgs.Text);
				playlistName.SelectedItem = playlistNewName;
			}
			string newName = playlistName.Text + textCompositionEventArgs.Text;
			if (prevPlaylist != null) prevPlaylist.Name = newName;
		}

		/// <summary>
		///     Open playlist from fs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenLocalPlaylist(object sender, RoutedEventArgs e)
		{
			var opf = new OpenFileDialog();
			opf.Filter = "M3U Playlists|*.m3u";
			bool? d = opf.ShowDialog();
			if (opf.FileName != "")
			{
				var fino = new FileInfo(opf.FileName);
				string newName = fino.Name.Replace("." + fino.Extension, "");
				string path = opf.FileName;

				var m3 = PluginCore.Instance().GetPlaylist(fino.Extension);
				m3.Path = path;
				Regex r = new Regex("#G#");
				m3.Name = r.Split(Path.GetFileName(path).Replace(Path.GetExtension(path), ""))[0];


				playlistManager.Save(m3, new List<TrackViewModel>());
				playlist.Items.Clear();
				playlistName.Items.Add(m3.Name);
				playlistName.Items.Refresh();
				playlistName.SelectedItem = m3.Name;
				playlists.Add(m3);
				prevPlaylist = m3;
				foreach (TrackViewModel v in PlaylistManager.TranslateList(m3.Read()))
				{
					playlist.Items.Add(v);
				}
			}
		}

		/// <summary>
		///     save playlist to fs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveLocalPlaylist(object sender, RoutedEventArgs e)
		{
			var svf = new SaveFileDialog();
			var last = PluginCore.Instance().PlaylistModules.Last();
			foreach (var c in PluginCore.Instance().PlaylistModules)
			{
				svf.Filter += c.FileFormat + " | *." + c.FileExtension;
				if (c != last) svf.Filter += " | ";
			}
			svf.ShowDialog();

			if (svf.FileName != "" && svf.CheckPathExists)
			{
				var m3 = PluginCore.Instance().GetPlaylist(new FileInfo(svf.FileName).Extension.ToLower());
				m3.Path = svf.FileName;

				var tr = new List<TrackViewModel>();
				m3.ID = (playlists.Find(c => c.Name == playlistName.Text)).ID;
				foreach (object v in playlist.Items)
				{
					tr.Add(v as TrackViewModel);
				}
				m3.Write(PlaylistManager.TranslateList(tr));
			}
		}

		#endregion

		#region Audio
		TrackViewModel prevModel = new TrackViewModel();
		//Player property change handler
		private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Player.Instance.NetStreamingConfigsLoaded) grdNowPlaying.Visibility = Visibility.Collapsed;
			else grdNowPlaying.Visibility = Visibility.Visible;
			switch (e.PropertyName)
			{
				case "ActiveStreamHandle":
					//AlbArt.Source = Bitmap.GetImage(Main.MyPath() + "\\resources\\unknown.png");
					if (Player.Instance.NetStreamingConfigsLoaded || prevModel.Path == CoreMain.CurrentTrack.Path) break;
					ArtistArt.Source = null;
					//write pending tags 
					TaglibCore.WritePendingTracks();
					Thread tr = new Thread(() =>
					{
						var ctra = Core.CoreMain.CurrentTrack;
						var al = Artworks.GetAlbumArt();

						var aa = Artworks.GetArtistArt();

						//Search in lirbary and add if doesnt exist
						using (var db = new SQLiteConnection(Tables.DBPath))
						{
							if (!db.Table<Track>().Any(c => c.Path == CoreMain.CurrentTrack.Path))
							{
								LibraryAdder.AddFile(CoreMain.CurrentTrack.Path);
							}
						}


						if (ctra.Path == CoreMain.CurrentTrack.Path)
							Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
							{
								ArtistArt.Source = null;
								AlbArt.Source = al;
								ArtistArt.Source = aa;

								borArt.BeginAnimation(UIElement.OpacityProperty, null);
								borArt.Opacity = 0.0;

								borAlb.BeginAnimation(UIElement.OpacityProperty, null);
								borAlb.Opacity = 0.0;

								if (al != null)
									borAlb.FadeIn();
								else
									borAlb.FadeOutC();


								if (aa != null)
									borArt.FadeIn();
								else
									borArt.FadeOutC();
							}));

					});
					tr.Start();

					TblkTitle.Text = CoreMain.CurrentTrack.Title;
					TblkArtist.Text = CoreMain.CurrentTrack.Artist;
					TblkAlbum.Text = CoreMain.CurrentTrack.Album;

					icoArt.Text = "   " + CoreMain.CurrentTrack.Artist;
					icoGen.Text = " " + CoreMain.CurrentTrack.Genre;
					icoAlb.Text = " " + CoreMain.CurrentTrack.Album;
					prevModel = CoreMain.CurrentTrack;
					break;
				case "BufferProgress":
					if (CoreMain.CurrentNetStreamTrack.Title != null)
						TblkTitle.Text = CoreMain.CurrentNetStreamTrack.Title;
					TblkArtist.Text = String.Format("Buffering... {0}%", Player.Instance.Wave.BufferProgress);
					break;
				case "GoodBuffer":
					if (CoreMain.CurrentNetStreamTrack.Title != null)
						TblkTitle.Text = CoreMain.CurrentNetStreamTrack.Title;

					if (!String.IsNullOrEmpty(CoreMain.CurrentNetStreamTrack.Artist))
						TblkArtist.Text = CoreMain.CurrentNetStreamTrack.Artist;
					else TblkArtist.Text = CoreMain.CurrentNetStreamTrack.Bitrate + " kbps";
					break;
			}
		}

		#endregion

		#region Plugin Methods

		public void LoadArtist(string artistname)
		{
			hLibrary.artistList.LoadArtist(artistname);
			EnumerationsContainer.LibraryUi = LibraryUiMode.Artist;
			hLibrary.RefreshUi();
			hLibrary.ShowLibrary();
		}
		#endregion

		private void LaunchSettingsWindow(object sender, RoutedEventArgs e)
		{
			if (App.SettingsWindow == null) App.SettingsWindow = new SettingsWindow();
			App.SettingsWindow.Show();
			App.SettingsWindow.Activate();
		}

		#region TiledWall
		private void recenticon_MouseLeftButtonUp(object sender, RoutedEventArgs routedEventArgs)
		{
			var sng = ((sender as Tile).Tag as Track).Path;
			if (!string.IsNullOrEmpty(sng))
			{
				var aw = new AudioWave(sng, OutputMode.DirectSound, Player.Instance.Volume);
				aw.ReactorUsageLocked = true;
				aw.Play();
				aw.ReactorUsageLocked = false;
				var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
				fader.StartAndKill();
				Player.Instance.NewMedia(ref aw);
			}
		}

		private void mosticon_MouseLeftButtonUp(object sender, RoutedEventArgs routedEventArgs)
		{
			var sng = ((sender as Tile).Tag as Track).Path;
			if (!string.IsNullOrEmpty(sng))
			{
				var aw = new AudioWave(sng, OutputMode.DirectSound, Player.Instance.Volume);
				aw.ReactorUsageLocked = true;
				aw.Play();
				aw.ReactorUsageLocked = false;
				var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
				fader.StartAndKill();

				Player.Instance.NewMedia(ref aw);
			}
		}

		private Point startPoint;
		private Point mov = new Point(0, 0);
		private void canvasPan(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				pancanvas.RenderTransform = new TranslateTransform(mov.X + e.GetPosition(grdPan).X - startPoint.X, 0);
			}
		}

		private void panStart(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			startPoint = e.GetPosition(grdPan);
			mov.X = pancanvas.RenderTransform.Value.OffsetX;
		}

		private void topCharts_MouseLeftButtonUp(object sender, RoutedEventArgs e)
		{
			if (!hLibrary.ComponentInitialized)
			{
				hLibrary.InitializeComponent();
				hLibrary.Initialize();
				hLibrary.ComponentInitialized = true;
				hLibrary.RefreshUi();
				hLibrary.Tabs.FadeOut();
			}

			hLibrary.Tabs.FadeOut();
			LibraryComponent lc = Core.Plugins.PluginCore.Instance().LibraryComponents.First(c => c.Caption == " Top Charts");

			if (!lc.Initialized) lc.Initialize();

			if (!hLibrary.Container.Children.Contains(lc.ComponentUI))
			{
				if (lc.ComponentUI != null) hLibrary.Container.Children.Add(lc.ComponentUI);
			}

			foreach (Control v in hLibrary.Container.Children)
			{
				if (v != lc.ComponentUI)
				{
					v.FadeOut();
				}
			}

			lc.ComponentUI.FadeIn();
			MainNav.SelectedIndex = 1;
		}

		#endregion

		#region Cycle Toggle


		private void togCycleClick(object sender, System.Windows.RoutedEventArgs e)
		{
			Settings.Default.CycTogPos = ++Settings.Default.CycTogPos % 4;
			ShowToggle();
		}

		void ShowToggle()
		{
			switch (Settings.Default.CycTogPos)
			{
				case 0:
					Cycler.Mode = CycleMode.SongCycle;
					togCycle.Content = "";
					togCycle.ToolTip = "Play all songs in list.";
					togCycle.FontSize = 14;
					break;
				case 1:
					Cycler.Mode = CycleMode.Repeat;
					togCycle.Content = "";
					togCycle.ToolTip = "Repeat current song.";
					togCycle.FontSize = 14;
					break;
				case 2:
					Cycler.Mode = CycleMode.CycleRepeat;
					togCycle.Content = "";
					togCycle.ToolTip = "Repeat the current list.";
					togCycle.FontSize = 14;
					break;
				case 3:
					Cycler.Mode = CycleMode.Shuffle;
					togCycle.Content = "";
					togCycle.ToolTip = "Shuffle songs in the list.";
					togCycle.FontSize = 12;
					break;
			}
		}
		public void ShowFxToggle()
		{
			switch (Settings.Default.afx)
			{
				case true:


					togScrobble.ToolTip = "Audio effects ON";
					togScrobble.Opacity = 1;
					break;
				case false:


					togScrobble.ToolTip = "Audio effects OFF";
					togScrobble.Opacity = .5;
					break;

			}
		}
		#endregion


	}
}