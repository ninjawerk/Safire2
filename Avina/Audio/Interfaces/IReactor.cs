using System;

namespace Kornea.Audio.Interfaces
{
    public interface IReactor : IDisposable
    {
         void RaiseActionCompletedEvent();

        /// <summary>
        ///     Start the reactor
        /// </summary>
        void Start();

        /// <summary>
        ///     Start reactor and dispose the wave after fading
        /// </summary>
        void StartAndKill();

        /// <summary>
        ///     Abort thread
        /// </summary>
        void ForceStop();
    }
}