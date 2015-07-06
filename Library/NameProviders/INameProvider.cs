using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NameProvider
{
    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant", Justification = "Can't subclass System.Enum")]
    public interface INameProvider<TEnum> where TEnum : struct, IFormattable, IConvertible, IComparable
    {
        IEnumerable<string> Names { get; }
        TEnum NameType { get; }
    }
}