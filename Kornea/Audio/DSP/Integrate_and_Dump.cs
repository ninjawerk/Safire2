using System;
using Un4seen.Bass;

namespace Kornea.Audio.DSP
{
    public class Integrate_and_Dump
    {
        private float out_val;
        private float last_in;
        private int reset_timer;
        private bool freq_below_cutoff;
        private int samplerate;
        private int latency;
        static   int cutoff_freq = 16;
        private RingBuffer buffer;

        public Integrate_and_Dump(int _samplerate)
        {
            out_val = 0;
            last_in = 0;
            reset_timer = 0;
            freq_below_cutoff = false;
            samplerate = _samplerate;
            latency = Bass.BASS_GetInfo().latency ;
            buffer = new RingBuffer((int) GetLatency());
        } 

        public float GetLatency()
        {
            return latency;
        }

        void handle_zero_crossings(float inp)
        {
            if (last_in > 0 && inp < 0)
            {
                freq_below_cutoff = false;

                float freq = (float) (  reset_timer/(samplerate/2.0));
                buffer.apply_gain((float) (50.0f* Math.Pow(freq,2)), reset_timer/10);

                out_val = 0;
                reset_timer = 0;
            }
            else
            {                                     
                if (reset_timer == latency)
                {
                    freq_below_cutoff = true;
                    buffer.Reset();
                }

                if (freq_below_cutoff)
                {
                    reset_timer = 0;
                }
                else
                {
                    reset_timer++;
                }
            }
            last_in = inp;
        }

        public unsafe void Run(float* in_port, float* out_port, int nframes)
        {
            for (int i = 0; i < nframes; ++i)

            {
                handle_zero_crossings(in_port[i]);
                out_val += Math.Abs(in_port[i]);
                out_port[i]=buffer.Run(freq_below_cutoff ? 0.0f : out_val  );
            }
        }
    }
}