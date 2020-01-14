using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive
{
    public static class AppLogic
    {
        public static bool HasStaticField(Type type, string fieldName)
        {
            return type.GetField(fieldName) != null;
        }

        public static bool HasStaticField<T>(string fieldName)
        {
            return HasStaticField(typeof(T), fieldName);
        }

        public static TReturn GetStaticField<TType, TReturn>(string fieldName)
        {
            Type type = typeof(TType);
            return GetStaticField<TReturn>(type, fieldName);
        }

        public static TReturn GetStaticField<TReturn>(Type type, string fieldName)
        {
            FieldInfo fi = type.GetField(fieldName);

            if (fi == null)
            {
                throw new Exception($"{type.Name} does not have a fieldName of {fieldName}");
            }

            object value = fi.GetValue(null);

            return (TReturn)ChangeType(value, typeof(TReturn));

        }

        public static bool HasProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }

        public static bool HasProperty<T>(string propertyName)
        {
            return HasProperty(typeof(T), propertyName);
        }

        public static TReturn GetPropertyValue<T, TReturn>(T obj, string name)
        {
            PropertyInfo pi = typeof(T).GetProperty(name);

            return (TReturn)ChangeType(pi.GetValue(obj, null), typeof(TReturn));
        }

        public static T GetPropertyValue<T>(Type type, object obj, string name)
        {
            PropertyInfo pi = type.GetProperty(name);

            return (T)ChangeType(pi.GetValue(obj, null), typeof(T));
        }

        public static bool CanViewHidden()
        {
            return Settings.Groups.Any(g => g == 4 || g == 6);
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (value is DBNull)
            {
                value = null;
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

    }
}
