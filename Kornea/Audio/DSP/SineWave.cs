using System;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
    public class SineWave        : BaseDSP
    {
        public SineWave(int channel, int priority)
            : base(channel, priority, IntPtr.Zero)
        {
          
        }

        private float ii = 0;
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
                    ii += 0.9f;
                    data[i] = (float)Math.Cos(ii) ;
                }
                if (data != null)
                {
                   
                }
            }

            RaiseNotification();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
