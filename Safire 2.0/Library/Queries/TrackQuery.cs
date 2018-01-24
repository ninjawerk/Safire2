using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Safire.Library.ObservableCollection;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;
using Safire.Views;

namespace Safire.Library.Queries
{
	public class TrackQuery
	{
		private static Thread tr;

		/// <summary>
		/// Get the track from from the path of a song
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static TrackViewModel GetTrackViewModel(string path)
		{
			var customer = new TrackViewModel();
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				Track _customer = (db.Table<Track>().Where(
					c => c.Path == path)).Single();
				customer.Path = _customer.Path;
				customer.Title = _customer.Title;
				customer.Artist = _customer.Artist;
				customer.Album = _customer.Album;
				customer.Bitrate = _customer.Bitrate;
				customer.Composer = _customer.Composer;
				customer.Duration = _customer.Duration;
				customer.FileType = _customer.FileType;
				customer.Channels = _customer.Channels;
				customer.Genre = _customer.Genre;
				customer.LastPlay = _customer.LastPlay;
				customer.Listens = _customer.Listens;
				customer.Lyrics = _customer.Lyrics;
				customer.Meta = _customer.Meta;
				customer.Rate = _customer.Rate;
				customer.TrackPosition = _customer.TrackPosition;
				customer.Year = _customer.Year;
			}
			return customer;
		}

		/// <summary>
		/// Get the track from from the path of a song
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Track GetTrack(string path)
		{

			Track _customer;
			try
			{
				using (var db = new SQLiteConnection(Tables.DBPath))
				{
					_customer = (db.Table<Track>().Where(
						c => c.Path == path)).Single();
					return _customer;
				}
			}
			catch (Exception)
			{

				return null;
			}

		}

