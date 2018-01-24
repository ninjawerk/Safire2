using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kornea.Audio
{
	public class WavePool
	{
		public static IDictionary<string, AudioWave> Pool = new Dictionary<string, AudioWave>();
	}
}
