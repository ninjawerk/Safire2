using System.Net;

namespace libc_def.Web.LFM.TopArtists
{
	public class Artists
	{
		public Artist[] artist { get; set; }
		public Attr attr { get; set; }
		public class Rootobject
		{
			public Artists artists { get; set; }
		}
		public class Artist
		{
			public string name { get; set; }
			public string playcount { get; set; }
			public string listeners { get; set; }
			public string mbid { get; set; }
			public string url { get; set; }
			public string streamable { get; set; }
			public Image[] image { get; set; }

			public string IconImage 
			{
				get
				{
					if (image != null && image.Length > 3)
					{
						return image[3].text;
					}
					else
					{
						return "";
					}
				}
				
			}
			public string Available { get; set; }

			public string sub
			{
				get
				{
					return listeners + " listeners";
				}
			}
		}
		public static Artists GetTopArtists( )
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://ws.audioscrobbler.com/2.0/?method=chart.gettopartists" ;
				 string data = wc.DownloadString(LFMDev.Build(url));
				return Deserializer<Rootobject>.Deserialize(data.Replace("#","")).artists;
			}
		}

	}
}