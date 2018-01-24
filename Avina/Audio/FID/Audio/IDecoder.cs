// -----------------------------------------------------------------------
// <copyright file="IDecoder.cs" company="">
// Original C++ implementation by Lukas Lalinsky, http://acoustid.org/chromaprint
// </copyright>
// -----------------------------------------------------------------------

using Kornea.Audio.AcousticID.Chromaprint;

namespace Kornea.Audio.AcousticID.Audio
{
	/// <summary>
    /// Interface for audio decoders.
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        /// Gets the sample rate of the audio sent to the fingerprinter. 
        /// </summary>
        /// <remarks>
        /// May be different from the source audio sample rate, if the decoder does resampling.
        /// </remarks>
        int SampleRate { get; }

        /// <summary>
        /// Gets the channel count of the audio sent to the fingerprinter. 
        /// </summary>
        /// <remarks>
        /// May be different from the source audio channel count.
        /// </remarks>
        int Channels { get; }

        /// <summary>
        /// Load an audio file.
        /// </summary>
        void Load(string file);

        /// <summary>
        /// Decode audio file.
        /// </summary>
        bool Decode(IAudioConsumer consumer, int maxLength);
    }
}
