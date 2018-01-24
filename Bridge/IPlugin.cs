namespace Bridge
{
	public abstract  class IPlugin
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public string Company { get; set; }
		public string Version { get; set; }
		public string ReleaseDate { get; set; }
	}
}
