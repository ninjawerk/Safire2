using System;
using System.Collections;
using System.Collections.Generic;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
	public class EQ : BaseDSP
	{
		private float[] OctavesSpacing;
		public EQ(int channel, int priority, float[] octavesSpacing)
			: base(channel, priority, IntPtr.Zero)
		{
			foreach (var f1 in octavesSpacing)
			{
				 bql.Add(f1,BiQuadFilter.PeakingEQ(44000,f1,4,1));
			}
			OctavesSpacing = octavesSpacing;
		}
		Dictionary<float, BiQuadFilter> bql = new Dictionary<float, BiQuadFilter>();
		public EQSTATE Eq = new EQSTATE();
		public unsafe override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
		{
			if (IsBypassed) return;
			float* data = (float*)buffer;

			for (int i = 0; i < length / 4; i++)
			{
				try
				{
					foreach (var biQuadFilter in bql)
					{
						data[i]=biQuadFilter.Value.Transform(data[i]);
					}
				}
				catch (Exception exception)
				{
					//Console.WriteLine(exception);
				}
			}
		}

		public void Update(int band, float gain)
		{
			bql[OctavesSpacing[band]] = BiQuadFilter.PeakingEQ(44000, 
				OctavesSpacing[band],.2f, gain);
		}


		public override string ToString()
		{
			throw new NotImplementedException();
		}
	}
}
