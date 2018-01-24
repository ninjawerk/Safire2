using System;
using Safire.Library.ObservableCollection;
using Safire.Library.ViewModels;

namespace Safire.Library.Queries
{
	internal sealed class SOFilter
	{
		public static bool ValidTrack(SearchPackage searchPackage, TrackViewModel track)
		{
			bool valid = false;
			searchPackage.Data = (searchPackage.Data == null) ? "" : searchPackage.Data;

			if (track.Title.ToLower().Contains(searchPackage.Data)) valid = true;

			if (track.Artist.ToLower().Contains(searchPackage.Data)) valid = true;

			if (track.Album.ToLower().Contains(searchPackage.Data)) valid = true;

			if (track.Path.ToLower().Contains(searchPackage.Data)) valid = true;



			if (track.Composer.ToLower().Contains(searchPackage.Data)) valid = true;

			if (track.Genre.ToLower().Contains(searchPackage.Data)) valid = true;

			if (track.Lyrics.ToLower().Contains(searchPackage.Data)) valid = true;

			try
			{
				if (track.Bitrate == Convert.ToInt32(searchPackage.Data)) valid = true;

				if (track.Year == Convert.ToInt32(searchPackage.Data)) valid = true;

				if (track.Rate == Convert.ToInt32(searchPackage.Data)) valid = true;
			}
			catch (Exception e)
			{

			}


			return valid; //valid;
		}

		public static bool ValidArtist(SearchPackage searchPackage, ArtistViewModel artist)
		{
			bool valid = false;
			searchPackage.Data = (searchPackage.Data == null) ? "" : searchPackage.Data;
	 
					if (artist.Name.ToLower().Contains(searchPackage.Data)) valid = true;
		 
			 
			return valid; //valid;
		}
		public static bool ValidGenre(SearchPackage searchPackage, ViewModels.GenreViewModel gen)
		{
			bool valid = false;
			searchPackage.Data = (searchPackage.Data == null) ? "" : searchPackage.Data;
		 
					if (gen.Name.ToLower().Contains(searchPackage.Data)) valid = true;
				 
		 
			return valid; //valid;
		}
		public static bool ValidAlbum(SearchPackage searchPackage, AlbumViewModel album)
		{
			bool valid = false;
			searchPackage.Data = (searchPackage.Data == null) ? "" : searchPackage.Data;
 
					if (album.Name.ToLower().Contains(searchPackage.Data)) valid = true;
				 
			return valid; //valid;
		}
	}
}