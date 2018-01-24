using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;
using Kornea.Audio.AudioCore;
using Kornea.Audio.Interfaces;
using Kornea.Audio.Reactor;
using Kornea.Properties;
using Un4seen.Bass;
using Un4seen.Bass.Misc;

namespace Kornea.Audio
{
    public sealed class AudioWave : IDisposable, INotifyPropertyChanged, IAudioWave
    {

        public DispatcherTimer dpt = new DispatcherTimer(); 
        public string ErrorMessage = string.Empty;
        private bool Valid;
        private int _handle; //stream handle
        private StreamStatus _streamStatus; //streaming status (playing, can play, stopped)
        private float _volume;
        public bool ReactorUsageLocked = false;
        public List<BaseDSP > AttachedDSPs = new List<BaseDSP>();

        #region Ctor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="streamHandle">Recieve the stream handle</param>
        public AudioWave(string path, OutputMode _output, float vol = -999)
        {
            dpt.Interval = new TimeSpan(0, 0, 0, 1);
            dpt.Tick += dpt_Tick;
            dpt.Start();

            Path = path;
            Output = _output;
            if (vol == -999)
            {
                Volume = Player.Instance.Volume;
            }
            else
            {
                Volume = vol;
            }
            OnPropertyChanged("Stopped"); // to reset the controls
            OnPropertyChanged("Initialized"); // notify that the object is not loaded
        }

        private BASSActive prevStatus;
        void dpt_Tick(object sender, EventArgs e)
        {
            if (prevStatus != Bass.BASS_ChannelIsActive(Handle))
            {
                prevStatus = Bass.BASS_ChannelIsActive(Handle);
                if (prevStatus == BASSActive.BASS_ACTIVE_STOPPED) StreamStatus = StreamStatus.Stopped;
                OnPropertyChanged("StreamStatus");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Access the the stream handle
        /// </summary>
        public int Handle
        {
            get { return _handle; }
        }

        /// <summary>
        ///     Set the stream's volume
        /// </summary>
        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (!Muted) Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, _volume);

            }
        }

        /// <summary>
        ///     Contains the current state of the audio stream
        /// </summary>
        public StreamStatus StreamStatus
        {
            get { return _streamStatus; }
            set
            {
                _streamStatus = value;
                OnPropertyChanged("StreamStatus");
            }
        }
        /// <summary>
        /// Current songs file path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Output mode, Directsound, WASAPI...
        /// </summary>
        public OutputMode Output { get; set; }

        /// <summary>
        /// Is the channel on mute
        /// </summary>
        public bool Muted { get; set; }

        /// <summary>
        /// Is the audio wave currently used by a reactor.
        /// </summary>
        public bool ReactorBound
        {
            get
            {
                if (WorkingReactors.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Pool addreses of the reactors using this wave.
        /// </summary>
        public IList<string> WorkingReactors = new List<string>();

        public bool Stopped = false;

        #endregion

        #region Methods

        /// <summary>
        ///     Play stream
        /// </summary>
        public void Play()
        {
            //Check if the song is stopped
            if (Bass.BASS_ChannelIsActive(Handle) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                if (Valid) //Check if the stream handle is valid
                {
                    if (Bass.BASS_ChannelPlay(Handle, false))
                    {
                        StreamStatus = StreamStatus.CanPause;
                    }
                }
            }
            else //song is stopped, load again
            {
                Open(); //Open the file since its stopped
                if (Bass.BASS_ChannelPlay(Handle, false))
                {
                    StreamStatus = StreamStatus.CanPause;

                }
            }
            if (Config.ReactorFade & !ReactorUsageLocked)
            {
                var fader = new Fader(this, FadeMode.Linear, 5, 250, false, Volume);

                fader.Start();
            }
            if (!Muted) Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, _volume);
        }
        /// <summary>
        /// pause the current stream
        /// </summary>
        public void Pause()
        {
            if (Valid && StreamStatus == StreamStatus.CanPause)
            {
                //Check if the Fader is on, if so
                //First fade and then pause, else just pause
                if (Config.ReactorFade)
                {
                    var fader = new Fader(this, FadeMode.Linear, 5, 250, true, Volume);

                    fader.Start();
                    fader.ActionCompletedEvent += new Fader.ActionCompleted(delegate
                    {
                        if (Bass.BASS_ChannelPause(Handle))
                        {
                            StreamStatus = StreamStatus.CanPlay;

                        }
                    });
                }
                else // just pause
                {
                    if (Bass.BASS_ChannelPause(Handle))
                    {
                        StreamStatus = StreamStatus.CanPlay;
                    }
                }

            }
        }
        /// <summary>
        /// stop the current stream
        /// </summary>
        public void Stop()
        {
            var x = Bass.BASS_ChannelIsActive(Handle);
            if (Valid && Bass.BASS_ChannelIsActive(Handle) != BASSActive.BASS_ACTIVE_STOPPED)
            {
                if (Bass.BASS_ChannelStop(Handle))
                {
                    StreamStatus = StreamStatus.CanPlay;
                    Stopped = true;
                }
            }
        }
        /// <summary>
        /// Open the song, specified in the Path property
        /// </summary>
        public void Open()
        {
            switch (Output)
            {
                case OutputMode.DirectSound:
                    int newStreamHandle = Bass.BASS_StreamCreateFile(Path, 0, 0,
                                                                     BASSFlag.BASS_MUSIC_AUTOFREE | BASSFlag.BASS_MUSIC_FLOAT
                                                                     );

                    if (newStreamHandle != 0)
                    {
                        Stopped = false;
                        _handle = newStreamHandle;
                        OnPropertyChanged("StreamStatus");
                        OnPropertyChanged("ActiveStreamHandle");
                        Valid = true;
                        ended = false;
                    }
                    else
                    {
                        ErrorMessage = "Stream handle returned is zero ";
                        OnPropertyChanged("Error");
                    }

                    break;
                case OutputMode.WASAPI:
                    break;
                case OutputMode.ASIO:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("_output");
            }
        }

        public void Mute()
        {
            if (Muted)
            {
                Muted = false;
                Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, Volume);
            }
            else
            {
                Muted = true;
                Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, 0.0f);
            }
        }

        public double Duration
        {
            get
            {// length in bytes 
                long len = Bass.BASS_ChannelGetLength(Handle, BASSMode.BASS_POS_BYTES);
                // the time length 
                double time = Bass.BASS_ChannelBytes2Seconds(Handle, len);

                return time;
            }
        }

        private bool ended=false;
        public double Position
        {
            get
            {
                // length in bytes 
                long len = Bass.BASS_ChannelGetPosition(Handle, BASSMode.BASS_POS_BYTES);
                // the time length 
                double time = Bass.BASS_ChannelBytes2Seconds(Handle, len);
                if (Duration == time && ended==false) { OnPropertyChanged("TrackEnded"); TrackEnded(); ended = true; }
                return time;
            }
            set
            {
                Bass.BASS_ChannelSetPosition(Handle, value);
            }
        }

        /// <summary>
        /// Implemented member, disposed
        /// </summary>
        public void Dispose()
        {
            Bass.BASS_ChannelStop(Handle);
        }
        #endregion

        private void TrackEnded()
        {
            foreach (var attachedDsP in AttachedDSPs)
            {
                if (attachedDsP==null)continue;
                attachedDsP.Stop();
                attachedDsP.Dispose();
            }
        }

        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}