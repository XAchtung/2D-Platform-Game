using System;
using System.Linq;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PropertyGroupAttribute : Attribute
    {
        public readonly string path;
        public readonly string name;
        public readonly int groupDepth;

        public PropertyGroupAttribute(string groupPath)
        {
            this.path = groupPath ?? string.Empty;
            string[] parts = path.Split('/');
            name = parts.Length > 0 ? parts[parts.Length - 1] : string.Empty;
            groupDepth = path.Count(x => x == '/');
        }
    }
}