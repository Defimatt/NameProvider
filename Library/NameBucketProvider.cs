namespace NameProvider
{
    using System;
    using System.Collections.Generic;

    public abstract class NameBucketProvider<TProvider>
        where TProvider : NameBucketProvider<TProvider>, new()
    {
        public static NameBucketProvider<TProvider> Provider() { return new TProvider(); }
    
        public abstract IEnumerable<string> Fetch<TEnum>(TEnum nameType) where TEnum : struct, IFormattable, IConvertible, IComparable;
    }

    public static class NameBucketProvider
    {
        public static NameBucketProvider<DefaultNameBucketProvider> DefaultProvider() { return new DefaultNameBucketProvider(); }
    }
}