using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Bridge;
using Safire.Core.Plugins;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.SQLite;
using Playlist = Bridge.Playlist;

namespace Safire.Core.Playlists
{
	internal class PlaylistManager
	{
		public string CPlaylistID = "0";

		public Playlist GetFirst()
        {
            using (var db = new SQLiteConnection(Tables.DBPath))
            {
                try
                {
                    if (db.Table<Library.TableModels.Playlist>().Any())
                    {
						var ext = new FileInfo(db.Table<Library.TableModels.Playlist>().First().Path).Extension.ToLower();

	                    Playlist m3 = PluginCore.Instance().GetPlaylist(ext);
	                    m3.Path = db.Table<Library.TableModels.Playlist>().First().Path;
	                    m3.ID = db.Table<Library.TableModels.Playlist>().First().ID;                            
                                CPlaylistID = m3.ID;
                                return m3;                          
                        }
                   
                }
                catch
                {
                }
            }
            return null;
        }

		public IEnumerable<Playlist> GetAllPlaylists()
		{
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				if (db.Table<Library.TableModels.Playlist>().Any())
				{
					foreach (Library.TableModels.Playlist v in db.Table<Library.TableModels.Playlist>())
					{
						if (v.Path != null)
						{
							Playlist m3 = PluginCore.Instance().GetPlaylist(new FileInfo(v.Path).Extension.ToLower());
							m3.Path = v.Path;
							m3.ID = v.ID;
							m3.Name = v.Name;
							CPlaylistID = m3.ID;
							yield return m3;
						}
					}
				}
			}
		}

		public string NewUntitledPlaylist()
		{
			string ret = "Untitled Playlist ";
			int c = -1;
			foreach (var v in GetAllPlaylists())
			{
				if (v.Name.ToLower().Contains("untitled playlist"))
				{
					string n = v.Name.ToLower().Replace("untitled playlist ", "");
					if (n != null)
					{
						try
						{
							int i = Convert.ToInt32(n);
							if (i >= c) c = ++i;
						}
						catch
						{
						}
					}
				}
			}
			if (c == -1) c = 1;
			return ret + c;
		}

		public Playlist GetPlaylistfromPath(string path)
		{
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				try
				{
					if (db.Table<Library.TableModels.Playlist>().Any())
					{


						Playlist m3 =
							PluginCore.Instance()
								.GetPlaylist(
									new FileInfo(db.Table<Library.TableModels.Playlist>().Where(c => c.Path == path).First().Path).Extension.ToLower
										());
						m3.ID = db.Table<Library.TableModels.Playlist>().First().ID;
						m3.Path = db.Table<Library.TableModels.Playlist>().First().Path;
						CPlaylistID = m3.ID;
						return m3;


					}
				}
				catch
				{
				}
			}
			return null;
		}

		 

		public void Save(Bridge.Playlist playlist, List<TrackViewModel> lst)
		{
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				try
				{
					if (!string.IsNullOrEmpty(playlist.ID)) //already a member in DB
					{
						playlist.Write(TranslateList(lst)); //save to given path
						//Now check if the given path was the original path or
						//a different one
						//if (db.Table<Library.TableModels.Playlist>().Where(c => c.ID == playlist.ID).First().Path != playlist.Path)
						//{
							//delete old playlist, since new one is already saved
							string f = db.Table<Library.TableModels.Playlist>().Where(c => c.ID == playlist.ID).First().Path;
							//if (File.Exists(f)) File.Delete(f);
							var p = new Library.TableModels.Playlist();
							p.ID = playlist.ID;
							p.Path = playlist.Path;
							p.Name = playlist.Name;
							db.Update(p);
						//}

					}
					else //not a member in DB means its a new playlist
					{
						playlist.ID = Guid.NewGuid().ToString();
						playlist.Write(TranslateList(lst));
						var p = new Library.TableModels.Playlist();
						p.ID = playlist.ID;
						p.Path = playlist.Path;
						p.Name = playlist.Name;
						db.Insert(p);
					}
				}
				catch (Exception e)
				{
				}
			}
		}

		public static string RemoveIllegalChars(string s)
		{

			string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			s = r.Replace(s, "");
			return s;
		}
		public void RemovePlaylist(Playlist ipl)
		{
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				if (ipl != null)
				{
					var p = new Library.TableModels.Playlist();
					p.ID = ipl.ID;
					p.Path = ipl.Path;
					db.Delete(p);

				}
			}
		}
		private long GenerateID()
		{
			String s = DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.DayOfYear +
					   DateTime.Now.Year + "";
			return Convert.ToInt32(s);
		}

		public static List<Song>TranslateList(List<TrackViewModel> lst)
		{
			var d = new List<Song>();
			foreach (var c  in lst)
			{
							 d.Add(new Song(){Album = c.Album,Artist = c.Artist,Duration = c.Duration,Path = c.Path,Title = c.Title});
			}
			return d;
		}
		public static List<TrackViewModel> TranslateList(List<Song> lst)
		{
			var d = new List<TrackViewModel>();
			foreach (var c in lst)
			{
				d.Add(new TrackViewModel() { Album = c.Album, Artist = c.Artist, Duration = c.Duration, Path = c.Path, Title = c.Title });
			}
			return d;
		}
	}
}