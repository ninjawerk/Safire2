using System;
using System.Diagnostics;

namespace Safire.SQLite
{
    /// <summary>
    /// Since the insert never changed, we only need to prepare once.
    /// </summary>
    public class PreparedSqlLiteInsertCommand : IDisposable
    {
        public bool Initialized { get; set; }

        protected SQLiteConnection Connection { get; set; }

        public string CommandText { get; set; }

        protected IntPtr Statement { get; set; }
        internal static readonly IntPtr NullStatement = default(IntPtr);

        internal PreparedSqlLiteInsertCommand (SQLiteConnection conn)
        {
            Connection = conn;
        }

        public int ExecuteNonQuery (object[] source)
        {
            if (Connection.Trace) {
                Debug.WriteLine ("Executing: " + CommandText);
            }

            var r = SQLite3.Result.OK;

            if (!Initialized) {
                Statement = Prepare ();
                Initialized = true;
            }

            //bind the values.
            if (source != null) {
                for (int i = 0; i < source.Length; i++) {
                    SQLiteCommand.BindParameter (Statement, i + 1, source [i], Connection.StoreDateTimeAsTicks);
                }
            }
            r = SQLite3.Step (Statement);

            if (r == SQLite3.Result.Done) {
                int rowsAffected = SQLite3.Changes (Connection.Handle);
                SQLite3.Reset (Statement);
                return rowsAffected;
            } else if (r == SQLite3.Result.Error) {
                string msg = SQLite3.GetErrmsg (Connection.Handle);
                SQLite3.Reset (Statement);
                throw SQLiteException.New (r, msg);
            } else {
                SQLite3.Reset (Statement);
                throw SQLiteException.New (r, r.ToString ());
            }
        }

        protected virtual IntPtr Prepare ()
        {
            var stmt = SQLite3.Prepare2 (Connection.Handle, CommandText);
            return stmt;
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        private void Dispose (bool disposing)
        {
            if (Statement != NullStatement) {
                try {
                    SQLite3.Finalize (Statement);
                } finally {
                    Statement = NullStatement;
                    Connection = null;
                }
            }
        }

        ~PreparedSqlLiteInsertCommand ()
        {
            Dispose (false);
        }
    }
}