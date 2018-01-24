using System;
using System.Windows.Threading;
using Kornea.Audio.Interfaces;
using Un4seen.Bass;

namespace Kornea.Audio.Reactor
{
    public enum FadeMode
    {
        Linear,
        Square,
        Cubic,
        PanOrbit
    }

    public sealed class Fader : IReactor
    {
        private const string POOL_PREFIX = "REACTFADER";
        public delegate void ActionCompleted();
        private readonly DispatcherTimer FadeThread;
        private readonly bool _drop;
        private readonly double jump = 0.1;
        private readonly FadeMode _myMode;
        private readonly AudioWave _mywave;
        private readonly float _reachcap;
        private bool _kill;
        private float _startVol;

        /// <summary>
        ///     Use to fade the volume of a song.
        /// </summary>
        /// <param name="ad">Audio Wave to utilize</param>
        /// <param name="mode">Mode of fade</param>
        /// <param name="interval">Thread loop time in ticks</param>
        /// <param name="duration">Total duration for the fade</param>
        /// <param name="Drop">FadeOut</param>
        /// <param name="reach">Ceiling for fade In</param>
        public Fader(AudioWave ad, FadeMode mode, int interval, float duration, bool Drop, float reach)
        {
            if (ad != null)
            {
             
                
                _reachcap = reach;
                jump = 1 / (duration / interval);
                _drop = Drop;
                _mywave = ad;
                _myMode = mode;
                FadeThread = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = new TimeSpan(interval * TimeSpan.TicksPerMillisecond)
                };
                FadeThread.Tick += FadeThread_Tick;
                _startVol = _drop ? ad.Volume : 0;
                try
                {
                    foreach (var v in _mywave.WorkingReactors)
                    {
                        if (v.Contains(POOL_PREFIX))
                        {
                            if (ReactorPool.Pool.ContainsKey(v)) ReactorPool.Pool[v].ForceStop();
                            if (ReactorPool.Pool.ContainsKey(v)) ReactorPool.Pool[v].Dispose();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public event ActionCompleted ActionCompletedEvent;

        public void RaiseActionCompletedEvent()
        {
            if (ActionCompletedEvent != null)
            {
                ActionCompletedEvent();
            }
        }

        /// <summary>
        ///     Dispose object and force stop Fader
        /// </summary>
        public void Dispose()
        {
            ForceStop();
            try
            {
                //remove from the pool
                if (ReactorPool.Pool.ContainsKey(POOL_PREFIX + _mywave.Handle))
                    ReactorPool.Pool.Remove(POOL_PREFIX + _mywave.Handle);

                //remove from wave reactors
                if (_mywave.WorkingReactors.Contains(POOL_PREFIX + _mywave.Handle))
                    _mywave.WorkingReactors.Remove(string.Format("{0}{1}", POOL_PREFIX, _mywave.Handle));

                if (_kill && _mywave != null) _mywave.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Start the fading
        /// </summary>
        public void Start()
        {
            //check if already in pool
            if (ReactorPool.Pool.ContainsKey(POOL_PREFIX + _mywave.Handle))
            {
                ReactorPool.Pool[POOL_PREFIX + _mywave.Handle].Dispose(); //remove from pool
            }
            //add to pool
            if (!ReactorPool.Pool.ContainsKey(POOL_PREFIX + _mywave.Handle))
                ReactorPool.Pool.Add(POOL_PREFIX + _mywave.Handle, this);
            //add to wave reactors
            if (!_mywave.WorkingReactors.Contains(POOL_PREFIX + _mywave.Handle))
                _mywave.WorkingReactors.Add(POOL_PREFIX + _mywave.Handle);

            FadeThread.Start();
            FadeThread.IsEnabled = true;
        }

        /// <summary>
        ///     Start fading and dispose the wave after fading
        /// </summary>
        public void StartAndKill()
        {
            _kill = true;
            Start();
        }


        /// <summary>
        ///     Abort thread
        /// </summary>
        public void ForceStop()
        {
            FadeThread.Stop();
            FadeThread.IsEnabled = false;
        }

        private void FadeThread_Tick(object sender, EventArgs e)
        {
            switch (_myMode)
            {
                case FadeMode.Linear:
                    if (_drop)
                    {
                        if (_startVol - (float)jump > 0)
                        {
                            _startVol -= (float)jump;
                            if (!_mywave.Muted)
                                Bass.BASS_ChannelSetAttribute(_mywave.Handle, BASSAttribute.BASS_ATTRIB_VOL, _startVol);
                        }
                        else
                        {
                            RaiseActionCompletedEvent();
                            //if ordered to kill wave, dispose it
                            if (_kill)
                            {
                                _mywave.Dispose();
                            }
                            Dispose();
                        }
                    }
                    else
                    {
                        if (_startVol + (float)jump < _reachcap)
                        {
                            _startVol += (float)jump;
                            if (!_mywave.Muted)
                                Bass.BASS_ChannelSetAttribute(_mywave.Handle, BASSAttribute.BASS_ATTRIB_VOL, _startVol);
                        }
                        else
                        {
                            RaiseActionCompletedEvent();
                            //if ordered to kill wave, dispose it
                            if (_kill)
                            {
                                _mywave.Dispose();
                            }
                            Dispose();
                        }
                    }

                    break;
                case FadeMode.Square:
                    break;
                case FadeMode.Cubic:
                    break;


                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}