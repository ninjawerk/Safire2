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
    public class ShelfBassBoost : BaseDSP
    {
        public unsafe
            float* data;

        private float out_left;
        private float out_right;

        private float width;
		private BiQuadFilter bqf = BiQuadFilter.LowShelf(44000, 120, .5f, 2);

		public ShelfBassBoost(int channel, int priority)
            : base(channel, priority, IntPtr.Zero)
        {
        }


        public float Width
        {
            get { return width; }
	        set
	        {
		        width = value;
				bqf = BiQuadFilter.LowShelf(44000, width*10, .5f, 25);
	        }
        }


        public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
      if (IsBypassed || Player.Instance.NetStreamingConfigsLoaded) 
                return;

            if (ChannelBitwidth == 16)
            {
				// 16-bit sample data
				//CURRENTLY NOT REQUIRED
				//SAFIRE DEFAULTS DECODING IN FLOATS
            }
            else if (ChannelBitwidth == 32)
            {
                // 32-bit sample data
                data = (float*) buffer;
				for (int a = 0; a < length / 4; a++)
				{
					data[a] = bqf.Transform(data[a]);

				}
            }
            else
            {
				// ??-bit sample data
				//NOT REQUIRED
				//SAFIRE DEFAULTS DECODING IN FLOATS
            }
            RaiseNotification();
        }

        public override void OnChannelChanged()
        {
        }

         

        public override string ToString()
        {
            return "Stereo Enhancer";
        }
    }
}