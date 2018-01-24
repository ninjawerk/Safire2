using System;
using System.Runtime.InteropServices;

namespace Safire.SQLite
{
    public static class SQLite3
    {
        public enum Result : int
        {
            OK = 0,
            Error = 1,
            Internal = 2,
            Perm = 3,
            Abort = 4,
            Busy = 5,
            Locked = 6,
            NoMem = 7,
            ReadOnly = 8,
            Interrupt = 9,
            IOError = 10,
            Corrupt = 11,
            NotFound = 12,
            Full = 13,
            CannotOpen = 14,
            LockErr = 15,
            Empty = 16,
            SchemaChngd = 17,
            TooBig = 18,
            Constraint = 19,
            Mismatch = 20,
            Misuse = 21,
            NotImplementedLFS = 22,
            AccessDenied = 23,
            Format = 24,
            Range = 25,
            NonDBFile = 26,
            Row = 100,
            Done = 101
        }

        public enum ConfigOption : int
        {
            SingleThread = 1,
            MultiThread = 2,
            Serialized = 3
        }

#if !USE_CSHARP_SQLITE && !USE_WP8_NATIVE_SQLITE
        [DllImport("sqlite3", EntryPoint = "sqlite3_open", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Open ([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Open ([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db, int flags, IntPtr zvfs);
		
        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open(byte[] filename, out IntPtr db, int flags, IntPtr zvfs);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_close", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Close (IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_config", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Config (ConfigOption option);

        [DllImport("sqlite3", EntryPoint = "sqlite3_win32_set_directory", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        public static extern int SetDirectory (uint directoryType, string directoryPath);

        [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result BusyTimeout (IntPtr db, int milliseconds);

        [DllImport("sqlite3", EntryPoint = "sqlite3_changes", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Changes (IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Prepare2 (IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

        public static IntPtr Prepare2 (IntPtr db, string query)
        {
            IntPtr stmt;
            var r = Prepare2 (db, query, query.Length, out stmt, IntPtr.Zero);
            if (r == Result.OK) {
                //throw SQLiteException.New (r, GetErrmsg (db));
            }
            else if (r == Result.Busy)
            {
                while (r != Result.OK)
                {
                      r = Prepare2(db, query, query.Length, out stmt, IntPtr.Zero);
                }
            }
            return stmt;
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_step", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Step (IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_reset", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Reset (IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_finalize", CallingConvention=CallingConvention.Cdecl)]
        public static extern Result Finalize (IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid", CallingConvention=CallingConvention.Cdecl)]
        public static extern long LastInsertRowid (IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr Errmsg (IntPtr db);

        public static string GetErrmsg (IntPtr db)
        {
            return Marshal.PtrToStringUni (Errmsg (db));
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index", CallingConvention=CallingConvention.Cdecl)]
        public static extern int BindParameterIndex (IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null", CallingConvention=CallingConvention.Cdecl)]
        public static extern int BindNull (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int", CallingConvention=CallingConvention.Cdecl)]
        public static extern int BindInt (IntPtr stmt, int index, int val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64", CallingConvention=CallingConvention.Cdecl)]
        public static extern int BindInt64 (IntPtr stmt, int index, long val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double", CallingConvention=CallingConvention.Cdecl)]
        public static extern int BindDouble (IntPtr stmt, int index, double val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int BindText (IntPtr stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, IntPtr free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob", CallingConvention=CallingConvention.Cdecl)]
        public static extern int BindBlob (IntPtr stmt, int index, byte[] val, int n, IntPtr free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_count", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ColumnCount (IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr ColumnName (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr ColumnName16Internal (IntPtr stmt, int index);
        public static string ColumnName16(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnName16Internal(stmt, index));
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_type", CallingConvention=CallingConvention.Cdecl)]
        public static extern ColType ColumnType (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ColumnInt (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64", CallingConvention=CallingConvention.Cdecl)]
        public static extern long ColumnInt64 (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_double", CallingConvention=CallingConvention.Cdecl)]
        public static extern double ColumnDouble (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr ColumnText (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr ColumnText16 (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr ColumnBlob (IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes", CallingConvention=CallingConvention.Cdecl)]
        public static extern int ColumnBytes (IntPtr stmt, int index);

        public static string ColumnString (IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni (SQLite3.ColumnText16 (stmt, index));
        }

        public static byte[] ColumnByteArray (IntPtr stmt, int index)
        {
            int length = ColumnBytes (stmt, index);
            byte[] result = new byte[length];
            if (length > 0)
                Marshal.Copy (ColumnBlob (stmt, index), result, 0, length);
            return result;
        }
#else
        public static Result Open(string filename, out Sqlite3DatabaseHandle db)
        {
            return (Result) Sqlite3.sqlite3_open(filename, out db);
        }

		public static Result Open(string filename, out Sqlite3DatabaseHandle db, int flags, IntPtr zVfs)
		{
#if USE_WP8_NATIVE_SQLITE
			return (Result)Sqlite3.sqlite3_open_v2(filename, out db, flags, "");
#else
			return (Result)Sqlite3.sqlite3_open_v2(filename, out db, flags, null);
#endif
		}

		public static Result Close(Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close(db);
		}

		public static Result BusyTimeout(Sqlite3DatabaseHandle db, int milliseconds)
		{
			return (Result)Sqlite3.sqlite3_busy_timeout(db, milliseconds);
		}

		public static int Changes(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_changes(db);
		}

		public static Sqlite3Statement Prepare2(Sqlite3DatabaseHandle db, string query)
		{
			Sqlite3Statement stmt = default(Sqlite3Statement);
#if USE_WP8_NATIVE_SQLITE
			var r = Sqlite3.sqlite3_prepare_v2(db, query, out stmt);
#else
			stmt = new Sqlite3Statement();
			var r = Sqlite3.sqlite3_prepare_v2(db, query, -1, ref stmt, 0);
#endif
			if (r != 0)
			{
				throw SQLiteException.New((Result)r, GetErrmsg(db));
			}
			return stmt;
		}

		public static Result Step(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_step(stmt);
		}

		public static Result Reset(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_reset(stmt);
		}

		public static Result Finalize(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_finalize(stmt);
		}

		public static long LastInsertRowid(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_last_insert_rowid(db);
		}

		public static string GetErrmsg(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_errmsg(db);
		}

		public static int BindParameterIndex(Sqlite3Statement stmt, string name)
		{
			return Sqlite3.sqlite3_bind_parameter_index(stmt, name);
		}

		public static int BindNull(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_bind_null(stmt, index);
		}

		public static int BindInt(Sqlite3Statement stmt, int index, int val)
		{
			return Sqlite3.sqlite3_bind_int(stmt, index, val);
		}

		public static int BindInt64(Sqlite3Statement stmt, int index, long val)
		{
			return Sqlite3.sqlite3_bind_int64(stmt, index, val);
		}

		public static int BindDouble(Sqlite3Statement stmt, int index, double val)
		{
			return Sqlite3.sqlite3_bind_double(stmt, index, val);
		}

		public static int BindText(Sqlite3Statement stmt, int index, string val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n);
#else
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n, null);
#endif
		}

		public static int BindBlob(Sqlite3Statement stmt, int index, byte[] val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n);
#else
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n, null);
#endif
		}

		public static int ColumnCount(Sqlite3Statement stmt)
		{
			return Sqlite3.sqlite3_column_count(stmt);
		}

		public static string ColumnName(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static string ColumnName16(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static ColType ColumnType(Sqlite3Statement stmt, int index)
		{
			return (ColType)Sqlite3.sqlite3_column_type(stmt, index);
		}

		public static int ColumnInt(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int(stmt, index);
		}

		public static long ColumnInt64(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int64(stmt, index);
		}

		public static double ColumnDouble(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_double(stmt, index);
		}

		public static string ColumnText(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static string ColumnText16(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnBlob(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_blob(stmt, index);
		}

		public static int ColumnBytes(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_bytes(stmt, index);
		}

		public static string ColumnString(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnByteArray(Sqlite3Statement stmt, int index)
		{
			return ColumnBlob(stmt, index);
		}
#endif

        public enum ColType : int
        {
            Integer = 1,
            Float = 2,
            Text = 3,
            Blob = 4,
            Null = 5
        }
    }
}