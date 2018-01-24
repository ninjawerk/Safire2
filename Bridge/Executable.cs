using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Bridge
{
	[InheritedExport(typeof(Executable))]
	public class Executable : IPlugin, IDisposable
	{
		public String Icon { get; set; }
		public String Caption { get; set; }
		public String ToolTip { get; set; }

		public string Path { get; set; }
		public Color TileColor { get; set; }
		public bool UACElevation { get; set; }

		public void Dispose()
		{
			//throw new NotImplementedException();
		}

		public void Start()
		{
			ProcessStartInfo proc = new ProcessStartInfo();
			proc.UseShellExecute = true;
			proc.WorkingDirectory = Environment.CurrentDirectory;
			proc.FileName = Path;
			proc.Verb = "runas";

			try
			{
				Process.Start(proc);
			}
			catch
			{
				// The user refused the elevation.
				// Do nothing and return directly ...
				return;
			}
		}
	}
}
