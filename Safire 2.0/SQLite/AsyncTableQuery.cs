using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Safire.SQLite
{
    public class AsyncTableQuery<T>
        where T : new ()
    {
        TableQuery<T> _innerQuery;

        public AsyncTableQuery (TableQuery<T> innerQuery)
        {
            _innerQuery = innerQuery;
        }

        public AsyncTableQuery<T> Where (Expression<Func<T, bool>> predExpr)
        {
            return new AsyncTableQuery<T> (_innerQuery.Where (predExpr));
        }

        public AsyncTableQuery<T> Skip (int n)
        {
            return new AsyncTableQuery<T> (_innerQuery.Skip (n));
        }

        public AsyncTableQuery<T> Take (int n)
        {
            return new AsyncTableQuery<T> (_innerQuery.Take (n));
        }

        public AsyncTableQuery<T> OrderBy<U> (Expression<Func<T, U>> orderExpr)
        {
            return new AsyncTableQuery<T> (_innerQuery.OrderBy<U> (orderExpr));
        }

        public AsyncTableQuery<T> OrderByDescending<U> (Expression<Func<T, U>> orderExpr)
        {
            return new AsyncTableQuery<T> (_innerQuery.OrderByDescending<U> (orderExpr));
        }

        public Task<List<T>> ToListAsync ()
        {
            return Task.Factory.StartNew (() => {
                                                    using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock ()) {
                                                        return _innerQuery.ToList ();
                                                    }
            });
        }

        public Task<int> CountAsync ()
        {
            return Task.Factory.StartNew (() => {
                                                    using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock ()) {
                                                        return _innerQuery.Count ();
                                                    }
            });
        }

        public Task<T> ElementAtAsync (int index)
        {
            return Task.Factory.StartNew (() => {
                                                    using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock ()) {
                                                        return _innerQuery.ElementAt (index);
                                                    }
            });
        }

        public Task<T> FirstAsync ()
        {
            return Task<T>.Factory.StartNew(() => {
                                                      using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock ()) {
                                                          return _innerQuery.First ();
                                                      }
            });
        }

        public Task<T> FirstOrDefaultAsync ()
        {
            return Task<T>.Factory.StartNew(() => {
                                                      using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock ()) {
                                                          return _innerQuery.FirstOrDefault ();
                                                      }
            });
        }
    }
}