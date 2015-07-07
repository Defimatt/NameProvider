using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;
using static NameProvider.EnumHelpers;

namespace NameProvider
{
    public class FullNameGenerator
    {
        protected readonly NameProviderFactory<MaleFemaleGendersAndSurname, RandomNameProvider> ProviderFactory;

        public FullNameGenerator()
            : this(new NameProviderFactory<MaleFemaleGendersAndSurname, RandomNameProvider>())
        {
        }

        public FullNameGenerator(
            NameProviderFactory<MaleFemaleGendersAndSurname, RandomNameProvider> nameProviderFactory)
        {
            ProviderFactory = nameProviderFactory;
        }

        public string NextName(FullNameType fullNameType)
        {
            return $@"{
                ProviderFactory.ProviderForNameType(Parse<MaleFemaleGendersAndSurname>(fullNameType))
                    .Names.Take(1)
                    .Single()} {
                ProviderFactory.ProviderForNameType(MaleFemaleGendersAndSurname.Surname)
                    .Names.Take(1)
                    .Single()}";
        }

        public IEnumerable<T> Names<T>(FullNameType fullNameType, Func<string, string, T> joinFunc)
        {
            return
                ProviderFactory.ProviderForNameType(Parse<MaleFemaleGendersAndSurname>(fullNameType))
                    .Names.Zip(ProviderFactory.ProviderForNameType(MaleFemaleGendersAndSurname.Surname).Names,
                        joinFunc ?? ((firstName, surname) => (T)Convert.ChangeType($"{firstName} {surname}", typeof(T))));
        }

        public IEnumerable<string> Names(FullNameType fullNameType)
        {
            return Names<string>(fullNameType, null);
        }

        public IEnumerable<string> AllNames(FullNameType fullNameType)
        {
            return new List<IEnumerable<string>>
            {
                ProviderFactory.ProviderForNameType(Parse<MaleFemaleGendersAndSurname>(fullNameType))
                    .Names,
                ProviderFactory.ProviderForNameType(MaleFemaleGendersAndSurname.Surname).Names
            }.CartesianProduct().Select(_ => Join(" ", _));
        }

        public IEnumerable<string> AllNames()
        {
            return GetValues<FullNameType>().Select(AllNames).SelectMany(_ =>
            {
                var enumerable = _ as string[] ?? _ as IList<string> ?? _.ToList();
                return enumerable;
            });
        }
    }
}