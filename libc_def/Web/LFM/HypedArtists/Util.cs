using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace libc_def.Web.LFM.HypedArtists
{
	class Util
	{
		public static Artists GetHypedArtists()
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://ws.audioscrobbler.com/2.0/?method=chart.gethypedartists";
				string data = wc.DownloadString(LFMDev.Build(url));
				return Deserializer<Rootobject>.Deserialize(data.Replace("#", "")).artists;
			}
		}
	}
}
