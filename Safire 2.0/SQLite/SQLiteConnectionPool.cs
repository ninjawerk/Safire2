using System.Collections.Generic;

namespace Safire.SQLite
{
    class SQLiteConnectionPool
    {
        class Entry
        {
            public SQLiteConnectionString ConnectionString { get; private set; }
            public SQLiteConnectionWithLock Connection { get; private set; }

            public Entry (SQLiteConnectionString connectionString)
            {
                ConnectionString = connectionString;
                Connection = new SQLiteConnectionWithLock (connectionString);
            }

            public void OnApplicationSuspended ()
            {
                Connection.Dispose ();
                Connection = null;
            }
        }

        readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry> ();
        readonly object _entriesLock = new object ();

        static readonly SQLiteConnectionPool _shared = new SQLiteConnectionPool ();

        /// <summary>
        /// Gets the singleton instance of the connection tool.
        /// </summary>
        public static SQLiteConnectionPool Shared
        {
            get
            {
                return _shared;
            }
        }

        public SQLiteConnectionWithLock GetConnection (SQLiteConnectionString connectionString)
        {
            lock (_entriesLock) {
                Entry entry;
                string key = connectionString.ConnectionString;

                if (!_entries.TryGetValue (key, out entry)) {
                    entry = new Entry (connectionString);
                    _entries[key] = entry;
                }

                return entry.Connection;
            }
        }

        /// <summary>
        /// Closes all connections managed by this pool.
        /// </summary>
        public void Reset ()
        {
            lock (_entriesLock) {
                foreach (var entry in _entries.Values) {
                    entry.OnApplicationSuspended ();
                }
                _entries.Clear ();
            }
        }

        /// <summary>
        /// Call this method when the application is suspended.
        /// </summary>
        /// <remarks>Behaviour here is to close any open connections.</remarks>
        public void ApplicationSuspended ()
        {
            Reset ();
        }
    }
}