using System;
namespace YouChewArchive
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IgnoreAttribute : Attribute
    {
    }
}
