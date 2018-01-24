using System.Collections.Generic;
using System.Net;
using libc_def.Web.TENWS.Common;

namespace libc_def.Web.TENWS
{
	public class Image
	{
		public string Url { get; set; }
		public List<object> Tags { get; set; }
		public bool Verified { get; set; }
		public License License { get; set; }
		public class Response
		{
			public Status status { get; set; }
			public int start { get; set; }
			public int total { get; set; }
			public List<Image> images { get; set; }
		}

		public class RootObject
		{
			public Response response { get; set; }
		}
		public static Response GetbyName(string name)
		{
			using (var wc = new WebClient()
				)
			{
				string url =
					"http://developer.echonest.com/api/v4/artist/images?name=" + name;
				string data = wc.DownloadString(TENWSDev.Build(url));
				return Deserializer<RootObject>.Deserialize(data).response;
			}
		}
	}
}