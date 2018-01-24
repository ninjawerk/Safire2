using System.Collections.Generic;
using System.Net;
using libc_def.Web.TENWS.Common;

namespace libc_def.Web.TENWS
{
	public class Artist
	{
		public string name { get; set; }
		public string id { get; set; }

		public static Response GetSimilarbyName(string name)
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://developer.echonest.com/api/v4/artist/similar?name=" + name;
				string data = wc.DownloadString(TENWSDev.Build(url));
				return Deserializer<RootObject>.Deserialize(data).response;
			}
		}
		public static Response GetSimilarbyID(string name)
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://developer.echonest.com/api/v4/artist/similar?id=" + name;
				string data = wc.DownloadString(TENWSDev.Build(url));
				return Deserializer<RootObject>.Deserialize(data).response;
			}
		}
		public class Response
		{
			public Status status { get; set; }
			public List<Artist> artists { get; set; }
		}
		public class RootObject
		{
			public Response response { get; set; }
		}
	}
}