using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;

namespace Bridge
{
	[InheritedExport(typeof(Playlist))]
	public abstract class Playlist : IPlugin
	{
		public List<Song> Songs = new List<Song>();
		private string _name;

		public Playlist(string path, string id)
		{
			Path = path;
			ID = id;
		}

		public Playlist(string path)
		{
			Path = path;
		}

		protected Playlist()
		{

		}

		public string Name
		{
			get { return (string.IsNullOrEmpty(_name)) ? GetFileName() : _name; }
			set { _name = value; }
		}

		public string Path { get; set; }
		public string ID { get; set; }

		//Descriptors
		public string FileExtension { get; set; }
		public string FileFormat { get; set; }

		public string GetFileName()
		{
			var fin = new FileInfo(Path);
			if (fin.Name.Contains("#G#"))
			{
				Regex r = new Regex("#G#");
				return r.Split(fin.Name)[0].Replace(  fin.Extension, "");
			}
			else
			{
				return (fin.Name).Replace(  fin.Extension, "");
			}
		}

		public long SongCount()
		{
			return Songs.Count;
		}

		public abstract List<Song> Read();
		public abstract bool Write(List<Song> songs);



	}
}