using System.Net;
using libc_def.Web.TENWS.Common;

namespace libc_def.Web.TENWS
{
	public class Songs
	{
		public string id { get; set; }
		public string title { get; set; }

		public class RootObject
		{
			public Response response { get; set; }
		}

		public class Response
		{
			public Status status { get; set; }
			public int start { get; set; }
			public int total { get; set; }
			public Songs[] songs { get; set; }
		}

 
		public static Response GetbyArtistName(string name)
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://developer.echonest.com/api/v4/playlist/static?sort=song_hotttnesss-desc&artist=" + name;
				string data = wc.DownloadString(TENWSDev.Build(url));
				return Deserializer<RootObject>.Deserialize(data).response;
			}
		}
	}
	
	
}