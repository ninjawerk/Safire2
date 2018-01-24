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
	public class Volume : BaseDSP
	{
		public unsafe
			float* data;


		private float vol;
 
		public Volume(int channel, int priority)
			: base(channel, priority, IntPtr.Zero)
		{
		}

				   /// <summary>
				   /// 0 to 100 
				   /// </summary>
		public float Vol
		{
			get { return vol; }
			set
			{
				vol = value;

			}
		}


		public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
		{
			if (IsBypassed)
				return;
 
			else if (ChannelBitwidth == 32)
			{
				// 32-bit sample data
				data = (float*)buffer;
				for (int a = 0; a < length / 4; a ++)
				{
					data[a] = data[a] * (vol );
			 
				}
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