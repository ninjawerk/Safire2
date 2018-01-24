using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bridge;

namespace pla_def
{
	/// <summary>
	///     Sample:
	///     #EXTM3U
	///     #EXTINF:123, Sample artist - Sample title
	///     C:\Documents and Settings\I\My Music\Sample.mp3
	///     #EXTINF:321,Example Artist - Example title
	///     C:\Documents and Settings\I\My Music\Greatest Hits\Example.ogg
	/// </summary>
    public class M3U : Bridge.Playlist
    {
		public M3U()
		{
			FileExtension = "m3u";
			FileFormat = "M3U";
			Author = "Deshan Alahakoon";
			Version = "1.0";
		}

		public M3U(string path, string id) : this()  
	    {
		 
	    }

	    public override List<Song> Read()
	    {
			var tracks = new List<Song>();

			string line;
			string prevline = "";
			// Read the file
			if (File.Exists(Path))
			{
				var file =
				new StreamReader(Path);
				//parse the content
				var tr = new Song();
				;
				while ((line = file.ReadLine()) != null)
				{
					if (line.Contains("#EXTINF:"))
					{
						tr = new Song();
						var r = new Regex(",", RegexOptions.CultureInvariant);
						string[] re = r.Split(line.Replace("#EXTINF:", ""));

						tr.Duration = Convert.ToInt32(re[0]);
						string str = "";
						for (int i = 1; i < re.Length; i++)
						{
							str += re[i];
						}
						if (str.Contains("-"))
						{
							var r2 = new Regex("-", RegexOptions.Singleline);
							string[] rs = r2.Split(str);

							tr.Title = rs.Last().TrimEnd().TrimStart();
							tr.Artist =str.Replace(" - " + tr.Title,"").TrimEnd().TrimStart();
						}
						else
						{
							tr.Title =str;
						}

					}
					else if (prevline.Contains("#EXTINF:"))
					{
						tr.Path = line;
						tracks.Add(tr);
					}
					prevline = line;
				}

				file.Close();
			}
			return tracks;
	    }

	    public override bool Write(List<Song> songs)
	    {
			if (songs.Count == 0) return false;
		 
			var sb = new StringBuilder();

			//start building content
			sb.AppendLine("#EXTM3U");

			foreach (Song track in songs)
			{
				sb.AppendLine(); //leave line above
				sb.AppendLine("#EXTINF:" + track.Duration + "," + track.Artist + " - " + track.Title);
				sb.AppendLine(track.Path);
			}
			//end of building content

			//write file
			using (var sw = new StreamWriter(Path, false))
			{
				sw.Write(sb.ToString());
				sw.Close();
			}
			return true;
	    }
    }
}
