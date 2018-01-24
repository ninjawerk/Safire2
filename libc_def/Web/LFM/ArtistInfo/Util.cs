using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace libc_def.Web.LFM.ArtistInfo
{
	class Util
	{
		public static Artist GetData(string art)
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + HttpUtility.UrlEncode(art);
				string data = wc.DownloadString(LFMDev.Build(url));
				return Deserializer<Rootobject>.Deserialize(data.Replace("#", "")).artist;
			}
		}
	}
}
