using System;

namespace Safire.SQLite
{
    [AttributeUsage (AttributeTargets.Property)]
    public class UniqueAttribute : IndexedAttribute
    {
        public override bool Unique {
            get { return true; }
            set { /* throw?  */ }
        }
    }
}