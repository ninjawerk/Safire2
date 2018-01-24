namespace libc_def.Web
{
	public class TENWSDev
	{
		public static readonly string Key = "OYGEI3HNMZNB2YIHG";
		public static readonly string ConsumerKey = "7323dc4bd38d0554c3e9c1be1c9cd215";
		public static   string Format = "json";

		public static string AttachKey(string url)
		{
			url += "&api_key=" + Key;
			return url;
		}
		public static string AttachFormat(string url)
		{
			url += "&format=" + Format;
			return url;
		}
		public static string AttachRange(string url,int start, int end)
		{
			url += "&results="+ end +"&start=" + start;
			return url;
		}
		public static string Build(string url)
		{
			return AttachFormat(AttachKey(url));
		}

	}
	public class LFMDev
	{
		public static readonly string Key = "1d3afe8559c38d819987e7bf617222f5";
		public static readonly string Secret = "cc6d3e0fe66297196b75682690226cd4";
		public static string Format = "json";

		public static string AttachKey(string url)
		{
			url += "&api_key=" + Key  ;
			return url;
		}
		public static string AttachFormat(string url)
		{
			url += "&format=" + Format + "&limit=20";
			return url;
		}
		public static string AttachRange(string url, int start, int end)
		{
			url += "&results=" + end + "&start=" + start;
			return url;
		}
		public static string Build(string url)
		{
			return AttachFormat(AttachKey(url));
		}

	}
}
