//using System;
//using Un4seen.Bass;
//using Un4seen.Bass.Misc;

//namespace Lym.Audio.DSP
//{
//    /// <summary>
//    ///     This function adds a low-passed signal to the original signal. The low-pass has a quite wide response.
//    ///     Deshan Alahakoon
//    ///     2013 (C)
//    ///     Safire_DSP_Architecture 2.0
//    ///     Primary Chain DSP - Yes
//    /// </summary>
//    public class BassBoost : BaseDSP
//    {
//        private float cap;
//        public unsafe float* data;
//        private float gain1;
//        private float gain2 = 1;
//        public bool heavySignal = false;
//        private float ratio = 0.5f;
//        private float selectivity = 60;
//        public float wmax = 0;

//        public BassBoost(int channel, int priority)
//            : base(channel, priority, IntPtr.Zero)
//        {
//        }

//        public int Cycles { get; set; }

//        public float Gain
//        {
//            get { return gain2; }
//            set { gain2 = value; }
//        }

//        public float Selectivity
//        {
//            get { return selectivity; }
//            set { selectivity = value; }
//        }

//        public float Ratio
//        {
//            get { return ratio; }
//            set { ratio = value; }
//        }

//        public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
//        {
//            if (IsBypassed)
//                return;

//            if (ChannelBitwidth == 16)
//            {
//                // 16-bit sample data
//                var data = (short*) buffer;
//                for (int a = 0; a < length/2; a++)
//                {
//                    //obsolete
//                }
//            }
//            else if (ChannelBitwidth == 32)
//            {
//                // 32-bit sample data

//                data = (float*) buffer;
//                float prevVal = data[0]; //for determining a jump from pos+ to neg- or vice versa
//                int curRange = 0;
//                float max = data[0];
//                if (data != null)
//                {
//                    var flar = new float[256];
//                    Bass.BASS_ChannelGetData(ChannelHandle, flar, (int) BASSData.BASS_DATA_FFT256);
//                    float tot = 0;
//                    for (int i = 0; i < 50; i++)
//                    {
//                        tot += flar[i];
//                    }
//                    //apply boosts
//                    for (int a = 0; a < length/4; a++)
//                    {
//                        for (int m = 0; m < Cycles; m++) //cycling 
//                        {
//                            data[a] = Boost(data[a]);
//                        }
//                        if (Math.Abs(data[a]) > max) max = Math.Abs(data[a]);
//                    }
//                }
//                wmax = max;
//            }
//            else
//            {
//                // Others process in full
//                var data = (byte*) buffer;
//                for (int a = 0; a < length; a++)
//                {
//                    // (8-bit sample data)
//                }
//            }
//            RaiseNotification();
//        }

//        public override void OnChannelChanged()
//        {
//        }
             

//        private float Boost(float sample)
//        {
//            gain1 = (float) (1.0/(selectivity + 1));
//            cap = (sample + (cap*selectivity))*gain1;
//            sample = (heavySignal) ? sample*(1 - ratio) : sample + (cap)*ratio;
//            return Math.Min(Math.Max(-10, sample*gain2), 1f);
//        }

//        private float saturate(float val)
//        {
//            return Math.Min(Math.Max(-1, val), 1f);
//        }

//        public override string ToString()
//        {
//            return "Bass Booster 2.0";
//        }
//    }
//}