		/// <summary>
		/// Save fresh track or update existing one
		/// </summary>
		/// <param name="track"></param>
		/// <returns></returns>
		public static string SaveTrack(Track track)
		{
			string result = string.Empty;
			LockManager lm = new LockManager();
			lm.PrepareForWrite();
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				try
				{
					//load the existing track
					Track existingTrack = (db.Table<Track>().Where(
						c => c.Path == track.Path)).SingleOrDefault();

					//if there is a track already
					if (existingTrack != null)
					{
						existingTrack.Path = track.Path;
						existingTrack.Title = track.Title;
						existingTrack.Artist = track.Artist;
						existingTrack.Album = track.Album;
						existingTrack.Bitrate = track.Bitrate;
						existingTrack.Composer = track.Composer;
						existingTrack.Duration = track.Duration;
						existingTrack.FileType = track.FileType;
						existingTrack.Channels = track.Channels;
						existingTrack.Genre = track.Genre;
						existingTrack.LastPlay = track.LastPlay;
						existingTrack.Listens = track.Listens;
						existingTrack.Lyrics = track.Lyrics;
						existingTrack.Meta = track.Meta;
						existingTrack.Rate = track.Rate;
						existingTrack.TrackPosition = track.TrackPosition;
						existingTrack.Year = track.Year;

						int success = db.Update(existingTrack);
					}
					else
					{
						//no track exists, make a new one
						int success = db.Insert(new Track
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
						});
					}
					result = "Success";
				}
				catch
				{
					result = "This customer was not saved.";
				}

			}
			lm.RemoveLock();
			return result;
		}

		/// <summary>
		/// Delete the track
		/// </summary>
		/// <param name="track"></param>
		/// <returns></returns>
		public static string DeleteTrack(Track track)
		{
			string result = string.Empty;
			LockManager lm = new LockManager();
			lm.PrepareForWrite();
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				//search for artist
				TableQuery<Artist> artist = db.Table<Artist>().Where(
					p => p.Name == track.Title);
				//search for album
				TableQuery<Album> album = db.Table<Album>().Where(
					p => p.ID == track.Album);
				//search for track
				Track existingTrack = (db.Table<Track>().Where(
					c => c.Path == track.Path)).Single();

				db.RunInTransaction(() =>
				{
					//delete artist record if this is the last song
					foreach (Artist ar in artist)
					{
						if (ar.SongCount == 1) db.Delete(ar);
					}

					//delete album record if this is the last song
					foreach (Album ar in album)
					{
						if (ar.SongCount == 1) db.Delete(ar);
					}

					if (db.Delete(existingTrack) > 0)
					{
						result = "Success";
					}
					else
					{
						result = "This customer was not removed";
					}
				});


				lm.RemoveLock();
			}
			return result;
		}

		/// <summary>
		/// Get all the tracks
		/// </summary>
		/// <returns></returns>
		public static ObservableCollection<TrackViewModel> GetTracks()
		{
			if (Results.Tracks == null) Results.Tracks = new ObservableCollection<TrackViewModel>();

			Results.Tracks.Clear();
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
					Results.Tracks.Add(etrack);
				}
			}
			return Results.Tracks;
		}

		/// <summary>
		/// Get the tracks from a specific artist
		/// </summary>
		/// <param name="artist"></param>
		/// <returns></returns>
		public static ObservableCollection<TrackViewModel> GetTracksFromArtist(string artist)
		{
			var Tracks = new ObservableCollection<TrackViewModel>();

			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				TableQuery<Track> query = db.Table<Track>().OrderBy(c => c.Title).Where(c => c.Artist == artist);
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
			return Tracks;
		}

		/// <summary>
		/// Get the tracks from a specific album
		/// </summary>
		/// <param name="album"></param>
		/// <returns></returns>
		public static ObservableCollection<TrackViewModel> GetTracksFromAlbum(string album)
		{
			var Tracks = new ObservableCollection<TrackViewModel>();

			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				TableQuery<Track> query = db.Table<Track>().OrderBy(c => c.Title).Where(c => c.Album == album);
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
			return Tracks;
		}

		/// <summary>
		/// Get the tracks from a specific album
		/// </summary>
		/// <param name="genre"></param>
		/// <returns></returns>
		public static ObservableCollection<TrackViewModel> GetTracksFromGenre(string genre)
		{
			var Tracks = new ObservableCollection<TrackViewModel>();

			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				TableQuery<Track> query = db.Table<Track>().OrderBy(c => c.Title).Where(c => c.Genre == genre);
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
			return Tracks;
		}

		/// <summary>
		/// Find tracks, searching, threaded.
		/// </summary>
		/// <param name="search"></param>
		/// <param name="dp"></param>
		public static void FindTracks(SearchPackage search, Dispatcher dp)
		{

			//JUMP START WARDEN//
			//if (!warden.IsAlive) warden.Start();

			var t = new Thread(Start);
			tickKEY = Environment.TickCount;
			var o = new object[] {search, dp, tickKEY, t};
			Results.displayFreeze = false;

			while (t.IsAlive) //very stupid,, from prev code.. didnt delete
			{
				t.Abort();

			}

			tr = null;

			//if (tr == null) tr = new Thread(Start);
			t.Start(o);

		}

		private static Thread warden = new Thread(StartWarden);

		private static void StartWarden()
		{
			while (true)
			{
				for (int i = 0; i < threads.Count; i++)
				{
					var thread = threads.ElementAt(i);
					if (thread.Key != tickKEY)
					{
						thread.Value.Abort();
						threads.Remove(thread.Key);
					}
				}
			}

		}

		private static Dictionary<long, Thread> threads = new Dictionary<long, Thread>();
		private static long tickKEY = 0;

		private static void Start(object o)
		{
			var ob = o as object[];
			var search = (SearchPackage) ob[0];
			var dp = (Dispatcher) ob[1];
			TableQuery<Track> query = null;
			var mkey = (long) ob[2];
			var mt = (Thread) ob[3];

			bool ftrack = search.SearchType.HasFlag(SearchType.trTitle);
			bool fArtist = search.SearchType.HasFlag(SearchType.trArtist);
			bool fAlbum = search.SearchType.HasFlag(SearchType.trAlbum);
			bool fGenre = search.SearchType.HasFlag(SearchType.trGenre);
			Results.TrackSearch = search;
			IEnumerable<TrackViewModel> nq = null;

			dp.BeginInvoke(new Action(() => Results.Tracks.Clear()));
			try
			{
				int i = 0;
				if (!Adder.LibraryAdder.DB_InUse)
				{
					using (var db = new SQLiteConnection(Tables.DBPath))
					{
						db.TimeExecution = true;
						query = doFilteredQuery(db, search);
						if (query != null)
						{
							Track lastone = query.Last();
							foreach (Track track in query)
							{
								if (String.IsNullOrEmpty(track.Title)) continue;
								if (Results.TrackSearch != search)
								{
									dp.Invoke(new Action(() => Results.Tracks.Clear()));
									return;
								}
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
								try
								{
									if (i++ > Results.previewCount)
									{
										Results.displayFreeze = true;
										dp.Invoke(new Action(() =>
										{
											if (lastone == track)
											{
												Results.displayFreeze = false;
												Results.Tracks.Add(etrack);
											}
											else
												Results.Tracks.Add(etrack);
										}));
									}
									else if (i == Results.previewCount)
									{
										Thread.Sleep(50);
									}
									else
									{
										dp.Invoke(new Action(() =>
										{
											if (lastone == track)
											{
												Results.displayFreeze = false;
												Results.Tracks.Add(etrack);
											}
											else
												Results.Tracks.Add(etrack);
										}));
									}
								}
								catch (Exception e)
								{
									Console.WriteLine(e);
								}
							}
						}

					}

				}
				else
				{
					nq = Adder.LibraryAdder.Tracks
						.OrderByDescending(c => c.Listens)
						.Where(
							c => (
								(ftrack && (c.Title.ToLower().Contains(search.Data.ToLower()))) ||
								(fArtist && (c.Artist.ToLower().Contains(search.Data.ToLower()))) ||
								(fAlbum && (c.Genre.ToLower().Contains(search.Data.ToLower()))) ||
								(fGenre && (c.Album.ToLower().Contains(search.Data.ToLower())))
								));


					var lastone = nq.Last();
					foreach (var etrack in nq)
					{
						if (Results.TrackSearch != search)
						{
							dp.Invoke(new Action(() => Results.Tracks.Clear()));
							return;
						}
						try
						{
							if (i++ > Results.previewCount)
							{
								Results.displayFreeze = true;
								dp.Invoke(new Action(() =>
								{
									if (lastone == etrack)
									{
										Results.displayFreeze = false;
										Results.Tracks.Add(etrack);
									}
									else Results.Tracks.Add(etrack);
								}));
							}
							else if (i == Results.previewCount)
							{
								Thread.Sleep(50);
							}
							else
							{
								dp.Invoke(new Action(() =>
								{
									if (lastone == etrack)
									{
										Results.displayFreeze = false;
										Results.Tracks.Add(etrack);
									}
									else
										Results.Tracks.Add(etrack);
								}));
							}
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
						}
					}
				}
				//  dp.Invoke(new Action(() => { if (tickKEY == mkey) Results.Tracks.Add(null); }));
			}
			catch (Exception)
			{

			}
			threads.Remove(mkey);
		}

		private static TableQuery<Track> doFilteredQuery(SQLiteConnection db, SearchPackage search)
		{
			bool ftrack = search.SearchType.HasFlag(SearchType.trTitle);
			bool fArtist = search.SearchType.HasFlag(SearchType.trArtist);
			bool fAlbum = search.SearchType.HasFlag(SearchType.trAlbum);
			bool fGenre = search.SearchType.HasFlag(SearchType.trGenre);
			TableQuery<Track> query = null;
			switch (EnumerationsContainer.LibraryTrackFilter)
			{
				case TrackFilter.Songs:
					query =
						db.Table<Track>()
							.OrderBy(c => c.Title)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				case TrackFilter.Artist:
					query =
						db.Table<Track>()
							.OrderBy(c => c.Artist)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				case TrackFilter.Album:
					query =
						db.Table<Track>()
							.OrderBy(c => c.Album)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				case TrackFilter.Genre:
					query =
						db.Table<Track>()
							.OrderBy(c => c.Genre)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				case TrackFilter.Rate:
					query =
						db.Table<Track>()
							.OrderByDescending(c => c.Rate)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				case TrackFilter.Bitrate:
					query =
						db.Table<Track>()
								.OrderByDescending(c => c.Bitrate)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				case TrackFilter.Duration:
					query =
						db.Table<Track>()
						 .OrderByDescending (c => c.Duration)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				case TrackFilter.AddedOn:
					query =
						db.Table<Track>()
							.OrderBy(c => c.LastPlay)
							.Where(
								c => (
									(ftrack && (c.Title.Contains(search.Data))) ||
									(fArtist && (c.Artist.Contains(search.Data))) ||
									(fAlbum && (c.Genre.Contains(search.Data))) ||
									(fGenre && (c.Album.Contains(search.Data)))
									));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return query;
		}
	}
}