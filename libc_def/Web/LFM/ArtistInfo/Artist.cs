namespace libc_def.Web.LFM.ArtistInfo
{
	public class Artist
	{
		public string name { get; set; }
		public string mbid { get; set; }
		public string url { get; set; }
		public Image[] image { get; set; }
		public string streamable { get; set; }
		public string ontour { get; set; }
		public Stats stats { get; set; }
		public Similar similar { get; set; }
		public Tags tags { get; set; }
		public Bio bio { get; set; }
	}
}