using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Safire;
using Safire.Animation;
using Safire.Controls.Interactive;
using Safire.Library.TableModels;
using Safire.SQLite;

namespace libc_def.Top_Tracks
{
	/// <summary>
	/// Interaction logic for lcUI.xaml
	/// </summary>
	public partial class lcUI : UserControl
	{
		String DBPath = Path.Combine(
Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hyperLib.emlib");

		public lcUI()
		{
			InitializeComponent();
			lst.PreviewMouseDoubleClick += lst_PreviewMouseDoubleClick;
			lst2.PreviewMouseDoubleClick += lst2_PreviewMouseDoubleClick;
			lst3.PreviewMouseDoubleClick += lst3_PreviewMouseDoubleClick;
			lst4.PreviewMouseDoubleClick += lst4_PreviewMouseDoubleClick;
			btnLibrary.Click += showLibraryArtist;
						similars.MouseDoubleClick+=SimilarsOnMouseDoubleClick;
			if (Connectivity.CheckForInternetConnection())
			{
				int lcount = 0;
				grdLoading.FadeIn();
				ThreadPool.QueueUserWorkItem(_ =>
				{
					var la = Web.LFM.TopTracks.Util.GetTopTracks().track.Take(20);
					using (var db = new SQLiteConnection(DBPath))
					{
						foreach (var v in la)
						{
							var av =
								db.Table<Track>().Where(c => c.Title == v.name && c.Artist == v.artist.name);
							if (av.Count() > 0)
							{
								v.Available = "";
								v.Path = av.First().Path;
							}
						}
					}
					Dispatcher.Invoke(new Action(() =>
					{
						lst.ItemsSource = la;
						lcount++;
						if (lcount > 3)
						{
							grdLoading.FadeOut();
						}
					}));
				});
				ThreadPool.QueueUserWorkItem(_ =>
				{
					var lb = Web.LFM.HypedTracks.Util.GetHypedTracks().track.Take(20);
					using (var db = new SQLiteConnection(DBPath))
					{
						foreach (var v in lb)
						{
							var av =
								db.Table<Track>().Where(c => c.Title == v.name && c.Artist == v.artist.name);
							if (av.Count() > 0)
							{
								v.Available = "";
								v.Path = av.First().Path;
							}
						}
					}
					Dispatcher.Invoke(new Action(() =>
					{
						lst2.ItemsSource = lb;
						lcount++;
						if (lcount > 3)
						{
							grdLoading.FadeOut();
						}
					}));
				});
				ThreadPool.QueueUserWorkItem(_ =>
				{
					var lc = Web.LFM.TopArtists.Artists.GetTopArtists().artist.Take(20);
					using (var db = new SQLiteConnection(DBPath))
					{
						foreach (var v in lc)
						{
							var av =
								db.Table<Artist>().Where(c => c.Name == v.name).Count();
							if (av > 0)
							{
								v.Available = "";
							}
						}
					}
					Dispatcher.Invoke(new Action(() =>
					{
						lst3.ItemsSource = lc;
						lcount++;
						if (lcount > 3)
						{
							grdLoading.FadeOut();
						}
					}));
				});

				ThreadPool.QueueUserWorkItem(_ =>
				{
					var ld = Web.LFM.HypedArtists.Util.GetHypedArtists().artist.Take(20);
					using (var db = new SQLiteConnection(DBPath))
					{
						foreach (var v in ld)
						{
							var av =
								db.Table<Artist>().Where(c => c.Name == v.name).Count();
							if (av > 0)
							{
								v.Available = "";
							}
						}
					}
					Dispatcher.Invoke(new Action(() =>
					{
						lst4.ItemsSource = ld;
						lcount++;
						if (lcount > 3)
						{
							grdLoading.FadeOut();							
						}
					}));
				});
			}
		}

		private void SimilarsOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			var lcount = 0;
			var track = similars.SelectedItem as Web.LFM.ArtistInfo.Artist1;
			grdLoading.FadeIn();
			ThreadPool.QueueUserWorkItem(_ =>
			{
				using (var db = new SQLiteConnection(DBPath))
				{
				 
						var av =
							db.Table<Artist>().Where(c => c.Name == track.name).Count();
						if (av > 0)
						{
							track.Available = "";
						}
				 
				}
				var la = Web.LFM.ArtistTracks.Util.GetData(track.name);
				using (var db = new SQLiteConnection(DBPath))
				{
					foreach (var v in la.track)
					{
						var av =
							db.Table<Track>().Where(c => c.Title == v.name && c.Artist == v.artist.name);
						if (av.Count() > 0)
						{
							v.Available = "";
							v.Path = av.First().Path;
						}
					}
				}
				Dispatcher.Invoke(new Action(() =>
				{
					tracks.Children.Clear();
					foreach (var v in la.track)
					{
						if (string.IsNullOrEmpty(v.Available))
						{
							var nt = new TextBlock();
							nt.Text = v.TrackName;
							nt.Margin = new Thickness(10, 7, 0, 0);
							nt.Width = 200;
							nt.FontFamily = modeltemp.FontFamily;
							nt.TextTrimming = TextTrimming.CharacterEllipsis;
							nt.FontSize = 12;
							nt.Height = 15;
							tracks.Children.Add(nt);
						}
						else
						{
							var nt = new FluidLink();
							nt.Caption = " " + v.name;
							nt.Margin = new Thickness(10, 7, 0, 0);
							nt.Width = 200;
							nt.FontFamily = modeltemp.FontFamily;
							nt.FontSize = 12;
							nt.Height = 15;
							tracks.Children.Add(nt);
							nt.MouseDoubleClick += nt_Click;
							nt.Tag = v.Path;
						}
					}
					lcount++;
					if (lcount > 1)
					{
						grdLoading.FadeOut();
						artPan.FadeIn();
					}
				}));
			});
			ThreadPool.QueueUserWorkItem(_ =>
			{
				var ld = Web.LFM.ArtistInfo.Util.GetData(track.name);


				Dispatcher.Invoke(new Action(() =>
				{
					artName.Text = ld.name;
					artListens.Text = ld.stats.listeners + " listeners";
					var res = String.Join(", ", ld.tags.tag.Select(p => p.name.ToString()).ToArray());
					artTags.Text = res;
					similars.ItemsSource = ld.similar.artist;
					imgArt.Source = GetImage(track.IconImage);
					if (!String.IsNullOrEmpty(track.Available))
					{
						btnLibrary.FadeIn();
						btnLibrary.Tag = track.name;
					}
					else
					{
						btnLibrary.FadeOut();
					}
					lcount++;
					if (lcount > 1)
					{
						grdLoading.FadeOut();
						artPan.FadeIn();
					}
				}));
			});
 		}

