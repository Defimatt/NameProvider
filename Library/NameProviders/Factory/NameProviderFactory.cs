using System;
using System.Collections.Generic;
using System.Linq;

namespace NameProvider
{
    public class NameProviderFactory<TEnum, TNameProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
        where TNameProvider : NameProvider, new()
    {
        public NameProviderFactory()
        {
            NameProviders = EnumHelpers.GetValues<TEnum>()
                .Aggregate(NameProviders,
                    (providers, @enum) =>
                        providers.Concat(
                            (INameProvider<TEnum>)
                                Activator.CreateInstance(new TNameProvider().Genericise<TEnum>(),
                                    NameBucketProvider.Fetch(@enum), @enum)))
                .Where(provider => provider.Names.Any());
        }

        public IEnumerable<INameProvider<TEnum>> NameProviders { get; protected set; } =
            new List<INameProvider<TEnum>>();

        public bool Supports(TEnum nameType)
            => NameProviders.Select(nameProvider => nameProvider.NameType).Contains(nameType);

        public IEnumerable<TEnum> SupportedNameTypes()
            => NameProviders.Select(nameProvider => nameProvider.NameType);

        public INameProvider<TEnum> ProviderForNameType(TEnum nameType)
            => NameProviders.Single(nameProvider => nameProvider.NameType.Equals(nameType));

        public IEnumerable<string> NamesForNameType(TEnum nameType)
            => ProviderForNameType(nameType).Names;
    }
}