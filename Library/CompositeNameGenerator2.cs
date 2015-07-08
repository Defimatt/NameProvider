using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NameProvider;
using Weighted_Randomizer;

namespace NameProvider
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using static EnumHelpers;

    public struct ProbabilisticNameFormatter
    {
        public Expression<Func<string>> NameFormatter { get; private set; }
        public int Chance { get; private set; }

        public static ProbabilisticNameFormatter Create(Expression<Func<string>> nameFormatter, int probability)
        {
            return new ProbabilisticNameFormatter { NameFormatter = nameFormatter, Chance = probability };
        }
    }

    public class CompositeNameGenerator2<TEnum, TNameProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
        where TNameProvider : NameProvider, new()
    {
        private readonly StaticWeightedRandomizer<Expression<Func<string>>> NameFormatterChooser = new StaticWeightedRandomizer<Expression<Func<string>>>();

        [SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Local", Justification="Emphasise the immutability of this to the consumer")]
        public CompositeNameGenerator2(ReadOnlyCollection<ProbabilisticNameFormatter> nameFormatters)
            : this(new NameProviderFactory<TEnum, TNameProvider>())
        {
            foreach (var item in nameFormatters) NameFormatterChooser.Add(item.NameFormatter, item.Chance);

            foreach (var item in GetValues<TEnum>())
            {
                Enumerators.Add(item, ProviderFactory.ProviderForNameType(item).Names.GetEnumerator());
            }
        }

        private Dictionary<TEnum, IEnumerator<string>> Enumerators = new Dictionary<TEnum, IEnumerator<string>>(GetValues<TEnum>().Count());

        protected readonly NameProviderFactory<TEnum, TNameProvider> ProviderFactory;

        private CompositeNameGenerator2(NameProviderFactory<TEnum, TNameProvider> nameProviderFactory): this()
        {
            ProviderFactory = nameProviderFactory;
        }

        private CompositeNameGenerator2()
        {
            this.enumModifier = new EnumModifier(this);
        }

        private readonly EnumModifier enumModifier;

        public string NextName()
        {
            return
                ((Expression<Func<string>>)this.enumModifier.Modify(NameFormatterChooser.NextWithReplacement()))
                    .Compile()();
        }

        public class EnumModifier: ExpressionVisitor
        {
            private CompositeNameGenerator2<TEnum, TNameProvider> parent;

            public EnumModifier(CompositeNameGenerator2<TEnum, TNameProvider> parent) { this.parent = parent; }

            public Expression Modify(Expression expression)
            {
                return Visit(expression);
            }

            protected override Expression VisitUnary(UnaryExpression b)
            {
                if (b.Operand.Type == typeof(TEnum))
                {
                    if (b.Operand is ConstantExpression)
                    {
                        var bOperand = (ConstantExpression)b.Operand;
                        var enumerator = parent.Enumerators[(TEnum)bOperand.Value];
                        if (enumerator.MoveNext())
                        {
                            return b.Update(Expression.Constant(enumerator.Current, typeof(string)));
                        }
                        else throw new NoMoreNamesException();
                    }
                    else Debug.Assert(false);

                }

                return base.VisitUnary(b);
            }
        }

    }

    public class NoMoreNamesException : Exception
    {
    }
}
