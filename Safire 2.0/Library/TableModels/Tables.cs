using System;
using System.IO;
using Safire.SQLite;

namespace Safire.Library.TableModels
{
    internal class Tables
    {
        public static string DBPath { get; set; }

        public static void Initialize()
        {
            DBPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hyperLib.emlib");

            string tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                             @"\Safire";
            if (!Directory.Exists(tString)) Directory.CreateDirectory(tString);


            tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                      @"\Safire\ArtistData";
            if (!Directory.Exists(tString)) Directory.CreateDirectory(tString);

			tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
				  @"\Safire\Cache";
			if (!Directory.Exists(tString)) Directory.CreateDirectory(tString);

            tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                    @"\Safire\AlbumData";
            if (!Directory.Exists(tString))  Directory.CreateDirectory(tString);

            tString = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) +
                  @"\Safire\";
            if (!Directory.Exists(tString)) Directory.CreateDirectory(tString);

            using (var db = new SQLiteConnection(DBPath))
            {
                // Create the tables if they don't exist
                db.CreateTable<Track>();
                db.CreateTable<Artist>();
                db.CreateTable<Album>();
                db.CreateTable<Genre>();
                db.CreateTable<Playlist>();
				db.CreateTable<Fx>();
            }
        }
    }
}