		private void showLibraryArtist(object sender, RoutedEventArgs e)
		{
			var mw = App.Current.MainWindow as MainWindow;
			mw.LoadArtist((sender as Button).Tag.ToString());
		}

		void lst4_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var lcount = 0;
			var track = lst4.SelectedItem as Web.LFM.HypedArtists.Artist;
			grdLoading.FadeIn();
			ThreadPool.QueueUserWorkItem(_ =>
			{
				var la = Web.LFM.ArtistTracks.Util.GetData(track.name);
				using (var db = new SQLiteConnection(DBPath))
				{
					foreach (var v in la.track)
					{
						var av =
							db.Table<Track>().Where(c => c.Title == v.name && c.Artist == v.artist.name);
						if (av.Count() > 0)
						{
							v.Available = "";
							v.Path = av.First().Path;
						}
					}
				}
				Dispatcher.Invoke(new Action(() =>
				{
					tracks.Children.Clear();
					foreach (var v in la.track)
					{
						if (string.IsNullOrEmpty(v.Available))
						{
							var nt = new TextBlock();
							nt.Text = v.TrackName;
							nt.Margin = new Thickness(10, 7, 0, 0);
							nt.Width = 200;
							nt.FontFamily = modeltemp.FontFamily;
							nt.TextTrimming = TextTrimming.CharacterEllipsis;
							nt.FontSize = 12;
							nt.Height = 15;
							tracks.Children.Add(nt);
						}
						else
						{
							var nt = new FluidLink();
							nt.Caption = " " + v.name;
							nt.Margin = new Thickness(10, 7, 0, 0);
							nt.Width = 200;
							nt.FontFamily = modeltemp.FontFamily;
							nt.FontSize = 12;
							nt.Height = 15;
							tracks.Children.Add(nt);
							nt.MouseDoubleClick += nt_Click;
							nt.Tag = v.Path;
						}
					}
					lcount++;
					if (lcount > 1)
					{
						grdLoading.FadeOut();
						artPan.FadeIn();
					}
				}));
			});
			ThreadPool.QueueUserWorkItem(_ =>
			{
				var ld = Web.LFM.ArtistInfo.Util.GetData(track.name);


				Dispatcher.Invoke(new Action(() =>
				{
					artName.Text = ld.name;
					artListens.Text = ld.stats.listeners + " listeners";
					var res = String.Join(", ", ld.tags.tag.Select(p => p.name.ToString()).ToArray());
					artTags.Text = res;
					similars.ItemsSource = ld.similar.artist;
					imgArt.Source = GetImage(track.IconImage);
					if (!String.IsNullOrEmpty(track.Available))
					{
						btnLibrary.FadeIn();
						btnLibrary.Tag = track.name;
					}
					else
					{
						btnLibrary.FadeOut();
					}
					lcount++;
					if (lcount > 1)
					{
						grdLoading.FadeOut();
						artPan.FadeIn();
					}
				}));
			});

		}

