using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Upd
{
	[Serializable]
	public class Definition
	{
		public string CodeName { get; set; }
		public string PostInstallRun { get; set; }
		public double Version { get; set; }
		public string Author { set; get; }
		public DateTime ReleasedDateTime { get; set; }
		public string PatchNotes { get; set; }
		public string URL { get; set; }

	}
}
