// -----------------------------------------------------------------------
// <copyright file="IAudioConsumer.cs" company="">
// Original C++ implementation by Lukas Lalinsky, http://acoustid.org/chromaprint
// </copyright>
// -----------------------------------------------------------------------

namespace Kornea.Audio.AcousticID.Chromaprint
{
    /// <summary>
    /// Consumer for 16bit audio data buffer.
    /// </summary>
    public interface IAudioConsumer
    {
        void Consume(short[] input, int length);
    }
}
