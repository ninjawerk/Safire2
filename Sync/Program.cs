using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Safire.Library.TableModels;
using Safire.SQLite;

namespace Sync
{
	class Program
	{
		static void Main(string[] args)
		{
			start();
		}
		public static void start()
		{
			Tables.Initialize();
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				var query =
					db.Table<Artist>();					

				foreach (var v in query)
				{
					try
					{
						var q =
							db.Table<Track>().Where(c => c.Artist == v.Name);
						foreach (var vv in q)
						{
							File.Copy(vv.Path, @"I:\Songs\" + v.Name + "\\" + (new FileInfo(vv.Path)).Name);
							Console.WriteLine(vv.Title);
						}
						Directory.CreateDirectory(@"I:\Songs\" + v.Name);
						Console.WriteLine(v.Name);
					}
					catch (Exception)
					{

					}
				}



			}




		}
	}
}
