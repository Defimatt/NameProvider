namespace NameProvider
{
    using NameFormatter = System.Linq.Expressions.Expression<System.Func<string>>;

    public class ProbabilisticNameFormatter
    {
        public NameFormatter NameFormatter { get; private set; }
        public int Chance { get; private set; }

        public static ProbabilisticNameFormatter Create(NameFormatter nameFormatter, int probability)
        {
            return new ProbabilisticNameFormatter { NameFormatter = nameFormatter, Chance = probability };
        }

        public static ProbabilisticNameFormatter CreateEqualChance(NameFormatter nameFormatter)
        {
            return new EqualChanceProbabilisticNameFormatter { NameFormatter = nameFormatter, Chance = 1 };
        }

        public class EqualChanceProbabilisticNameFormatter : ProbabilisticNameFormatter {}
    }
}