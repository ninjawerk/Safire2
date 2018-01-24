namespace Safire.SQLite
{
    /// <summary>
    /// Represents a parsed connection string.
    /// </summary>
    class SQLiteConnectionString
    {
        public string ConnectionString { get; private set; }
        public string DatabasePath { get; private set; }
        public bool StoreDateTimeAsTicks { get; private set; }

#if NETFX_CORE
		static readonly string MetroStyleDataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

        public SQLiteConnectionString (string databasePath, bool storeDateTimeAsTicks)
        {
            ConnectionString = databasePath;
            StoreDateTimeAsTicks = storeDateTimeAsTicks;

#if NETFX_CORE
			DatabasePath = System.IO.Path.Combine (MetroStyleDataPath, databasePath);
#else
            DatabasePath = databasePath;
#endif
        }
    }
}