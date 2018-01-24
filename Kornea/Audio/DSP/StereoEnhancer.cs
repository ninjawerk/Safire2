using System;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
    /// <summary>
    ///     Stereo Widening Digital Signal Processor
    ///     Coded by Deshan Alahakoon
    ///     2013 (C)
    ///     Safire_DSP_Architecture 2.0
    ///     Primary Chain DSP - Yes
    /// </summary>
    public class StereoEnhancer : BaseDSP
    {
        public unsafe
            float* data;

        private float out_left;
        private float out_right;

        private float width;

        public StereoEnhancer(int channel, int priority)
            : base(channel, priority, IntPtr.Zero)
        {
        }


        public float Width
        {
            get { return width; }
            set { width = value; }
        }


        public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (IsBypassed)
                return;

            if (ChannelBitwidth == 16)
            {
                // 16-bit sample data
                var data = (short*) buffer;
                for (int a = 0; a < length/2; a++)
                {
                    //todo biatch
                 }
            }
            else if (ChannelBitwidth == 32)
            {
                // 32-bit sample data
                data = (float*) buffer;
                for (int a = 0; a < length/4; a += 2)
                {
                    Widen(data[a], data[a + 1]);
                    data[a] = out_left;
                    data[a + 1] = out_right;
                }
            }
            else
            {
                // Others process in full
                var data = (byte*) buffer;
                for (int a = 0; a < length; a++)
                {
                    //todo biatch
                 }
            }
            RaiseNotification();
        }

        public override void OnChannelChanged()
        {
        }

        private void Widen(float in_left, float in_right)
        {
            // calculate scale coefficient
            float coef_S = width*0.5f;

            float m = (in_left + in_right)*0.5f;
            float s = (in_right - in_left)*coef_S;

            out_left = m - s;
            out_right = m + s;
        }

        private float Saturate(float val)
        {
            return Math.Min(Math.Max(-1, val), 1f);
        }

        public override string ToString()
        {
            return "Stereo Enhancer";
        }
    }
}