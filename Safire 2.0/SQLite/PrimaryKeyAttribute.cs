using System;

namespace Safire.SQLite
{
    [AttributeUsage (AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
}