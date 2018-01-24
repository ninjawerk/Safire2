using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Safire.SQLite
{
    public class TableMapping
    {
        public Type MappedType { get; private set; }

        public string TableName { get; private set; }

        public Column[] Columns { get; private set; }

        public Column PK { get; private set; }

        public string GetByPrimaryKeySql { get; private set; }

        Column _autoPk = null;
        Column[] _insertColumns = null;
        Column[] _insertOrReplaceColumns = null;

        public TableMapping (Type type)
        {
            MappedType = type;

#if NETFX_CORE
			var tableAttr = (TableAttribute)System.Reflection.CustomAttributeExtensions
                .GetCustomAttribute(type.GetTypeInfo(), typeof(TableAttribute), true);
#else
            var tableAttr = (TableAttribute)type.GetCustomAttributes (typeof (TableAttribute), true).FirstOrDefault ();
#endif

            TableName = tableAttr != null ? tableAttr.Name : MappedType.Name;

#if !NETFX_CORE
            var props = MappedType.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
#else
			var props = from p in MappedType.GetRuntimeProperties()
						where ((p.GetMethod != null && p.GetMethod.IsPublic) || (p.SetMethod != null && p.SetMethod.IsPublic) || (p.GetMethod != null && p.GetMethod.IsStatic) || (p.SetMethod != null && p.SetMethod.IsStatic))
						select p;
#endif
            var cols = new List<Column> ();
            foreach (var p in props) {
#if !NETFX_CORE
                var ignore = p.GetCustomAttributes (typeof(IgnoreAttribute), true).Length > 0;
#else
				var ignore = p.GetCustomAttributes (typeof(IgnoreAttribute), true).Count() > 0;
#endif
                if (p.CanWrite && !ignore) {
                    cols.Add (new Column (p));
                }
            }
            Columns = cols.ToArray ();
            foreach (var c in Columns) {
                if (c.IsAutoInc && c.IsPK) {
                    _autoPk = c;
                }
                if (c.IsPK) {
                    PK = c;
                }
            }
			
            HasAutoIncPK = _autoPk != null;

            if (PK != null) {
                GetByPrimaryKeySql = string.Format ("select * from \"{0}\" where \"{1}\" = ?", TableName, PK.Name);
            }
            else {
                // People should not be calling Get/Find without a PK
                GetByPrimaryKeySql = string.Format ("select * from \"{0}\" limit 1", TableName);
            }
        }

        public bool HasAutoIncPK { get; private set; }

        public void SetAutoIncPK (object obj, long id)
        {
            if (_autoPk != null) {
                _autoPk.SetValue (obj, Convert.ChangeType (id, _autoPk.ColumnType, null));
            }
        }

        public Column[] InsertColumns {
            get {
                if (_insertColumns == null) {
                    _insertColumns = Columns.Where (c => !c.IsAutoInc).ToArray ();
                }
                return _insertColumns;
            }
        }

        public Column[] InsertOrReplaceColumns {
            get {
                if (_insertOrReplaceColumns == null) {
                    _insertOrReplaceColumns = Columns.ToArray ();
                }
                return _insertOrReplaceColumns;
            }
        }

        public Column FindColumnWithPropertyName (string propertyName)
        {
            var exact = Columns.Where (c => c.PropertyName == propertyName).FirstOrDefault ();
            return exact;
        }

        public Column FindColumn (string columnName)
        {
            var exact = Columns.Where (c => c.Name == columnName).FirstOrDefault ();
            return exact;
        }
		
        PreparedSqlLiteInsertCommand _insertCommand;
        string _insertCommandExtra = null;

        public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
        {
            if (_insertCommand == null) {
                _insertCommand = CreateInsertCommand(conn, extra);
                _insertCommandExtra = extra;
            }
            else if (_insertCommandExtra != extra) {
                _insertCommand.Dispose();
                _insertCommand = CreateInsertCommand(conn, extra);
                _insertCommandExtra = extra;
            }
            return _insertCommand;
        }
		
        private PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra)
        {
            var cols = InsertColumns;
            string insertSql;
            if (!cols.Any() && Columns.Count() == 1 && Columns[0].IsAutoInc)
            {
                insertSql = string.Format("insert {1} into \"{0}\" default values", TableName, extra);
            }
            else
            {
                var replacing = string.Compare (extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

                if (replacing) {
                    cols = InsertOrReplaceColumns;
                }

                insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", TableName,
                    string.Join(",", (from c in cols
                        select "\"" + c.Name + "\"").ToArray()),
                    string.Join(",", (from c in cols
                        select "?").ToArray()), extra);
                
            }
			
            var insertCommand = new PreparedSqlLiteInsertCommand(conn);
            insertCommand.CommandText = insertSql;
            return insertCommand;
        }
		
        protected internal void Dispose()
        {
            if (_insertCommand != null) {
                _insertCommand.Dispose();
                _insertCommand = null;
            }
        }

        public class Column
        {
            PropertyInfo _prop;

            public string Name { get; private set; }

            public string PropertyName { get { return _prop.Name; } }

            public Type ColumnType { get; private set; }

            public string Collation { get; private set; }

            public bool IsAutoInc { get; private set; }

            public bool IsPK { get; private set; }

            public IEnumerable<IndexedAttribute> Indices { get; set; }

            public bool IsNullable { get; private set; }

            public int MaxStringLength { get; private set; }

            public Column (PropertyInfo prop)
            {
                var colAttr = (ColumnAttribute)prop.GetCustomAttributes (typeof(ColumnAttribute), true).FirstOrDefault ();

                _prop = prop;
                Name = colAttr == null ? prop.Name : colAttr.Name;
                //If this type is Nullable<T> then Nullable.GetUnderlyingType returns the T, otherwise it returns null, so get the the actual type instead
                ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                Collation = Orm.Collation (prop);
                IsAutoInc = Orm.IsAutoInc (prop);
                IsPK = Orm.IsPK (prop);
                Indices = Orm.GetIndices(prop);
                IsNullable = !IsPK;
                MaxStringLength = Orm.MaxStringLength (prop);
            }

            public void SetValue (object obj, object val)
            {
                _prop.SetValue (obj, val, null);
            }

            public object GetValue (object obj)
            {
                return _prop.GetValue (obj, null);
            }
        }
    }
}