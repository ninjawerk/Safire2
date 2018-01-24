using System;

namespace Safire.SQLite
{
    [AttributeUsage (AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute (string name)
        {
            Name = name;
        }
    }
}