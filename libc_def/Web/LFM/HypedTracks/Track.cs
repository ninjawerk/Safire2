namespace libc_def.Web.LFM.HypedTracks
{
	public class Track
	{
		public string name { get; set; }
		public string duration { get; set; }
		public string percentagechange { get; set; }
		public string mbid { get; set; }
		public string url { get; set; }
		public Streamable streamable { get; set; }
		public Artist artist { get; set; }
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
		public string Path { get; set; }

		public string sub
		{
			get
			{
				return "by " + artist.name;
			}
		}
	}
}