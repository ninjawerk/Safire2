using System;

namespace Kornea.Audio.DSP
{
    /// <summary>
    ///     BiQuad filter
    ///     Deshan Alahakoon
    ///     2013 (C)
    ///     Safire_DSP_Architecture 2.0
    ///     Primary Chain DSP - No
    ///     logic help from the audio cook book
    /// </summary>
    public class BiQuadFilter
    {
        // coefficients
        private readonly double a0;
        private readonly double a1;
        private readonly double a2;
        private readonly double a3;
        private readonly double a4;

        // state
        private float x1;
        private float x2;
        private float y1;
        private float y2;

        private BiQuadFilter(double a0, double a1, double a2, double b0, double b1, double b2)
        {
            // precompute the coefficients
            this.a0 = b0/a0;
            this.a1 = b1/a0;
            this.a2 = b2/a0;
            a3 = a1/a0;
            a4 = a2/a0;

            // zero initial samples
            x1 = x2 = 0;
            y1 = y2 = 0;
        }

        /// <summary>
        ///     Passes a single sample through the filter
        /// </summary>
        /// <param name="inSample">Input sample</param>
        /// <returns>Output sample</returns>
        public float Transform(float inSample)
        {
            // compute result
            double result = a0*inSample + a1*x1 + a2*x2 - a3*y1 - a4*y2;

            // shift x1 to x2, sample to x1 
            x2 = x1;
            x1 = inSample;

            // shift y1 to y2, result to y1 
            y2 = y1;
            y1 = (float) result;

            return y1;
        }

