using System;
using System.Collections.Generic;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
    public class TrebleBooster : BaseDSP
    {
        private float _pVol = 1;
        private readonly RingBuffer delay_line;

        private float _bandwidth = 4;
        private float _cutOff = 100;
        private float _dVol = 1;
        public TrebleBooster(int channel, int priority)
            : base(channel, priority, IntPtr.Zero)
        {
            delay_line = new RingBuffer((int)(ChannelSampleRate / 2 / CutOff));
        }
        public float CutOff
        {
            get { return _cutOff; }
            set
            {
                _cutOff = value;

                update_Pre_Filters();
            }
        }

        public float Bandwidth
        {
            get { return _bandwidth; }
            set
            {
                _bandwidth = value;

                update_Pre_Filters();
            }
        }
        public List<Filter> pre_Filters = new List<Filter>();
        //PRE FILTERS ||| BAND PASS
        private void update_Pre_Filters()
        {
            float freq = 0;
            float fq = 0;

            fq = (float)(Math.Sqrt(Math.Pow(2, Bandwidth)) / (Math.Pow(2, Bandwidth) - 1));
            freq = (float)(((CutOff / Math.Pow(2, Bandwidth)) + CutOff) / 2f);
            if (pre_Filters.Count == 0)
            {
                pre_Filters.Add(new Filter(eBiQuadFilter.LP, ChannelSampleRate, freq, fq));
                pre_Filters.Add(new Filter(eBiQuadFilter.LP, ChannelSampleRate, freq, fq));
            }
            foreach (Filter preFilter in pre_Filters)
            {
                preFilter.update(ChannelSampleRate, freq, fq);
            }
        }

        private unsafe void run_Pre_Filters(float* data, float length, int bit_space)
        {
            for (int i = 0; i < length / bit_space; i++)
            {
                float x = pre_Filters[1].Process(pre_Filters[0].Process(data[i]));
                data[i] = x;
            }
        }
        public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (IsBypassed)
                return;

            if (ChannelBitwidth == 32) // 32-bit sample data
            {
                var data = (float*)buffer;

                float* x = stackalloc float[length];
                for (int i = 0; i < length / 4; i++)
                {
                    x[i] = data[i];
                }
                if (data != null)
                {
                    update_Pre_Filters();


                    run_Pre_Filters(data, length, 4);
                    iad.Run(data, data, length / 4);
                    run_Post_Filters(data, length, 4);

                    mix_with_DirectSignal(x, data, length, 4);
                }
            }

            RaiseNotification();
        }
        public float d_vol
        {
            get { return _dVol; }
            set { _dVol = value; }
        }

        public float p_vol
        {
            get { return _pVol; }
            set { _pVol = value; }
        }
        private unsafe void mix_with_DirectSignal(float* orig, float* processed, float length, int bit_space)
        {
            float original_compensated;
            for (int i = 0; i < length / bit_space; ++i)
            {
                original_compensated = delay_line.Run(orig[i]);
                processed[i] = (float)(d_vol * original_compensated + p_vol    * processed[i]);
            }
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
