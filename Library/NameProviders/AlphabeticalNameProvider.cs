using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NameProvider
{
    public class AlphabeticalNameProvider : NameProvider
    {
        public override Type Genericise<TEnum>() => typeof(AlphabeticalNameProvider<TEnum>);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AlphabeticalNameProvider<TEnum> : INameProvider<TEnum>
        where TEnum : struct, IFormattable, IConvertible, IComparable
    {
        protected readonly Random Random = new Random();

        public AlphabeticalNameProvider(IEnumerable<string> nameBucket, TEnum nameType)
        {
            NameType = nameType;
            Names = nameBucket.OrderBy(_ => _);
        }

        public TEnum NameType { get; }
        public IEnumerable<string> Names { get; protected set; }
    }
}