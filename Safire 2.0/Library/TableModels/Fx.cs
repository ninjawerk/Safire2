using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Safire.SQLite;

namespace Safire.Library.TableModels
{
		[Serializable]
	public class Fx
	{
		//Basic
	
		[PrimaryKey]
		public string ID { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public string Author { get; set; }
		public string Data { get; set; }

		 
	}
}
