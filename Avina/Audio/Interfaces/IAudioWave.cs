namespace Kornea.Audio.Interfaces
{
    public interface IAudioWave
    {
        /// <summary>
        ///     Access the the stream handle
        /// </summary>
        int Handle { get; }

        /// <summary>
        ///     Set the stream's volume
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        ///     Contains the current state of the audio stream
        /// </summary>
        StreamStatus StreamStatus { get; set; }

        string Path { get; set; }
        OutputMode Output { get; set; }

        /// <summary>
        ///     Play stream
        /// </summary>
        void Play();

        void Pause();
        void Stop();
        void Open();
    }
}
