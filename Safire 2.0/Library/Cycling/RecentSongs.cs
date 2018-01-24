using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kornea.Audio;
using Safire.Core;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;
using Un4seen.Bass;

namespace Safire.Library.Cycling
{
	public class RecentSongs
	{
		public Queue<Track> tk = new Queue<Track>();
			public		   delegate void _UpdateRecentSongs();
			public event _UpdateRecentSongs UpdateRecentSongs;
		public RecentSongs()
		{
			var tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
	@"\Safire\recent.bin";
			if (File.Exists(tString))
			{
				try
				{
					tk = SerialData.BinaryDeSerializeObject<Queue<Track>>(tString);
					if(tk==null)tk=new Queue<Track>();
				}
				catch
				{
				}
			}

			Player.Instance.PropertyChanged += Instance_PropertyChanged;
		}

		void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveStreamHandle")
			{
				if (CoreMain.CurrentTrack != null && !tk.Any(c =>  c.Path == CoreMain.CurrentTrack.Path))
				{

					tk.Enqueue(CoreMain.CurrentTrack.ToTrack());
					if (tk.Count > 4) tk.Dequeue();
					
				}
				if (UpdateRecentSongs != null)
				{
					UpdateRecentSongs();
				}
			}
		}



	}
}
