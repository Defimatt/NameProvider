using static System.Linq.Enumerable;

namespace NameProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using static MaleFemaleGendersAndSurname;

    using static ProbabilisticNameFormatter;

    public struct ProbabilisticNameFormatter
    {
        public Expression<Func<string>> NameFormatter { get; set; }
        public double Chance { get; set; }

        public static ProbabilisticNameFormatter Create(Expression<Func<string>> nameFormatter, double probability)
        {
            return new ProbabilisticNameFormatter { NameFormatter = nameFormatter, Chance = probability };
        }
    }

    public static class ExtensionMethods
    {
        public static ProbabilisticNameFormatter AddProbability(this Expression<Func<string>> @this, double probability)
        {
            return new ProbabilisticNameFormatter { NameFormatter = @this, Chance = probability };
        }
    }

    internal class Example
    {
        private static NameProviderFactory<MaleFemaleGendersAndSurname, RandomNameProvider> np = new NameProviderFactory<MaleFemaleGendersAndSurname, RandomNameProvider>();
        private static Random random = new Random();
        private static void Main(string[] args)
        {
            var nameFormats = new List<ProbabilisticNameFormatter>
            {
                Create(() => $"{Male} {Surname}", 50),
                Create(() => $"{Male} {Male} {Surname}", 20),
                Create(() => $"{Male} {Male} {Male} {Surname}", 10),
                Create(() => $"{Male} {Surname}-{Surname}", 10),
                Create(() => $"{Male} {Male} {Surname}-{Surname}", 6),
                Create(() => $"{Male} {Male} {Male} {Surname}-{Surname}", 4)
            };

            var uu = (Expression<Func<string>>)(new CompositeNameGeneratorExtensions.EnumModifier<MaleFemaleGendersAndSurname>(null, null).Modify(nameFormats[5].NameFormatter));

            //List<string> nameList = new List<string>();

            //for (int i = 0; i < 300; i++)
            //{

            //    var malenamegenerator = $@"{string.Join(" ", np.NamesForNameType(MaleFemaleGendersAndSurname.Male).Take(random.Next(1, 3)))} {
            //    string.Join("-", np.NamesForNameType(MaleFemaleGendersAndSurname.Surname).Take(random.Next(1, 2)))}";

            //    nameList.Add(malenamegenerator);
            //}


            var fnp = new FullNameGenerator();
            
            //var maleNames = fnp.Names(Male);
            //var femaleNames = fnp.Names(Female);

            fnp = new FullNameGenerator();
            //var allNames = fnp.AllNames(Male).Take(200000).ToList();

            var cng = new CompositeNameGenerator<MaleFemaleGendersAndSurname>();
            cng.SetConverter(() => $"{Male} {(Func<IEnumerable<MaleFemaleGendersAndSurname>>)(() => Repeat(Male, new Random().Next(0, 2)))} {Surname}");
        }
    }
}