using System;
using Kornea.Audio.Interfaces;

namespace Kornea.Audio.Reactor
{
    public sealed class CrossFade : IReactor
    {
        public delegate void ActionCompleted();
        private const string POOL_PREFIX = "REACTCROSSFADE";
        private readonly AudioWave _waveIn;
        private readonly AudioWave _waveOut;
        private bool _kill;

        /// <summary>
        ///     Use to fade the volume of a song.
        /// </summary>
        public CrossFade(AudioWave wIN, AudioWave wOut)
        {
            if (wIN != null && wOut != null)
            {
               
                _waveIn = wIN;
                _waveOut = wOut; foreach (var v in _waveIn.WorkingReactors)
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
            }
        }

        public void RaiseActionCompletedEvent()
        {
            Dispose();

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
                    _waveOut.WorkingReactors.Remove(string.Format("{0}{1}", POOL_PREFIX,
                                                                  _waveIn.Handle + _waveOut.Handle));

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

              fadeIn = new Fader(_waveIn,FadeMode.Linear, 5,250,false,Player.Instance.Volume);
            fadeIn.Start();

              fadeOut = new Fader(_waveOut, FadeMode.Linear, 5, 250, true, Player.Instance.Volume);
            fadeOut.StartAndKill();
             fadeOut.ActionCompletedEvent += RaiseActionCompletedEvent;

        }

        private Fader fadeIn;
        private Fader fadeOut;
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
            fadeIn.ForceStop();
            fadeOut.ForceStop();
        }

        public event ActionCompleted ActionCompletedEvent;
    }
}