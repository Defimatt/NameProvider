using System;

namespace NameProvider
{
    public static class EnumHelpers
    {
        public static T Parse<T>(object value) => (T)Enum.Parse(typeof(T), value.ToString());
        public static T[] GetValues<T>() => (T[])Enum.GetValues(typeof(T));
    }
}