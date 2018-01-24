using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Safire.Animation;
using Safire.Controls.Window;
using Safire.Core;
using Safire.Library.ObservableCollection;
using Safire.Library.Queries;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.Properties;
using Safire.SQLite;
using Application = System.Windows.Application;

namespace Safire.Library.Adder
{
	public class LibraryAdder : INotifyPropertyChanged
	{
		#region Fields

		private static LibraryAdder instance;
		public delegate void TrackAdded(TrackViewModel tk, Guid mGuid);
		private readonly BackgroundWorker cworker = new BackgroundWorker();
		//datasets
		private readonly TaglibCore tgl = new TaglibCore();
		public int cval;
		private IEnumerable<string> files;
		public int max;
		public string operation;
		private Thumbnail opt = new Thumbnail();
		private String path;
		public static HashSet<TrackViewModel> Tracks = new HashSet<TrackViewModel>();
		public static HashSet<ArtistViewModel> Artists = new HashSet<ArtistViewModel>();
		public static HashSet<AlbumViewModel> Albums = new HashSet<AlbumViewModel>();
		public static HashSet<ViewModels.GenreViewModel> Genres = new HashSet<ViewModels.GenreViewModel>();
		public Guid SearchGuid;
		public static bool DB_InUse = false;
		public static bool GlobalSearchCancel = false;
		#endregion

		#region Singleton Instance

		public static LibraryAdder Instance
		{
			get
			{
				if (instance == null)
					instance = new LibraryAdder();
				return instance;
			}
		}

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Constructor

		public LibraryAdder()
		{

			initialize_myworker();
			//if (Settings.Default.LibSearchCompleted == false)
			//{
			//    if (Settings.Default.LibSearchPath != "")
			//    {
			//        FromFolder(Settings.Default.LibSearchPath);
			//    }
			//    else
			//    {
			//        Settings.Default.LibSearchCompleted = true;
			//        Settings.Default.LibSearchPath = "";
			//    }
			//}
		}

		#endregion

		#region Methods

		/// <summary>
		///     Send Notifcations to the creator of this class.
		/// </summary>
		/// <param name="info"></param>
		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		/// <summary>
		///     Open Folder Dialog search.
		/// </summary>
		public void FromFolder()
		{
			if (cworker.IsBusy) return;
			cval = 0;
			max = 0;
			var m = new FolderBrowserDialog();
			string selpath;
			if (m.ShowDialog() == DialogResult.OK && m.SelectedPath != "")
			{
				UserRequest.ShowDialogBox(
					"Safire will add the songs from the specified locations, the progress can be seen from bottom of the library panel. Artists, Albums and Genres will be loaded after the search is complete.",
					"Ok");

				selpath = m.SelectedPath;
				//Settings.Default.LibSearchPath = selpath;
				//Settings.Default.LibSearchCompleted = false;
				Settings.Default.Save();
				//filesearch
				operation = "File search in progress...";
				NotifyPropertyChanged("$Progress");
				path = selpath;
				path = selpath;
				GlobalSearchCancel = false;
				cworker.RunWorkerAsync();
			}
		}

		public static event TrackAdded ListenToChanges;
		private static TrackAdded triggerCallback;
		/// <summary>
		///     Search from specified folder.
		/// Returns the GUID for this search.
		/// </summary>
		/// <param name="selpath">directory to start searching.</param>
		public Guid FromFolder(string selpath, TrackAdded callback)
		{
			if (cworker.IsBusy) return Guid.Empty; ;
			cval = 0;
			max = 0;
			operation = "File search in progress...";
			NotifyPropertyChanged("$Progress");
			path = selpath;
			GlobalSearchCancel = false;
			cworker.RunWorkerAsync();
			SearchGuid = Guid.NewGuid();
			triggerCallback = callback;
			return SearchGuid;
		}
		#endregion

		#region  Asynchronous MultiThreading

		/// <summary>
		///     Worker report progress
		/// </summary>
		private int ac;

		/// <summary>
		///     initialize my worker
		/// </summary>
		private void initialize_myworker()
		{
			cworker.DoWork += worker_load;
			cworker.RunWorkerCompleted += worker_completed;
			cworker.ProgressChanged += worker_progress;
			cworker.WorkerReportsProgress = true;
		}