        /// <summary>
        ///     H(s) = 1 / (s^2 + s/Q + 1)
        /// </summary>
        public static BiQuadFilter LowPassFilter(float sampleRate, float cutoffFrequency, float q)
        {
            double w0 = 2*Math.PI*cutoffFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double alpha = Math.Sin(w0)/(2*q);

            double b0 = (1 - cosw0)/2;
            double b1 = 1 - cosw0;
            double b2 = (1 - cosw0)/2;
            double a0 = 1 + alpha;
            double a1 = -2*cosw0;
            double a2 = 1 - alpha;

            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = s^2 / (s^2 + s/Q + 1)
        /// </summary>
        public static BiQuadFilter HighPassFilter(float sampleRate, float cutoffFrequency, float q)
        {
            double w0 = 2*Math.PI*cutoffFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double alpha = Math.Sin(w0)/(2*q);

            double b0 = (1 + cosw0)/2;
            double b1 = -(1 + cosw0);
            double b2 = (1 + cosw0)/2;
            double a0 = 1 + alpha;
            double a1 = -2*cosw0;
            double a2 = 1 - alpha;
            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = s / (s^2 + s/Q + 1)  (constant skirt gain, peak gain = Q)
        /// </summary>
        public static BiQuadFilter BandPassFilterConstantSkirtGain(float sampleRate, float centreFrequency, float q,
            double gain = 1.0)
        {
            double w0 = 2*Math.PI*centreFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double sinw0 = Math.Sin(w0);
            double alpha = sinw0/(2*q);

            double b0 = sinw0/2; // =   Q*alpha
            int b1 = 0;
            double b2 = -sinw0/2; // =  -Q*alpha
            double a0 = 1 + alpha;
            double a1 = -2*cosw0;
            double a2 = 1 - alpha;

            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = (s/Q) / (s^2 + s/Q + 1)      (constant 0 dB peak gain)
        /// </summary>
        public static BiQuadFilter BandPassFilterConstantPeakGain(float sampleRate, float centreFrequency, float q)
        {
            double w0 = 2*Math.PI*centreFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double sinw0 = Math.Sin(w0);
            double alpha = sinw0/(2*q);

            double b0 = alpha;
            int b1 = 0;
            double b2 = -alpha;
            double a0 = 1 + alpha;
            double a1 = -2*cosw0;
            double a2 = 1 - alpha;
            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = (s^2 + 1) / (s^2 + s/Q + 1)
        /// </summary>
        public static BiQuadFilter NotchFilter(float sampleRate, float centreFrequency, float q)
        {
            double w0 = 2*Math.PI*centreFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double sinw0 = Math.Sin(w0);
            double alpha = sinw0/(2*q);

            int b0 = 1;
            double b1 = -2*cosw0;
            int b2 = 1;
            double a0 = 1 + alpha;
            double a1 = -2*cosw0;
            double a2 = 1 - alpha;
            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = (s^2 - s/Q + 1) / (s^2 + s/Q + 1)
        /// </summary>
        public static BiQuadFilter AllPassFilter(float sampleRate, float centreFrequency, float q)
        {
            double w0 = 2*Math.PI*centreFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double sinw0 = Math.Sin(w0);
            double alpha = sinw0/(2*q);

            double b0 = 1 - alpha;
            double b1 = -2*cosw0;
            double b2 = 1 + alpha;
            double a0 = 1 + alpha;
            double a1 = -2*cosw0;
            double a2 = 1 - alpha;
            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = (s^2 + s*(A/Q) + 1) / (s^2 + s/(A*Q) + 1)
        /// </summary>
        public static BiQuadFilter PeakingEQ(float sampleRate, float centreFrequency, float q, float dbGain)
        {
            double w0 = 2*Math.PI*centreFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double sinw0 = Math.Sin(w0);
            double alpha = sinw0/(2*q);
            double a = Math.Sqrt(Math.Pow(10, dbGain/40)); // TODO: should we square root this value?

            double b0 = 1 + alpha*a;
            double b1 = -2*cosw0;
            double b2 = 1 - alpha*a;
            double a0 = 1 + alpha/a;
            double a1 = -2*cosw0;
            double a2 = 1 - alpha/a;
            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = A * (s^2 + (sqrt(A)/Q)*s + A)/(A*s^2 + (sqrt(A)/Q)*s + 1)
        /// </summary>
        /// <param name="sampleRate"></param>
        /// <param name="cutoffFrequency"></param>
        /// <param name="shelfSlope">
        ///     a "shelf slope" parameter (for shelving EQ only).
        ///     When S = 1, the shelf slope is as steep as it can be and remain monotonically
        ///     increasing or decreasing gain with frequency.  The shelf slope, in dB/octave,
        ///     remains proportional to S for all other values for a fixed f0/Fs and dBgain.
        /// </param>
        /// <param name="dbGain">Gain in decibels</param>
        public static BiQuadFilter LowShelf(float sampleRate, float cutoffFrequency, float shelfSlope, float dbGain)
        {
            double w0 = 2*Math.PI*cutoffFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double sinw0 = Math.Sin(w0);
            double a = Math.Pow(10, dbGain/40); // TODO: should we square root this value?
            double alpha = sinw0/2*Math.Sqrt((a + 1/a)*(1/shelfSlope - 1) + 2);
            double temp = 2*Math.Sqrt(a)*alpha;

            double b0 = a*((a + 1) - (a - 1)*cosw0 + temp);
            double b1 = 2*a*((a - 1) - (a + 1)*cosw0);
            double b2 = a*((a + 1) - (a - 1)*cosw0 - temp);
            double a0 = (a + 1) + (a - 1)*cosw0 + temp;
            double a1 = -2*((a - 1) + (a + 1)*cosw0);
            double a2 = (a + 1) + (a - 1)*cosw0 - temp;
            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     H(s) = A * (A*s^2 + (sqrt(A)/Q)*s + 1)/(s^2 + (sqrt(A)/Q)*s + A)
        /// </summary>
        /// <param name="sampleRate"></param>
        /// <param name="cutoffFrequency"></param>
        /// <param name="shelfSlope"></param>
        /// <param name="dbGain"></param>
        /// <returns></returns>
        public static BiQuadFilter HighShelf(float sampleRate, float cutoffFrequency, float shelfSlope, float dbGain)
        {
            double w0 = 2*Math.PI*cutoffFrequency/sampleRate;
            double cosw0 = Math.Cos(w0);
            double sinw0 = Math.Sin(w0);
            double a = Math.Pow(10, dbGain/40); // TODO: should we square root this value?
            double alpha = sinw0/2*Math.Sqrt((a + 1/a)*(1/shelfSlope - 1) + 2);
            double temp = 2*Math.Sqrt(a)*alpha;

            double b0 = a*((a + 1) + (a - 1)*cosw0 + temp);
            double b1 = -2*a*((a - 1) + (a + 1)*cosw0);
            double b2 = a*((a + 1) + (a - 1)*cosw0 - temp);
            double a0 = (a + 1) - (a - 1)*cosw0 + temp;
            double a1 = 2*((a - 1) - (a + 1)*cosw0);
            double a2 = (a + 1) - (a - 1)*cosw0 - temp;
            return new BiQuadFilter(a0, a1, a2, b0, b1, b2);
        }
    }
}