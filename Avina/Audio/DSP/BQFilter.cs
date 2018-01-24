using System;

namespace Kornea.Audio.DSP
{
    public enum eBiQuadFilter
    {
        HP,
        LP,
        BP
    }

    public class Filter

    {
        private readonly eBiQuadFilter typ;
        private float Cfreq;
        private BiQuadFilter bqf;

        public Filter(eBiQuadFilter e, float rate, float freq, float q)
        {
            typ = e;
            Cfreq = freq;
            switch (e)
            {
                case eBiQuadFilter.HP:
                    bqf = BiQuadFilter.HighPassFilter(rate, freq, q);
                    break;
                case eBiQuadFilter.LP:
                    bqf = BiQuadFilter.LowPassFilter(rate, freq, q);
                    break;
                case eBiQuadFilter.BP:
                    bqf = BiQuadFilter.BandPassFilterConstantSkirtGain(rate, freq, q);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("e");
            }
        }

        public void update(float rate, float f, float q)
        {
            if (f > 0 && Cfreq != f)
            {
                switch (typ)
                {
                    case eBiQuadFilter.HP:
                        bqf = BiQuadFilter.HighPassFilter(rate, f, q);
                        break;
                    case eBiQuadFilter.LP:
                        bqf = BiQuadFilter.LowPassFilter(rate, f, q);
                        break;
                    case eBiQuadFilter.BP:
                        bqf = BiQuadFilter.BandPassFilterConstantSkirtGain(rate, f, q);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("e");
                }
                Cfreq = f;
            }
        }

        public float Process(float inp)
        {
            return bqf.Transform(inp);
        }
    }
}