using System;
using System.Collections.Generic;

namespace Safire.SQLite
{
    public class CreateTablesResult
    {
        public Dictionary<Type, int> Results { get; private set; }

        internal CreateTablesResult ()
        {
            this.Results = new Dictionary<Type, int> ();
        }
    }
}