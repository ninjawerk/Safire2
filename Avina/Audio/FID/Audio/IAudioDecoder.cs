// -----------------------------------------------------------------------
// <copyright file="IAudioDecoder.cs" company="">
// Christian Woltering, https://github.com/wo80
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Kornea.Audio.AcousticID.Audio
{
	/// <summary>
    /// Interface for audio decoders.
    /// </summary>
    public interface IAudioDecoder : IDecoder, IDisposable
    {
        int SourceSampleRate { get; }
        int SourceBitDepth { get; }
        int SourceChannels { get; }

        int Duration { get; }
        bool Ready { get; }
    }
}
