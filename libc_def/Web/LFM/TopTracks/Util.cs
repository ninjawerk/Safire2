using System.Net;

namespace libc_def.Web.LFM.TopTracks
{
	class Util
	{
		public static Tracks GetTopTracks()
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://ws.audioscrobbler.com/2.0/?method=chart.gettoptracks";
				string data = wc.DownloadString(LFMDev.Build(url));
				return Deserializer<Rootobject>.Deserialize(data.Replace("#", "")).tracks;
			}
		}
	}
}
