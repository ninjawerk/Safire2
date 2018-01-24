using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Safire.Library.Core
{
    public class Main
    {
        public static void Initialize()
        {
            UpdateListens.Instance.Dummy();
        }
		public static string MyPath()
		{
			String exePath = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
			string dir = Path.GetDirectoryName(exePath);
			return dir;
		}
    }
}
