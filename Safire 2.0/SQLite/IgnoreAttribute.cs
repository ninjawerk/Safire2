using System;

namespace Safire.SQLite
{
    [AttributeUsage (AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}