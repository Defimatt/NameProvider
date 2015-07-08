namespace NameProvider
{
    using System;

    public abstract class NameProvider
    {
        public abstract Type Genericise<TEnum>() where TEnum : struct, IFormattable, IConvertible, IComparable;
    }
}