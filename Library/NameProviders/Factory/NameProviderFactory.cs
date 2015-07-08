namespace NameProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static System.Activator;
    using static EnumHelpers;

    public class NameProviderFactory : NameProviderFactory<MaleFemaleGendersAndSurname, RandomNameProvider, DefaultNameBucketProvider> { }

    public class NameProviderFactory<TEnum> : NameProviderFactory<TEnum, RandomNameProvider, DefaultNameBucketProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
    {}

    public class NameProviderFactory<TEnum, TNameProvider> : NameProviderFactory<TEnum, TNameProvider, DefaultNameBucketProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
        where TNameProvider : NameProvider, new()
    {}

    public class NameProviderFactorySpecial<TNameProvider> : NameProviderFactory<MaleFemaleGendersAndSurname, TNameProvider, DefaultNameBucketProvider>
        where TNameProvider : NameProvider, new()
    {}

    public class NameProviderFactorySpecial<TEnum, TNameBucketProvider> : NameProviderFactory<TEnum, RandomNameProvider, TNameBucketProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
        where TNameBucketProvider : NameBucketProvider<TNameBucketProvider>, new()
    {}

    public class NameProviderFactorySpecial2<TNameBucketProvider> : NameProviderFactory<MaleFemaleGendersAndSurname, RandomNameProvider, TNameBucketProvider>
        where TNameBucketProvider : NameBucketProvider<TNameBucketProvider>, new()
    {}

    public class NameProviderFactorySpecial2<TNameProvider, TNameBucketProvider> : NameProviderFactory<MaleFemaleGendersAndSurname, TNameProvider, TNameBucketProvider>
        where TNameProvider : NameProvider, new()
        where TNameBucketProvider : NameBucketProvider<TNameBucketProvider>, new()
    {}

    public class NameProviderFactory<TEnum, TNameProvider, TNameBucketProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
        where TNameProvider : NameProvider, new()
        where TNameBucketProvider : NameBucketProvider<TNameBucketProvider>, new()
    {
        public NameProviderFactory()
        {
            NameProviders =
                GetValues<TEnum>()
                    .Aggregate(
                        NameProviders,
                        (providers, @enum) =>
                        providers.Concat(
                            (INameProvider<TEnum>)
                            CreateInstance(
                                new TNameProvider().Genericise<TEnum>(),
                                NameBucketProvider<TNameBucketProvider>.Provider().Fetch(@enum),
                                @enum)))
                    .Where(provider => provider.Names.Any());
        }

        protected IEnumerable<INameProvider<TEnum>> NameProviders { get; } = new List<INameProvider<TEnum>>();

        public bool Supports(TEnum nameType) => NameProviders.Select(nameProvider => nameProvider.NameType).Contains(nameType);

        public IEnumerable<TEnum> SupportedNameTypes() => NameProviders.Select(nameProvider => nameProvider.NameType);

        public INameProvider<TEnum> ProviderForNameType(TEnum nameType)
            => NameProviders.Single(nameProvider => nameProvider.NameType.Equals(nameType));

        public IEnumerable<string> NamesForNameType(TEnum nameType) => ProviderForNameType(nameType).Names;
    }
}