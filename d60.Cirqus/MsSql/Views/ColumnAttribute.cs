using System;

namespace d60.Cirqus.MsSql.Views
{
    /// <summary>
    /// Apply to a property to explicitly set the column name and/or size used in the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; private set; }
        public string Size { get; private set; }

        public ColumnAttribute(string name = null, string size = null)
        {
            ColumnName = name;
            Size = size;
        }
    }
}