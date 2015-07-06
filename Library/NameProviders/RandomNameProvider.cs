using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NameProvider
{
    public class RandomNameProvider : NameProvider
    {
        public override Type Genericise<TEnum>() => typeof(RandomNameProvider<TEnum>);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RandomNameProvider<TEnum> : INameProvider<TEnum>
        where TEnum : struct, IFormattable, IConvertible, IComparable
    {
        protected readonly Random Random = new Random();

        public RandomNameProvider(IEnumerable<string> nameBucket, TEnum nameType)
        {
            NameType = nameType;
            var enumerable = nameBucket as string[] ?? nameBucket as IList<string> ?? nameBucket.ToList();
            Names = Enumerable.Range(0, enumerable.Count())
                .Select(index => new {Random = Random.Next(), Index = index})
                .OrderBy(tuple => tuple.Random)
                .Select(tuple => tuple.Index).Select(enumerable.ElementAt);
        }

        public TEnum NameType { get; }
        public IEnumerable<string> Names { get; protected set; }
    }
}