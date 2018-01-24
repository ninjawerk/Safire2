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
	public class Reverb : BaseDSP
	{
	
		public unsafe
	float* data2;
		private float decay;

		public Reverb(int channel, int priority)
			: base(channel, priority, IntPtr.Zero)
		{
		}


		public float Decay
		{
			get { return decay; }
			set
			{
				decay = value;
			}
		}
		public int Delay { get; set; }

		public override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
		{
 
			if (IsBypassed )
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
				data2 = (float*)buffer;
				float* x = stackalloc float[length];
				float* data = stackalloc float[length];
				for (int i = 0; i < length / 4; i++)
				{
					x[i] = data2[i];
				}
				for (int a = 0; a < length / 4; a++)
				{
					data[a] += x[a];
					data[Delay + a] += x[a] * Decay;

				}
				for (int i = 0; i < length / 4; i++)
				{
					data2[i] = data [i];
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