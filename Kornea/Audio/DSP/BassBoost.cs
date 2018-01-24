using System;
using System.Collections.Generic;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
    public class BassBoost : BaseDSP
    {
        private readonly RingBuffer delay_line;
        private readonly Integrate_and_Dump iad;
        private float _bandwidth = 4;
        private float _cutOff = 100;
        private float _dVol = 1;
        private float _highCutoff;
        private float _lowCutoff1;
        private float _lowCutoff2;
        private float _pVol = 1;
        public List<Filter> post_Filters = new List<Filter>();
        public List<Filter> pre_Filters = new List<Filter>();

        public BassBoost(int channel, int priority)
            : base(channel, priority, IntPtr.Zero)
        {
            iad = new Integrate_and_Dump(ChannelSampleRate);
            delay_line = new RingBuffer((int) iad.GetLatency());
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

        public float HighCutoff
        {
            get { return _highCutoff; }
            set
            {
                _highCutoff = value;

                update_Post_Filters();
            }
        }

        public float LowCutoff1
        {
            get { return _lowCutoff1; }
            set
            {
                _lowCutoff1 = value;

                update_Post_Filters();
            }
        }

        public float LowCutoff2
        {
            get { return _lowCutoff2; }
            set
            {
                _lowCutoff2 = value;

                update_Post_Filters();
            }
        }

        //PRE FILTERS ||| BAND PASS
        private void update_Pre_Filters()
        {
            float freq = 0;
            float fq = 0;

            fq = (float) (Math.Sqrt(Math.Pow(2, Bandwidth))/(Math.Pow(2, Bandwidth) - 1));
            freq = (float) (((CutOff/Math.Pow(2, Bandwidth)) + CutOff)/2f);
            if (pre_Filters.Count == 0)
            {
                pre_Filters.Add(new Filter(eBiQuadFilter.BP, ChannelSampleRate, freq, fq));
                pre_Filters.Add(new Filter(eBiQuadFilter.BP, ChannelSampleRate, freq, fq));
            }
            foreach (Filter preFilter in pre_Filters)
            {
                preFilter.update(ChannelSampleRate, freq, fq);
            }
        }

        private unsafe void run_Pre_Filters(float* data, float length, int bit_space)
        {
            for (int i = 0; i < length/bit_space; i++)
            {
                float x = pre_Filters[1].Process(pre_Filters[0].Process(data[i]));
                data[i] = x;
            }
        }

        //POST FILTERS ||| HIGH PASS | LOW PASS | LOW PASS
        //needs fix
        private void update_Post_Filters()
        {
            float freq = 0;
            float fq = 0;

            fq = (float) (Math.Sqrt(Math.Pow(2, Bandwidth))/(Math.Pow(2, Bandwidth) - 1));
            if (post_Filters.Count == 0)
            {
                post_Filters.Add(new Filter(eBiQuadFilter.HP, ChannelSampleRate, HighCutoff, fq));
                post_Filters.Add(new Filter(eBiQuadFilter.LP, ChannelSampleRate, LowCutoff1, fq));
                post_Filters.Add(new Filter(eBiQuadFilter.LP, ChannelSampleRate, LowCutoff2, fq));
            }

            post_Filters[0].update(ChannelSampleRate, HighCutoff*2, fq);
            post_Filters[1].update(ChannelSampleRate, LowCutoff1 , fq);
            post_Filters[2].update(ChannelSampleRate, LowCutoff2/2, fq);

        }

        private unsafe void run_Post_Filters(float* data, float length, int bit_space)
        {
            for (int i = 0; i < length/bit_space; i++)
            {
                float v = post_Filters[2].Process(post_Filters[1].Process(post_Filters[0].Process(data[i])));
                data[i] = v;
            }
        }

        private unsafe void mix_with_DirectSignal(float* orig, float* processed, float length, int bit_space)
        {
            float original_compensated;
            for (int i = 0; i < length/bit_space; ++i)
            {
                original_compensated = delay_line.Run(orig[i]);
                processed[i] = (float)(d_vol * original_compensated + (p_vol * .5 * processed[i])) * 1.5f;
            }
        }

        public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (IsBypassed)
                return;

            if (ChannelBitwidth == 32) // 32-bit sample data
            {
                var data = (float*) buffer;
                float* y = stackalloc float[length];
                float* x = stackalloc float[length];
                for (int i = 0; i < length/4; i++)
                {
                    x[i] = data[i];
                    y[i] = data[i];
                }
                if (data != null)
                {
                    update_Pre_Filters();
                    update_Post_Filters();                     
            
                    run_Pre_Filters(data, length, 4);
                    iad.Run(data,x, length / 4);                  
                    run_Post_Filters(data, length, 4); 
           
                    mix_with_DirectSignal(y, data, length, 4);
                }
            }

            RaiseNotification();
        }

        public override void OnChannelChanged()
        {
            update_Post_Filters();
            update_Pre_Filters();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}