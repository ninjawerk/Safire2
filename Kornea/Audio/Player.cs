using System;
using System.ComponentModel;
using Kornea.Audio.AudioCore;
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
        private static Player _myPlayer;

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

        public float Volume { get; set; }
        public StreamStatus StreamStatus { get; set; }
        public string Path { get; set; }
        public OutputMode Output { get; set; }

        public void Play()
        {
            if (Wave != null)
            {
                Wave.Play();
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

        private void Initialize()
        {
            Config.LoadConfigs();

            if (Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_LATENCY, IntPtr.Zero))
            {
                Config.LoadPlugins();

                AccessPermission.LymAudioLoaded = true;
            }
        }

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