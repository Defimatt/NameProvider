using static System.Globalization.CultureInfo;
using static System.String;

namespace NameProvider
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class NameBucketProvider
    {
        public static IEnumerable<string> Fetch<TEnum>(TEnum nameType) where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            var filename = Join(";", nameType.GetType().ToString(), nameType.ToString(InvariantCulture));
            return !File.Exists(filename) ? Enumerable.Empty<string>() : File.ReadAllLines(filename);
        }
    }
}