		/// <summary>
		///     Workers load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void worker_load(object sender, DoWorkEventArgs e)
		{
			//Load lists
			//InitializeLists();
				   if (Results.Tracks==null)Results.Tracks= new ObservableCollection<TrackViewModel>();
				   if (Results.Artists == null) Results.Artists = new ObservableCollection<ArtistViewModel>();
				   if (Results.Albums == null) Results.Albums = new ObservableCollection<AlbumViewModel>();
				   if (Results.Genres == null) Results.Genres = new ObservableCollection<ViewModels.GenreViewModel>();

			DB_InUse = true;
			try
			{
				NotifyPropertyChanged("Initialized");
				App.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					if (ListenToChanges != null) ListenToChanges(new TrackViewModel() { Title = "Searching" }, SearchGuid);
				}));
				files =
					Search(@path, "*.*")
						.Where(
							s =>
							s.ToLower().EndsWith(".mp3") || s.ToLower().EndsWith(".wma") ||
							s.ToLower().EndsWith(".flac") || s.ToLower().EndsWith(".ogg")
							|| s.ToLower().EndsWith(".wav") || s.ToLower().EndsWith(".mp2") ||
							s.ToLower().EndsWith(".mp1") || s.ToLower().EndsWith(".m4a"));
				;
				max = files.Count();
				NotifyPropertyChanged("New Count");
			}

			catch
			{
			}
			//NotifyPropertyChanged("New Count");
			//tag data holding variables

			int mval = 0;
			if (files == null)
			// if there is nothin in files, return
			{
				//Settings.Default.LibSearchPath = "";
				//Settings.Default.LibSearchCompleted = true;
				Settings.Default.Save();
				return;
			}

			var artists = new Dictionary<string, ArtistViewModel>();
			var albums = new Dictionary<string, AlbumViewModel>();
			var genres = new Dictionary<string, ViewModels.GenreViewModel>();

			Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 1 }, file =>
			{
				if (GlobalSearchCancel) return;
				try
				{
					var d = new TrackViewModel();
					//Build tags
					TrackViewModel track = TaglibCore.BuildTrack(file);

					try
					{
						//Save Track                   
						if (!string.IsNullOrEmpty(track.Title) && track.SaveTrack() == "success")
						{
							App.Current.Dispatcher.BeginInvoke(new Action(() =>
							{
								if (SOFilter.ValidTrack(Results.TrackSearch, track))
								{
									if (!Results.Tracks.Contains(track))
										Results.Tracks.Add(track);
								}
							}));
						}
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}


					//==============
					//Artists
					//==============

					//Add the artist to dictionary from DB if the dictionary doesnt have already

					if (!artists.ContainsKey(track.Artist))
					{
						ArtistViewModel arts = ArtistQuery.GetArtistViewModel(track.Artist);

						if (arts != null)
							artists.Add(track.Artist, arts);
						else
						{
							artists.Add(track.Artist, new ArtistViewModel());
						}
					}

					//Do the operations for artists
					ArtistViewModel art = artists[track.Artist];
					art.Name = track.Artist;

					art.Duration += track.Duration;
					art.SongCount++;


					art.SaveArtist();

					App.Current.Dispatcher.BeginInvoke(new Action(() =>
					{
						if (SOFilter.ValidArtist(Results.TrackSearch, art))
						{
							ArtistViewModel exar;
							try
							{
								exar = Results.Artists.Single(c => c.Name == art.Name);
							}
							catch (Exception exception)
							{
								exar = null;
							}

							if (exar == null)
							{

								Results.Artists.Add(art);
							}
							else
							{
								exar.Listens = art.Listens;
								exar.Duration = art.Duration;
								exar.SongCount = art.SongCount;
								exar.Name = art.Name;

							}
						}
					}));
					//==============
					//Genre
					//==============

					//Add the album to dictionary from DB if the dictionary doesnt have already
					if (!genres.ContainsKey(track.Genre))
					{
						ViewModels.GenreViewModel al = GenreQuery.GetGenreViewModel(track.Genre);

						if (al != null)
							genres.Add(track.Genre, al);
						else
						{
							genres.Add(track.Genre, new ViewModels.GenreViewModel());
						}
					}
					ViewModels.GenreViewModel gens = genres[track.Genre];
					gens.Name = track.Genre;

					gens.Duration += track.Duration;
					gens.SongCount++;


					gens.SaveGenre();
					App.Current.Dispatcher.BeginInvoke(new Action(() =>
					{
						if (SOFilter.ValidGenre(Results.TrackSearch, gens))
						{
							ViewModels.GenreViewModel exar;
							try
							{
								exar = Results.Genres.Single(c => c.Name == gens.Name);
							}
							catch (Exception exception)
							{
								exar = null;
							}

							if (exar == null)
							{

								Results.Genres.Add(gens);
							}
							else
							{
								exar.Listens = art.Listens;
								exar.Duration = art.Duration;
								exar.SongCount = art.SongCount;
								exar.Name = art.Name;
							}
						}
					}));

					//==============
					//Albums
					//==============

					//Add the genre to dictionary from DB if the dictionary doesnt have already
					if (!albums.ContainsKey(track.Album))
					{
						AlbumViewModel al = AlbumQuery.GetAlbumViewModel(track.Album);

						if (al != null)
							albums.Add(track.Album, al);
						else
						{
							albums.Add(track.Album, new AlbumViewModel());
						}
					}
					AlbumViewModel albs = albums[track.Album];
					albs.Name = track.Album;
					albs.Artist = track.Artist;
					albs.Duration += track.Duration;
					albs.SongCount++;
					albs.ID = track.Album;

					albs.SaveAlbum();
					App.Current.Dispatcher.BeginInvoke(new Action(() =>
					{
						if (SOFilter.ValidAlbum(Results.TrackSearch, albs))
						{
							AlbumViewModel exar;
							try
							{
								exar = Results.Albums.Single(c => c.ID == albs.ID);
							}
							catch (Exception exception)
							{
								exar = null;
							}

							if (exar == null)
							{

								Results.Albums.Add(albs);
							}
							else
							{
								exar.Listens = art.Listens;
								exar.Duration = art.Duration;
								exar.SongCount = art.SongCount;
								exar.Name = art.Name;
							}
						}
					}));
					if (track.Album.ToLower() != "unknown album")
					{
						//Global.SaveAlbumArt(album + " - " + Artists, tgl.getArtStream());
						//opt.SaveThumb("My Library\\AlbumArt\\" + album + " - " + Artists + ".jpg.thumb");
					}

					if (!Tracks.Any(c => c.Path == track.Path)) Tracks.Add(track);
					if(!Artists.Any(c=>c.Name==art.Name))Artists.Add(art);
					if (!Albums.Any(c => c.Name == albs.Name)) Albums.Add(albs);
					if (!Genres.Any(c => c.Name == gens.Name)) Genres.Add(gens);

					//Playlist callbacks
					triggerCallback(track, SearchGuid);
					ListenToChanges(track, SearchGuid);
				}
				catch
				{
				}
				mval += 1;
				cworker.ReportProgress(mval);
			});

		}

		/// <summary>
		///     Worker completed job
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void worker_completed(object sender, RunWorkerCompletedEventArgs e)
		{
			DB_InUse = false;
			//EchoDirector.WindowProgress(false, "Adding Songs");
			NotifyPropertyChanged("Completed");
			//Settings.Default.LibSearchPath = "";
			//Settings.Default.LibSearchCompleted = true;
			Settings.Default.Save();
			if (ListenToChanges != null) ListenToChanges(new TrackViewModel() { Path = SearchGuid.ToString() }, SearchGuid);
		}

		private void worker_progress(object sender, ProgressChangedEventArgs e)
		{
			cval = e.ProgressPercentage;
			NotifyPropertyChanged("Progress");

			if (ac < e.ProgressPercentage)
			{
				//max = files.Count();
				ac = e.ProgressPercentage + 1;
				NotifyPropertyChanged("$Progress");
				operation = "Files Added = " + e.ProgressPercentage;
			}
		}

		#endregion

		#region Functions

		/// <summary>
		///     Search or files
		/// </summary>
		/// <param name="root"></param>
		/// <param name="searchPattern"></param>
		/// <returns></returns>
		private static IEnumerable<string> Search(string root, string searchPattern)
		{
			var dirs = new Queue<string>();
			dirs.Enqueue(root);
			while (dirs.Count > 0)
			{
				string dir = dirs.Dequeue();

				// files
				string[] paths = null;
				try
				{
					paths = Directory.GetFiles(dir, searchPattern);
				}
				catch
				{
				}

				if (paths != null && paths.Length > 0)
				{
					foreach (string file in paths)
					{
						yield return file;
					}
				}

				// sub-directories
				paths = null;
				try
				{
					paths = Directory.GetDirectories(dir);
				}
				catch
				{
				}

				if (paths != null && paths.Length > 0)
				{
					foreach (string subDir in paths)
					{
						dirs.Enqueue(subDir);
					}
				}
			}
		}

		private void InitializeLists()
		{
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				TableQuery<Track> query = db.Table<Track>().OrderBy(c => c.Title);
				foreach (Track track in query)
				{
					var etrack = new TrackViewModel
					{
						Path = track.Path,
						Title = track.Title,
						Artist = track.Artist,
						Album = track.Album,
						Bitrate = track.Bitrate,
						Composer = track.Composer,
						Duration = track.Duration,
						FileType = track.FileType,
						Channels = track.Channels,
						Genre = track.Genre,
						LastPlay = track.LastPlay,
						Listens = track.Listens,
						Lyrics = track.Lyrics,
						Meta = track.Meta,
						Rate = track.Rate,
						TrackPosition = track.TrackPosition,
						Year = track.Year,
					};
					Tracks.Add(etrack);
				}
			}

			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				TableQuery<Artist> query = db.Table<Artist>().OrderBy(c => c.Name);
				foreach (Artist track in query)
				{
					var etrack = new ArtistViewModel()
					{

						Duration = track.Duration,
						Name = track.Name,
						Image = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
				  @"\Safire\ArtistData\" + track.Name + ".jpg.thumb",
						AlbumCount = track.AlbumCount,
						SongCount = track.SongCount,
						Listens = track.Listens,
						Meta = track.Meta,
						Rate = track.Rate,
						LastPlay = track.LastPlayed
					};

					Artists.Add(etrack);
				}
			}

			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				TableQuery<Album> query = db.Table<Album>().OrderBy(c => c.Name);
				foreach (Album track in query)
				{
					var etrack = new AlbumViewModel()
					{
						Duration = track.Duration,
						Name = track.Name,
						Image = track.Image,
						SongCount = track.SongCount,
						Listens = track.Listens,
						Meta = track.Meta,
						Rate = track.Rate,
						LastPlay = track.LastPlayed,
						Artist = track.Artist,
						ID = track.ID
					};

					Albums.Add(etrack);
				}
			}

			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				TableQuery<Genre> query = db.Table<Genre>().OrderBy(c => c.Name);
				foreach (Genre track in query)
				{
					var etrack = new ViewModels.GenreViewModel()
					{
						Duration = track.Duration,
						Name = track.Name,
						Image = track.Image,
						SongCount = track.SongCount,
						Listens = track.Listens,
						Meta = track.Meta,
						Rate = track.Rate,
						LastPlay = track.LastPlayed,

					};

					Genres.Add(etrack);
				}
			}
		}
		#endregion

		public static TrackViewModel AddFile(string file)
		{
			try
			{
				var d = new TrackViewModel();
				//Build tags
				TrackViewModel track = TaglibCore.BuildTrack(file);

				try
				{
					//Save Track                   
					track.SaveTrack();

				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
				}

				//==============
				//Artists
				//==============
				//Do the operations for artists
				ArtistViewModel art = ArtistQuery.GetArtistViewModel(track.Artist);
				art = art ?? new ArtistViewModel();
				art.Name = track.Artist;
				art.Duration += track.Duration;
				art.SongCount++;
				art.SaveArtist();
				//==============
				//Genre
				//============== 
				ViewModels.GenreViewModel gens = GenreQuery.GetGenreViewModel(track.Genre);
				gens = gens ?? new ViewModels.GenreViewModel();
				gens.Name = track.Genre;
				gens.Duration += track.Duration;
				gens.SongCount++;
				gens.SaveGenre();

				//==============
				//Albums
				//==============

				AlbumViewModel albs = AlbumQuery.GetAlbumViewModel(track.Album);
				albs = albs ?? new AlbumViewModel();
				albs.Name = track.Album;
				albs.Artist = track.Artist;
				albs.Duration += track.Duration;
				albs.SongCount++;
				albs.ID = track.Album;

				albs.SaveAlbum();


				if (track.Album.ToLower() != "unknown album")
				{
					//Global.SaveAlbumArt(album + " - " + Artists, tgl.getArtStream());
					//opt.SaveThumb("My Library\\AlbumArt\\" + album + " - " + Artists + ".jpg.thumb");
				}

				Tracks.Add(track);
				Artists.Add(art);
				Albums.Add(albs);
				Genres.Add(gens);
				App.Current.Dispatcher.Invoke(new Action(() =>
				{
					var mw = App.Current.MainWindow as MainWindow;
					if (mw != null)
						if (mw.hLibrary.grdDD.Visibility == Visibility.Visible)
							mw.hLibrary.grdDD.FadeOut();

					if (SOFilter.ValidGenre(Results.TrackSearch, gens)) Results.Genres.Add(gens);
					if (SOFilter.ValidArtist(Results.TrackSearch, art)) Results.Artists.Add(art);
					if (SOFilter.ValidAlbum(Results.TrackSearch, albs)) Results.Albums.Add(albs);
					if (SOFilter.ValidTrack(Results.TrackSearch, track)) Results.Tracks.Add(track);
				}));
				return track;
			}
			catch
			{
			}
			return null;
		}
	}
}