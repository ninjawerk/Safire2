namespace libc_def.Web.LFM.ArtistInfo
{
	public class Artist1
	{
		public string name { get; set; }
		public string url { get; set; }
		public Image[] image { get; set; }
		public string IconImage
		{
			get
			{
				if (image != null && image.Length > 2)
				{
					return image[2].text;
				}
				else
				{
					return "";
				}
			}

		}
		public string Available { get; set; }
	}
}