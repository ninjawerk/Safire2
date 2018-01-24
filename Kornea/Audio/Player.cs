using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kornea.Audio.AcousticID.Web;
using Kornea.Audio.AudioCore;
using Kornea.Audio.DSP;
using Kornea.Audio.Interfaces;
using Kornea.Properties;
using Un4seen.Bass;

namespace Kornea.Audio
{   /// <summary>
	/// Deshan Alahakoon 2013 (C)
	/// Safire 2.0
	/// </summary>
	public class Player : IDisposable, INotifyPropertyChanged, IAudioWave
	{
		// PINNED
		private string _myUserAgent = "SKD_Deshan";
		[FixedAddressValueType()]
		public IntPtr _myUserAgentPtr;

		private static Player _myPlayer;
		private float _volume;

		public Player()
		{
			Initialize();
		}



		/// <summary>
		///     Return a Single instance of the
		/// </summary>
		public static Player Instance
		{
			get { return _myPlayer ?? (_myPlayer = new Player()); }
		}

		public AudioWave Wave { get; set; }

		public int Handle
		{
			get
			{
				if (Wave != null) return Wave.Handle;
				else return 0;
			}
		}

		public float Volume
		{
			get { return _volume; }
			set
			{
				_volume = value;

			}
		}

		public bool NetStreamingConfigsLoaded = false;

		public StreamStatus StreamStatus { get; set; }
		public string Path { get; set; }
		public OutputMode Output { get; set; }

		public void Play()
		{
			if (Wave != null)
			{
				Wave.Play();
				if (Config.Muted)
				{
					Wave.Volume = 0f;
					//Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL,0.0f);
				}
			}
		}

		public void Mute()
		{
			if (Config.Muted)
			{
				Config.Muted = false;
				Wave.Volume = Volume;

				//Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, Volume);
			}
			else
			{

				Wave.Volume = 0f;
				Config.Muted = true;
				//Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, 0.0f);
			}
		}

		public void Pause()
		{
			if (Wave != null)
			{
				Wave.Pause();
			}
		}

		public void Stop()
		{
			if (Wave != null)
			{
				Wave.Stop();
			}
		}

		public void Open()
		{
			if (Wave != null)
			{
				Wave.Open();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}


		public event PropertyChangedEventHandler PropertyChanged;

		#region Inits
		private void Initialize()
		{
			Config.LoadConfigs();
			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_DEV_DEFAULT, true);
			if (Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_LATENCY, IntPtr.Zero))
			{
				Config.LoadPlugins();
				NetStreamingConfigsLoaded = false;
				AccessPermission.LymAudioLoaded = true;
			}
		}

		public void ReleaseSystem()
		{

			// close bass
			Bass.BASS_Stop();
			Bass.BASS_Free();

		}

		public void NormalInit()
		{
			Initialize();
		}

		public void NetInit()
		{
			Bass.BASS_SetConfigPtr(BASSConfig.BASS_CONFIG_NET_AGENT, _myUserAgentPtr);

			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, 0); // so that we can display the buffering%
			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1);
			Config.LoadPlugins();
			if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
			{
				if (Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_WMA_PREBUF, 0) == false)
				{
					Console.WriteLine("ERROR: " + Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));
				}
				NetStreamingConfigsLoaded = true;
			}
		}

		#endregion
		public void NewMedia(ref AudioWave wave)
		{
			if (Wave != null && !wave.ReactorBound) Wave.Dispose();
			Wave = wave;
			wave.PropertyChanged += WaveOnPropertyChanged;
			OnPropertyChanged("ActiveStreamHandle");

		}

		private void WaveOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			OnPropertyChanged(propertyChangedEventArgs.PropertyName);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}