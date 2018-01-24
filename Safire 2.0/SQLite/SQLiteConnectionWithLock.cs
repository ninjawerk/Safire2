using System;
using System.Threading;

namespace Safire.SQLite
{
    class SQLiteConnectionWithLock : SQLiteConnection
    {
        readonly object _lockPoint = new object ();

        public SQLiteConnectionWithLock (SQLiteConnectionString connectionString)
            : base (connectionString.DatabasePath, connectionString.StoreDateTimeAsTicks)
        {
        }

        public IDisposable Lock ()
        {
            return new LockWrapper (_lockPoint);
        }

        private class LockWrapper : IDisposable
        {
            object _lockPoint;

            public LockWrapper (object lockPoint)
            {
                _lockPoint = lockPoint;
                Monitor.Enter (_lockPoint);
            }

            public void Dispose ()
            {
                Monitor.Exit (_lockPoint);
            }
        }
    }
}