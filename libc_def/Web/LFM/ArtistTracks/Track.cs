namespace libc_def.Web.LFM.ArtistTracks
{
	public class Track
	{
		public string name { get; set; }
		public string duration { get; set; }
		public string playcount { get; set; }
		public string listeners { get; set; }
		public string mbid { get; set; }
		public string url { get; set; }
		public Streamable streamable { get; set; }
		public Artist artist { get; set; }
		public Image[] image { get; set; }
		public Attr1 attr { get; set; }

		public string TrackName
		{
			get { return " " + name; }
		}

		public string Available { get; set; }
		public string Path { get; set; }

	}
}