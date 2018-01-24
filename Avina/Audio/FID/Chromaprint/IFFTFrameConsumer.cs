// -----------------------------------------------------------------------
// <copyright file="IFFTFrameConsumer.cs" company="">
// Original C++ implementation by Lukas Lalinsky, http://acoustid.org/chromaprint
// </copyright>
// -----------------------------------------------------------------------

namespace Kornea.Audio.AcousticID.Chromaprint
{
    /// <summary>
    /// Consumer of frames produced by FFT.
    /// </summary>
    interface IFFTFrameConsumer
    {
		void Consume(FFTFrame frame);
    }
}
