using System;
using System.Windows.Threading;
using Kornea.Audio.Interfaces;
using Un4seen.Bass;

namespace Kornea.Audio.Reactor
{
    public sealed class PanFade : IReactor
    {
        public delegate void ActionCompleted();

        private const string POOL_PREFIX = "REACTPAN";

        private readonly DispatcherTimer FadeThread;
        private readonly bool _drop;
        private readonly float _reachcap;
        private readonly double jump = 0.1;
        private readonly AudioWave _waveIn;
        private readonly AudioWave _waveOut;
        private bool _kill;
        private float _startVol;

        /// <summary>
        ///     Use to fade the volume of a song.
        /// </summary>
    
        /// <param name="interval">Thread loop time in ticks</param>
        /// <param name="duration">Total duration for the fade</param>
        /// <param name="Drop">FadeOut</param>
        /// <param name="reach">Ceiling for fade In</param>
        public PanFade(AudioWave wIN, AudioWave wOut, int interval, float duration, bool Drop, float reach)
        {
            if (wIN != null && wOut != null)
            {
                
                _reachcap = reach;
                jump = 1 / (duration / interval);
                _drop = Drop;
                _waveIn = wIN;
                _waveOut = wOut;
                foreach (var v in _waveIn.WorkingReactors)
                {
                    if (v.Contains(POOL_PREFIX))
                    {
                        if (ReactorPool.Pool.ContainsKey(v)) ReactorPool.Pool[v].ForceStop();
                        if (ReactorPool.Pool.ContainsKey(v)) ReactorPool.Pool[v].Dispose();
                    }
                }
                foreach (var v in _waveOut.WorkingReactors)
                {
                    if (v.Contains(POOL_PREFIX))
                    {
                        if (ReactorPool.Pool.ContainsKey(v)) ReactorPool.Pool[v].ForceStop();
                        if (ReactorPool.Pool.ContainsKey(v)) ReactorPool.Pool[v].Dispose();
                    }
                }
                FadeThread = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = new TimeSpan(interval * TimeSpan.TicksPerMillisecond)
                };
                FadeThread.Tick += FadeThread_Tick;
                _startVol = 0;
            }
        }

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
                if (ReactorPool.Pool.ContainsKey(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle))
                    ReactorPool.Pool.Remove(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle);

                //remove from wave reactors
                if (_waveIn.WorkingReactors.Contains(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle))
                    _waveIn.WorkingReactors.Remove(string.Format("{0}{1}", POOL_PREFIX, _waveIn.Handle + _waveOut.Handle));
                if (_waveOut.WorkingReactors.Contains(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle))
                    _waveOut.WorkingReactors.Remove(string.Format("{0}{1}", POOL_PREFIX, _waveIn.Handle + _waveOut.Handle));

                if (_kill && _waveOut != null) _waveOut.Dispose();
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
            if (_waveIn == null && _waveOut == null) return;

            //check if already in pool
            if (ReactorPool.Pool.ContainsKey(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle))
            {
                ReactorPool.Pool[POOL_PREFIX + _waveIn.Handle + _waveOut.Handle].Dispose(); //remove from pool
            }

            //add to pool
            if (!ReactorPool.Pool.ContainsKey(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle))
                ReactorPool.Pool.Add(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle, this);

            //add to wave reactors
            if (!_waveIn.WorkingReactors.Contains(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle))
                _waveIn.WorkingReactors.Add(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle);
            if (!_waveOut.WorkingReactors.Contains(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle))
                _waveOut.WorkingReactors.Add(POOL_PREFIX + _waveIn.Handle + _waveOut.Handle);

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

        public event ActionCompleted ActionCompletedEvent;

        private void FadeThread_Tick(object sender, EventArgs e)
        {
            if (_startVol + (float)jump < _reachcap)
            {
                _startVol += (float)Math.Pow( jump, .95);
                if (!_waveIn.Muted)
                {
                    Bass.BASS_ChannelSetAttribute(_waveIn.Handle, BASSAttribute.BASS_ATTRIB_VOL, _startVol);
                    Bass.BASS_ChannelSetAttribute(_waveOut.Handle, BASSAttribute.BASS_ATTRIB_VOL, (_reachcap - _startVol));
                    Bass.BASS_ChannelSetAttribute(_waveOut.Handle, BASSAttribute.BASS_ATTRIB_PAN, (_startVol/_reachcap));
                    Bass.BASS_ChannelSetAttribute(_waveIn.Handle, BASSAttribute.BASS_ATTRIB_PAN, -1 + (_startVol / _reachcap));
                }

            }
            else
            {
                Bass.BASS_ChannelSetAttribute(_waveIn.Handle, BASSAttribute.BASS_ATTRIB_PAN, 0);

                RaiseActionCompletedEvent();
                //if ordered to kill wave, dispose it
                if (_kill)
                {
                    _waveOut.Dispose();
                }
                Dispose();
            }
        }
    }
}