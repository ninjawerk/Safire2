using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Safire.Library.ViewModels;
using TagLib;
using File = TagLib.File;

namespace Safire.Core
{
	public class TaglibCore
	{

		#region Fields

		public File file;
		private string mypath;

		#endregion

		#region Functions

		/// <summary>
		///     Gets a bitmapframe of the current albumart.
		/// </summary>
		/// <returns></returns>
		public BitmapFrame getArtFrame()
		{
			try
			{
				Tag tag;
				tag = file.Tag;

				if (tag.Pictures.Length > 0)
				{
					using (var albumArtworkMemStream = new MemoryStream(tag.Pictures[0].Data.Data))
					{
						try
						{
							IPicture pic = tag.Pictures[0]; //pic contains data for image.
							var stream = new MemoryStream(pic.Data.Data);
							BitmapFrame bmp = BitmapFrame.Create(stream);

							return bmp;
						}
						catch
						{
							return null;
							// System.NotSupportedException:
							// No imaging component suitable to complete this operation was found.
						}
					}
				}
				else
				{
					return null;
				}
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		///     Gets a bitmap Memory Stream of the current albumart.
		/// </summary>
		/// <returns></returns>
		public MemoryStream getArtStream()
		{
			try
			{
				Tag tag;
				tag = file.Tag;
				if (tag.Pictures.Length > 0)
				{
					using (var albumArtworkMemStream = new MemoryStream(tag.Pictures[0].Data.Data))
					{
						try
						{
							IPicture pic = tag.Pictures[0]; //pic contains data for image.
							var stream = new MemoryStream(pic.Data.Data);
							tag = null;
							albumArtworkMemStream.Close();
							return stream;
						}
						catch
						{
							return null;
							// System.NotSupportedException:
							// No imaging component suitable to complete this operation was found.
						}
					}
				}
				else
				{
					return null;
				}
				tag = null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		///     Loads the specified file.
		/// </summary>
		/// <param name="filepath">The file to be loaded.</param>
		/// <returns></returns>
		public Boolean loadfile(string filepath)
		{
			try
			{

				using (file = File.Create(filepath))
				{

				}
				mypath = filepath;
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		///     Gets the title tag of the song.
		/// </summary>
		/// <returns></returns>
		public string GetTitle()
		{
			try
			{
				//create the file

				//retrieve the song title
				string rparam = file.Tag.Title;
				if (!(string.IsNullOrEmpty(rparam)))
				{
					return rparam;
				}
				else
				{
					return Path.GetFileName(mypath);
				}
			}
			catch
			{
				return Path.GetFileName(mypath);
			}
		}

		/// <summary>
		///     Gets the Artists tag of the song.
		/// </summary>
		/// <returns></returns>
		public string GetArtist()
		{
			try
			{
				//retrieve the song title
				string rparam = file.Tag.FirstPerformer;
				if (!(string.IsNullOrEmpty(rparam)))
				{
					return rparam;
				}
				else
				{
					return "Unknown Artist";
				}
			}
			catch
			{
				return "Unknown Artist";
			}
		}

		/// <summary>
		///     Gets the Artists tag of the song.
		/// </summary>
		/// <returns></returns>
		public string GetComposer()
		{
			try
			{
				//retrieve the song title
				string rparam = file.Tag.FirstComposer;
				if (!(string.IsNullOrEmpty(rparam)))
				{
					return rparam;
				}
				else
				{
					return "Unknown Composer";
				}
			}
			catch
			{
				return "Unknown Composer";
			}
		}

		/// <summary>
		///     Gets the album tag of the song.
		/// </summary>
		/// <returns></returns>
		public string GetAlbum()
		{
			try
			{
				//retrieve the song title
				string rparam = file.Tag.Album;
				if (!(string.IsNullOrEmpty(rparam)))
				{
					return rparam;
				}
				else
				{
					return "Unknown Album";
				}
			}
			catch
			{
				return "Unknown Album";
			}
		}

		/// <summary>
		///     Gets the genre tag of the song.
		/// </summary>
		/// <returns></returns>
		public string GetGenre()
		{
			try
			{
				//retrieve the song title
				string rparam = file.Tag.FirstGenre;
				if (!(string.IsNullOrEmpty(rparam)))
				{
					return rparam;
				}
				else
				{
					return "Unknown Genre";
				}
			}
			catch
			{
				return "Unknown Genre";
			}
		}

		/// <summary>
		///     Gets the year tag of the song.
		/// </summary>
		/// <returns></returns>
		public uint GetYear()
		{
			try
			{
				uint rparam = file.Tag.Year;
				return rparam;
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		///     Gets the duration tag of the song.
		/// </summary>
		/// <returns></returns>
		public int GetDuration()
		{
			try
			{
				int rparam = (int)file.Properties.Duration.TotalSeconds;

				return rparam;
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		///     Gets the bitrate tag of the song.
		/// </summary>
		/// <returns></returns>
		public uint GetBitrate()
		{
			try
			{
				var rparam = (uint)file.Properties.AudioBitrate;

				return rparam;
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		///     Gets the number of audio channels tag of the song.
		/// </summary>
		/// <returns></returns>
		public uint GetChannels()
		{
			try
			{
				var rparam = (uint)file.Properties.AudioChannels;

				return rparam;
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		///     Gets the track position in album tag of the song.
		/// </summary>
		/// <returns></returns>
		public uint GetTrackPosition()
		{
			try
			{
				uint rparam = file.Tag.Track;
				return rparam;
			}
			catch
			{
				return 0;
			}
		}

		#endregion

		static IDictionary<string, TrackViewModel> pendingTagWrites = new Dictionary<string, TrackViewModel>();

		public static TrackViewModel BuildTrack(string path)
		{
			var tags = new TaglibCore();
			tags.loadfile(Uri.UnescapeDataString(path));

			var track = new TrackViewModel
				{
					Path = path,
					Title = tags.GetTitle(),
					Artist = tags.GetArtist(),
					Album = tags.GetAlbum(),
					Genre = tags.GetGenre(),
					Year = tags.GetYear(),
					Composer = tags.GetComposer(),
					Bitrate = tags.GetBitrate(),
					TrackPosition = tags.GetTrackPosition(),
					Duration = tags.GetDuration(),
					Channels = tags.GetChannels(),
				};
			return track;
		}

		public static bool SaveTrack(TrackViewModel tv)
		{
			try
			{
				using (File f = File.Create(tv.Path))
				{
					f.Tag.Title = tv.Title;
					f.Tag.Album = tv.Album;
					f.Tag.Performers = new[] { tv.Artist };
					f.Tag.Genres = new[] { tv.Genre };
					f.Tag.Year = tv.Year;
					f.Tag.Composers = new[] { tv.Composer };
					f.Save();
				}
			}

			catch (Exception e)
			{
				if (!pendingTagWrites.ContainsKey(tv.Path)) pendingTagWrites.Add(tv.Path, tv);
				else
				{	//remove previous one, and add the latest one
					pendingTagWrites.Remove(tv.Path);
					pendingTagWrites.Add(tv.Path, tv);
				}
				return false;
			}
			return true;
		}

		public static void WritePendingTracks()
		{
			 

		}
	}
}

