using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass.Misc;

namespace Kornea.Audio.DSP
{
	/// <summary>
	/// AGC - Automatic Gain Control
	/// </summary>
	public class AGC	  :BaseDSP
	{
		private float _manualGainFactor = 1f;

		public float ManualGainFactor
		{
			get { return _manualGainFactor; }
			set { _manualGainFactor = value; }
		}

		public  override unsafe void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
		{
			if (IsBypassed || Player.Instance.NetStreamingConfigsLoaded)
				return;

			if (IsBypassed || Player.Instance.NetStreamingConfigsLoaded)
				return;

			if (ChannelBitwidth == 32) // 32-bit sample data
			{
				var data = (float*)buffer;
			 
				for (int i = 0; i < length / 4; i++)
				{
					data[i] = data[i]*ManualGainFactor;
				}
		 
			}

		}

		public override string ToString()
		{
			return "Automatic Gain Control";
		}

		public AGC(int channel, int priority)
			: base(channel, priority, IntPtr.Zero)
		{
			//Safire kornea instance locked/initialized in other modes// disable DSP
			if (Player.Instance.NetStreamingConfigsLoaded) return;
		 
		}
	}
}
