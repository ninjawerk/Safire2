using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace libc_def.Web.LFM.HypedTracks
{
	class Util
	{
		public static Tracks GetHypedTracks()
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://ws.audioscrobbler.com/2.0/?method=chart.gethypedtracks";
				string data = wc.DownloadString(LFMDev.Build(url));
				return Deserializer<Rootobject>.Deserialize(data.Replace("#", "")).tracks;
			}
		}
	}
}
