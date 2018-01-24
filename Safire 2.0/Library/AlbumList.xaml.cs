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
using MahApps.Metro.Controls;
using Safire.Animation;
using Safire.Controls;
using Safire.Library;
using Safire.Library.Core;
using Safire.Library.Cycling;
using Safire.Library.ObservableCollection;
using Safire.Library.Queries;
using Safire.Library.ViewModels;
using Xceed.Wpf.DataGrid;

namespace Safire .Library
{
	/// <summary>
	///     Interaction logic for ArtistList.xaml
	/// </summary>
	public partial class AlbumList : UserControl
	{
		readonly XDGV_DragDropAutomation _dg_dda = new XDGV_DragDropAutomation();
		readonly XDG_DragDropAutomation _lb_dda = new XDG_DragDropAutomation();

		public AlbumList()
		{
			InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(this)) return;
			var albumViewModel = new AlbumsViewModel();
			Results.Albums = new ObservableCollection<AlbumViewModel>();
			if (Results.Albums != null)
				lst.ItemsSource = Results.Albums;
			if (Results.Albums != null) Results.Albums.CollectionChanged += Artists_CollectionChanged;
			albumViewModel.GetAlbums();
			lst.MouseLeftButtonDown += lst_MouseLeftButtonDown;
			//drags
			_dg_dda.Register(tracklist);
			_lb_dda.Register(lst,DragDropProcessor.AlbumProcessList);
		}

		void lst_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			MessageBox.Show(lst.SelectedItem.GetType().ToString());
		}

		public static String Album { get; set; }

		/// <summary>
		///     Update the list if the artist collection has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Artists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

				sender.Items.Refresh();
				if (sender.Items.Count <= 1)
				{
					noresults.Text = "No results for \"" + (App.Current.MainWindow as MainWindow).hLibrary.CurrentSearchPackage.Data + "\"";
					noresults.Visibility = Visibility.Visible;
					lst.Visibility = Visibility.Hidden;
				}
				else
				{
					noresults.Visibility = Visibility.Hidden;
					lst.Visibility = Visibility.Visible;
				}
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

		private void ListBoxItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			e.Handled = true;
		}

		private void lst_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
	 

			var art = lst.SelectedItem as AlbumViewModel;
			if (art != null)
			{

				artist.Text = art.Name;
				duration.Text = " " + TimeSpan.FromSeconds(art.Duration);
		 
				Album = art.Name;
				 
				tracklist.ItemsSource = TrackQuery.GetTracksFromAlbum(art.Name);
			 
				selArtist.FadeOut();
			}
	 
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