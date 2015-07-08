namespace NameProvider
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public sealed class DefaultNameBucketProvider : NameBucketProvider<DefaultNameBucketProvider>
    {
        public override IEnumerable<string> Fetch<TEnum>(TEnum nameType)
        {
            var filename = string.Join(";", nameType.GetType().ToString(), nameType.ToString(CultureInfo.InvariantCulture));
            return !File.Exists(filename) ? Enumerable.Empty<string>() : File.ReadAllLines(filename);
        }
    }
}