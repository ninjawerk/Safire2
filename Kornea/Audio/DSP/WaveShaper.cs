using System;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
    /// <summary>
    ///     a simple fold-back distortion filter.
    ///     if the signal exceeds the given threshold-level, it mirrors at the positive/negative threshold-border as long as the singal lies in the legal range (-threshold..+threshold).
    ///     there is no range limit, so inputs doesn't need to be in -1..+1 scale.
    ///     threshold should be >0
    ///     depending on use (low thresholds) it makes sense to rescale the input to full amplitude
    /// </summary>
    public class WaveShaper : BaseDSP
    {
        private float _threshold = 0.5f;

        public WaveShaper(int channel, int priority)
            : base(channel, priority, IntPtr.Zero)
        {
        }

        // example implementation of the DSPCallback method 
        public float Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (IsBypassed)
                return;

            if (ChannelBitwidth == 16)
            {
                // process the data 
                var data = (short*)buffer;
                for (int a = 0; a < length / 2; a++)
                {
                    // your work goes here (16-bit sample data)
                }
            }
            else if (ChannelBitwidth == 32)
            {
                // process the data 
                var data = (float*)buffer;
                for (int a = 0; a < length / 4; a++)
                {
                    data[a] = waveshape_distort(data[a]);
                }
            }
            else
            {
                // process the data 
                var data = (byte*)buffer;
                for (int a = 0; a < length; a++)
                {
                    // your work goes here (8-bit sample data)
                }
            }
            // if you have calculated UI relevant data you might raise the event 
            // else comment out the following line
            RaiseNotification();
        }

        public override void OnChannelChanged()
        {
            // override this method if you need to react on channel changes 
            // e.g. usefull, if an internal buffer needs to be reset etc.
        }

        float waveshape_distort(float ins)
        {
            return 1.5f * ins - 0.5f * ins * ins * ins;
        }


        public override string ToString()
        {
            return "My DSP";
        }
    }
}