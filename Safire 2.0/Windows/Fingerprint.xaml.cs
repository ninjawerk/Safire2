using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Kornea.Audio.AcousticID;
using Kornea.Audio.AcousticID.Audio;
using Kornea.Audio.AcousticID.Web;
using Kornea.Audio.AudioCore;
using MahApps.Metro.Controls;
using Safire.Animation;
using Safire.Controls.Window;
using Safire.Core;
using Safire.Library.Adder;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Safire.Views;

namespace Safire.Controls.ContextMenus
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class FingerPrint : MetroWindow
	{
		static IAudioDecoder decoder;
		private static string fp;
		private static int dur;

		public FingerPrint()
		{
			SupportSkinner.SetSkin(this);
			InitializeComponent();


			SupportSkinner.SetSkin(this);
			SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;

		}

		void SupportSkinner_OnSkinChanged()
		{
			SupportSkinner.SetSkin(this);

		}

		private TrackViewModel myTracky;

		void find(string str)
		{
			Kornea.Audio.AudioCore.Config.LoadConfigs();
			Config.LoadPlugins();
			decoder = new BassDecoder();

			if (!String.IsNullOrEmpty(str) && File.Exists(str))
			{
				decoder.Load(str);

				int bits = decoder.SourceBitDepth;
				int channels = decoder.SourceChannels;

				if (decoder.Ready)
				{

					Console.WriteLine(decoder.Duration.ToString());
					dur = decoder.Duration;
				}
				else
				{

				}

				if (String.IsNullOrEmpty(Kornea.Audio.AcousticID.Configuration.ApiKey))
				{
					// TODO: display a prompt for api key.
					Kornea.Audio.AcousticID.Configuration.ApiKey = "8XaBELgH";
				}
				ProcessFile(str);

			}
			else
			{
				Console.WriteLine("Problem with the path entered.");
			}

		}
		private void ProcessFile(string file)
		{
			if (File.Exists(file))
			{
				if (decoder.Ready)
				{
					Task.Factory.StartNew(() =>
					{
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();

						ChromaContext context = new ChromaContext();
						context.Start(decoder.SampleRate, decoder.Channels);
						decoder.Decode(context.Consumer, 120);

						context.Finish();

						stopwatch.Stop();
						ProcessFileCallback(context.GetFingerprint(), stopwatch.ElapsedMilliseconds);


					});
				}
				Lookup(fp, dur);
			}
		}
		private void ProcessFileCallback(string fingerprint, long time)
		{
			Action action = () =>
			{
				fp = fingerprint;
				// Release audio file handle.
				decoder.Dispose();
			};

			Dispatcher.BeginInvoke(action);


		}
		private void Lookup(string fingerprint, int duration)
		{


			LookupService service = new LookupService();

			service.GetAsync((results, e) =>
			{

				if (e != null)
				{
					UserRequest.ShowDialogBox(service.Error, "OK", "ERROR");
					return;
				}

				if (results.Count == 0)
				{
					if (String.IsNullOrEmpty(service.Error))
					{
						UserRequest.ShowDialogBox("No results for given fingerprint.", "OK", "ERROR");

					}
					else
						UserRequest.ShowDialogBox("Webservice error", "OK", "ERROR");
					return;
				}

				int max = 0;
				List<Recording> maxs = new List<Recording>();

				//foreach (var result in results)
				//{
				//	lst.Items.Add(result.Id);
				//	lst.Items.Add(result.Score);
				//	foreach (var v in result.Recordings)
				//	{
				//		if (v.Artists.Count > 0) lst.Items.Add(v.Title + "-" + v.Artists[0]);
				//	}
				//}

				//Calculation
				for (int index = 0; index < results.Count; index++)
				{
					var result = results[index];

					for (int i = 0; i < result.Recordings.Count; i++)
					{
						var v = result.Recordings[i];
						var cax = result.Recordings.Where(c => c.Title == v.Title && c.Artists == v.Artists).Count();
						if (cax > max)
						{
							maxs.Clear();
							maxs.Add(v);
							max = cax;
						}


					}
				}

				Dispatcher.BeginInvoke(new Action(() =>
				{
					grdProg.FadeOut();
					foreach (var recording in maxs)
					{
						BestResult = recording;
						res.Text = recording.Title + " by " + recording.Artists[0].Name;
					}
					grdResult.FadeIn();
				}));




			}, fingerprint, duration, new string[] { "recordings", "compress" });

		}

		public Recording BestResult;
		private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			Close();
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			BestResult = null;
			Close();
		}



		public static Recording GetTags(string path)
		{
			FingerPrint fp = new FingerPrint();
			fp.find(path);
			fp.ShowDialog();
			return fp.BestResult;
		}

	}
}