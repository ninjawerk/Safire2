using System;
using System.Collections.Generic;
using System.Linq;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
    public class BEQA : BaseDSP
    {
 
  
         
        public BEQA(int channel, int priority)
            : base(channel, priority, IntPtr.Zero)  
        {
            
            k32Imports.init_3band_state(ref Eq, 80, 5000, 44000);

        }
        public EQSTATE Eq = new EQSTATE();
        public unsafe override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (IsBypassed) return;
            float* data = (float*)buffer;
     
            for (int i = 0; i < length / 4; i++)
            {
        
                data[i] = (float)k32Imports.do_3band(ref  Eq, data[i]);
            }
        }
                  
      

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
