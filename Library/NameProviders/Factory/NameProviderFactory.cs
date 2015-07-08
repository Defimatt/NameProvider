namespace NameProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static System.Activator;
    using static EnumHelpers;
    using static NameBucketProvider;

    public class NameProviderFactory<TEnum, TNameProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
        where TNameProvider : NameProvider, new()
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
                            CreateInstance(new TNameProvider().Genericise<TEnum>(), Fetch(@enum), @enum)))
                    .Where(provider => provider.Names.Any());
        }

        public IEnumerable<INameProvider<TEnum>> NameProviders { get; protected set; } =
            new List<INameProvider<TEnum>>();

        public bool Supports(TEnum nameType)
            => NameProviders.Select(nameProvider => nameProvider.NameType).Contains(nameType);

        public IEnumerable<TEnum> SupportedNameTypes() => NameProviders.Select(nameProvider => nameProvider.NameType);

        public INameProvider<TEnum> ProviderForNameType(TEnum nameType)
            => NameProviders.Single(nameProvider => nameProvider.NameType.Equals(nameType));

        public IEnumerable<string> NamesForNameType(TEnum nameType) => ProviderForNameType(nameType).Names;
    }
}