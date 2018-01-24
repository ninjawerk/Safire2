using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Safire.Library.Queries;
using Safire.Library.ViewModels;
using Xceed.Wpf.DataGrid;

namespace Safire.Library.Core
{
	class DragDropProcessor
	{
		public static List<TrackViewModel> ArtistProcessList(ListBox lst)
		{
			var art = lst.SelectedItem as ArtistViewModel;
			List<TrackViewModel> tl = TrackQuery.GetTracksFromArtist(art.Name).ToList();
			return tl;
		}
		public static List<TrackViewModel> ArtistProcessList(DataGridControl lst)
		{
			var art = lst.SelectedItem as ArtistViewModel;
			List<TrackViewModel> tl = TrackQuery.GetTracksFromArtist(art.Name).ToList();
			return tl;
		}
		public static List<TrackViewModel> AlbumProcessList(DataGridControl lst)
		{
			var art = lst.SelectedItem as AlbumViewModel;
			List<TrackViewModel> tl = TrackQuery.GetTracksFromAlbum(art.Name).ToList();
			return tl;
		}
		public static List<TrackViewModel> GenreProcessList(DataGridControl lst)
		{
			var art = lst.SelectedItem as ViewModels.GenreViewModel;
			List<TrackViewModel> tl = TrackQuery.GetTracksFromGenre(art.Name).ToList();
			return tl;
		}
	}
}