		void lst3_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var lcount = 0;
			var track = lst3.SelectedItem as Web.LFM.TopArtists.Artists.Artist;
			grdLoading.FadeIn();
			ThreadPool.QueueUserWorkItem(_ =>
			{
				var la = Web.LFM.ArtistTracks.Util.GetData(track.name);
				using (var db = new SQLiteConnection(DBPath))
				{
					foreach (var v in la.track)
					{
						var av =
							db.Table<Track>().Where(c => c.Title == v.name && c.Artist == v.artist.name);
						if (av.Count() > 0)
						{
							v.Available = "";
							v.Path = av.First().Path;
						}
					}
				}
				Dispatcher.Invoke(new Action(() =>
				{
					tracks.Children.Clear();
					foreach (var v in la.track)
					{
						if (string.IsNullOrEmpty(v.Available))
						{
							var nt = new TextBlock();
							nt.Text = v.TrackName;
							nt.Margin = new Thickness(10, 7, 0, 0);
							nt.Width = 200;
							nt.FontFamily = modeltemp.FontFamily;
							nt.TextTrimming = TextTrimming.CharacterEllipsis;
							nt.FontSize = 12;
							nt.Height = 15;
							tracks.Children.Add(nt);
						}
						else
						{
							var nt = new FluidLink();
							nt.Caption = " " + v.name;
							nt.Margin = new Thickness(10, 7, 0, 0);
							nt.Width = 200;
							nt.FontFamily = modeltemp.FontFamily;
							nt.FontSize = 12;
							nt.Height = 15;
							tracks.Children.Add(nt);
							nt.MouseDoubleClick += nt_Click;
							nt.Tag = v.Path;
						}
					}
					lcount++;
					if (lcount > 1)
					{
						grdLoading.FadeOut();
						artPan.FadeIn();
					}
				}));
			});
			ThreadPool.QueueUserWorkItem(_ =>
			{
				var ld = Web.LFM.ArtistInfo.Util.GetData(track.name);
				Dispatcher.Invoke(new Action(() =>
				{
					artName.Text = ld.name;
					artListens.Text = ld.stats.listeners + " listeners";
					var res = String.Join(", ", ld.tags.tag.Select(p => p.name.ToString()).ToArray());
					artTags.Text = res;
					similars.ItemsSource = ld.similar.artist;
					imgArt.Source = GetImage(track.IconImage);
					if (!String.IsNullOrEmpty(track.Available))
					{
						btnLibrary.FadeIn();
						btnLibrary.Tag = track.name;
					}
					else
					{
						btnLibrary.FadeOut();
					}
					lcount++;
					if (lcount > 1)
					{
						grdLoading.FadeOut();
						artPan.FadeIn();
					}

				}));
			});

		}

		void nt_Click(object sender, RoutedEventArgs e)
		{
			var fluidLink = sender as FluidLink;
			if (fluidLink != null)
			{
				var track = fluidLink.Tag.ToString();
				if (track != null  )
				{
					var aw = new AudioWave(track, OutputMode.DirectSound, Player.Instance.Volume);
					aw.ReactorUsageLocked = true;
					aw.Play();
					aw.ReactorUsageLocked = false;
					var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
					fader.StartAndKill();

					Player.Instance.NewMedia(ref aw);
				}
			}
		}


		void lst2_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var track = lst2.SelectedItem as Web.LFM.HypedTracks.Track;
			if (track != null && !String.IsNullOrEmpty(track.Available))
			{
				var aw = new AudioWave(track.Path, OutputMode.DirectSound, Player.Instance.Volume);
				aw.ReactorUsageLocked = true;
				aw.Play();
				aw.ReactorUsageLocked = false;
				var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
				fader.StartAndKill();

				Player.Instance.NewMedia(ref aw);
			}
		}

		void lst_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var track = lst.SelectedItem as Web.LFM.TopTracks.Track;
			if (track != null && !String.IsNullOrEmpty(track.Available))
			{
				var aw = new AudioWave(track.Path, OutputMode.DirectSound, Player.Instance.Volume);
				aw.ReactorUsageLocked = true;
				aw.Play();
				aw.ReactorUsageLocked = false;
				var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
				fader.StartAndKill();

				Player.Instance.NewMedia(ref aw);
			}
		}

		private void jumpToCharts(object sender, RoutedEventArgs e)
		{
			artPan.FadeOut();
		}

		public BitmapSource GetImage(string value)
		{
			if (value != null)
			{

				var tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
			  @"\Safire\Cache\" + System.IO.Path.GetFileName((string)value);

				if (File.Exists(tString))
					return new BitmapImage(new Uri(tString));

				BitmapImage bi = new BitmapImage();

				bi.DownloadCompleted += (sender, args) =>
				{
					var encoder = new JpegBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(sender as BitmapImage));

					FileStream fs = new FileStream(tString, FileMode.Create);

					encoder.Save(fs);
					fs.Close();
				};
				bi.BeginInit();
				if (value.ToString() != "")
				{
					bi.UriSource = new Uri(value.ToString());


					bi.EndInit();


					return bi;
				}
				return null;
			}
			else
			{
				return null;
			}
		}
	}
}
