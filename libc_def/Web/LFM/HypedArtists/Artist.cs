namespace libc_def.Web.LFM.HypedArtists
{
	public class Artist
	{
		public string name { get; set; }
		public string percentagechange { get; set; }
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
				return "hype " + percentagechange  + "%";
			}
		}
	}
}