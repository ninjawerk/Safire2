using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Safire.Core;
using Safire.Library.ObservableCollection;
using Safire.Library.ViewModels;

namespace Safire.Library.Cycling
{
	public enum CycleMode
	{
		Repeat,
		SongCycle,
		CycleRepeat,
		Shuffle
	}
	class Cycler
	{
		public static CycleMode Mode = CycleMode.SongCycle;
		public static void Initialize()
		{
			Player.Instance.PropertyChanged += Instance_PropertyChanged;
		}

		static void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			try
			{
				if (e.PropertyName == "TrackEnded" && !Player.Instance.Wave.Stopped)
				{
					switch (Mode)
					{
						case CycleMode.Repeat:
							{
								if (CoreMain.CurrentTrack != null)
								{
									var tracky = CoreMain.CurrentTrack;

									if (tracky != null && File.Exists(tracky.Path))
									{
										var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, Player.Instance.Volume);
										aw.ReactorUsageLocked = true;
										aw.Play();
										aw.ReactorUsageLocked = false;
										var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true,
											Player.Instance.Volume);
										fader.StartAndKill();

										Player.Instance.NewMedia(ref aw);
										//Player.Instance.Play();
									}
								}
							}
							break;
						case CycleMode.SongCycle:
							{
								if (Items.trackSource != null)
								{
									int ind = 0;
									if (Items.trackSource.Tag != null)
									{
										ind = (int)Items.trackSource.Tag;
										ind++;


										if (Items.trackSource.Items.Count < ind) return;
										Items.trackSource.Tag = ind;
										var tracky = Items.trackSource.Items[ind] as TrackViewModel;

										if (tracky != null && File.Exists(tracky.Path))
										{
											var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, Player.Instance.Volume);
											aw.ReactorUsageLocked = true;
											aw.Play();
											aw.ReactorUsageLocked = false;
											var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true,
												Player.Instance.Volume);
											fader.StartAndKill();

											Player.Instance.NewMedia(ref aw);
											//Player.Instance.Play();
										}
									}
								}
							}
							break;
						case CycleMode.CycleRepeat:
							{
								if (Items.trackSource != null)
								{
									int ind = (int)Items.trackSource.Tag;
									if (Items.trackSource.Tag != null) ind++;
									if (getEnuCount(Items.trackSource.ItemsSource) <= ind) ind = 0;

									var tracky = Items.trackSource.Items[ind] as TrackViewModel;
									Items.trackSource.Tag = ind;
									if (tracky != null && File.Exists(tracky.Path))
									{
										var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, Player.Instance.Volume);
										aw.ReactorUsageLocked = true;
										aw.Play();
										aw.ReactorUsageLocked = false;
										var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true,
																Player.Instance.Volume);
										fader.StartAndKill();

										Player.Instance.NewMedia(ref aw);
										//Player.Instance.Play();
									}
								}
							}
							break;
						case CycleMode.Shuffle:
							{

								if (Items.trackSource != null)
								{
									if (Items.trackSource.GetHashCode() != TrackSourceHash)
									{
										PrepShuffles(getEnuCount(Items.trackSource.ItemsSource));
										TrackSourceHash = Items.trackSource.GetHashCode();
									}
									int ind = (int)Items.trackSource.Tag;
									if (Items.trackSource.Tag != null) ind++;
									if (getEnuCount(Items.trackSource.ItemsSource) <= ind) ind = 0;

									var tracky = Items.trackSource.Items[Shuffles[ind]] as TrackViewModel;
									Items.trackSource.Tag = ind;
									if (tracky != null && File.Exists(tracky.Path))
									{
										var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, Player.Instance.Volume);
										aw.ReactorUsageLocked = true;
										aw.Play();
										aw.ReactorUsageLocked = false;
										var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true,
																Player.Instance.Volume);
										fader.StartAndKill();

										Player.Instance.NewMedia(ref aw);
										//Player.Instance.Play();
									}
								}
							}
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
		}
		static List<int> Shuffles = new List<int>();
		private static int TrackSourceHash = -1;
		public static int getEnuCount(dynamic e)
		{
			int result = 0;
			using (IEnumerator<TrackViewModel> enumerator = e.GetEnumerator())
			{
				while (enumerator.MoveNext())
					result++;
			}
			return result;
		}

		static void PrepShuffles(int count)
		{
			Shuffles.Clear();
			for (int i = 0; i < count; i++)
			{
				Shuffles.Add(i);
			}
			Shuffles.Shuffle();
		}

		public static void NextSong()
		{
			if (Items.trackSource != null)
			{
				int ind = 0;
				if (Items.trackSource.Tag != null)
				{
					ind = (int)Items.trackSource.Tag;
					ind++;


					if (Items.trackSource.Items.Count <= ind) return;
					Items.trackSource.Tag = ind;
					var tracky = Items.trackSource.Items[ind] as TrackViewModel;

					if (tracky != null && File.Exists(tracky.Path))
					{
						var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, 1.0f);
						aw.ReactorUsageLocked = true;
						aw.Play();
						aw.ReactorUsageLocked = false;
						var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true,
							Player.Instance.Volume);
						fader.StartAndKill();

						Player.Instance.NewMedia(ref aw);
						//Player.Instance.Play();
					}
				}
			}
		}

		public static void PrevSong()
		{
			if (Items.trackSource != null)
			{
				int ind = 0;
				if (Items.trackSource.Tag != null)
				{
					ind = (int)Items.trackSource.Tag;
					ind--;


					if (ind < 0) return;
					Items.trackSource.Tag = ind;
					var tracky = Items.trackSource.Items[ind] as TrackViewModel;

					if (tracky != null && File.Exists(tracky.Path))
					{
						var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, 1.0f);
						aw.ReactorUsageLocked = true;
						aw.Play();
						aw.ReactorUsageLocked = false;
						var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true,
							Player.Instance.Volume);
						fader.StartAndKill();

						Player.Instance.NewMedia(ref aw);
						//Player.Instance.Play();
					}
				}
			}
		}
	}
}
