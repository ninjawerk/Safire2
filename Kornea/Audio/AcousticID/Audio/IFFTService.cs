// -----------------------------------------------------------------------
// <copyright file="IFFTService.cs" company="">
// Original C++ implementation by Lukas Lalinsky, http://acoustid.org/chromaprint
// </copyright>
// -----------------------------------------------------------------------

namespace Kornea.Audio.AcousticID.Audio
{
	/// <summary>
    /// Interface for services computing the FFT.
    /// </summary>
    public interface IFFTService
    {
        void Initialize(int frame_size, double[] window);
        void ComputeFrame(short[] input, double[] output);
    }
}
