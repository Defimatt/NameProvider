namespace NameProvider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using Weighted_Randomizer;
    using static System.Diagnostics.Debug;
    using static System.Linq.Expressions.Expression;
    using NameFormatter = System.Linq.Expressions.Expression<System.Func<string>>;
    using static EnumHelpers;
    using static ProbabilisticNameFormatter;

    public class CompositeNameGenerator<TEnum, TNameProvider>
        where TEnum : struct, IFormattable, IConvertible, IComparable
        where TNameProvider : NameProvider, new()
    {
        protected readonly Dictionary<TEnum, IEnumerator<string>> Enumerators =
            new Dictionary<TEnum, IEnumerator<string>>(GetValues<TEnum>().Count());

        protected readonly EnumModifier Modifier;

        protected readonly StaticWeightedRandomizer<NameFormatter> NameFormatterChooser = new StaticWeightedRandomizer<NameFormatter>();

        protected readonly NameProviderFactory<TEnum, TNameProvider> ProviderFactory;

        [SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Local",
            Justification = "Emphasise the immutability of this to the consumer")]
        public CompositeNameGenerator(ReadOnlyCollection<ProbabilisticNameFormatter> nameFormatters)
            : this(new NameProviderFactory<TEnum, TNameProvider>())
        {
            if (nameFormatters.Any(_ => _ is EqualChanceProbabilisticNameFormatter)
                && nameFormatters.Count(_ => _ is EqualChanceProbabilisticNameFormatter) < nameFormatters.Count)
                throw new ArgumentException(
                    "Name formatters must be all equal chance or all explicit chance; mixing is not permitted",
                    nameof(nameFormatters));

            foreach (var item in nameFormatters) NameFormatterChooser.Add(item.NameFormatter, item.Chance);

            var enumLister = new EnumLister();
            var enumValues = NameFormatterChooser.SelectMany(_ => enumLister.NameFormatterEnumValues(_)).Distinct().ToList();
            var unsupportedEnumValues = enumValues.Except(ProviderFactory.SupportedNameTypes()).ToList();

            if (unsupportedEnumValues.Any())
                throw new NotSupportedException(
                    $@"Name provider factory {ProviderFactory.GetType()} does not support {typeof(TEnum)} value(s) {
                        (string.Join(", ", unsupportedEnumValues))}");

            foreach (var item in enumValues) Enumerators.Add(item, ProviderFactory.NamesForNameType(item).GetEnumerator());
        }

        private CompositeNameGenerator(NameProviderFactory<TEnum, TNameProvider> nameProviderFactory) : this()
        {
            ProviderFactory = nameProviderFactory;
        }

        private CompositeNameGenerator() { Modifier = new EnumModifier(this); }

        public virtual string NextName()
        {
            return ((NameFormatter)Modifier.Modify(NameFormatterChooser.NextWithReplacement())).Compile()();
        }

        public virtual IEnumerable<string> Names()
        {
            while (true)
            {
                string nextName;

                try {
                    nextName = NextName();
                }
                catch (NoMoreNamesException) {
                    nextName = null;
                }

                if (nextName == null) yield break;
                yield return nextName;
            }
        }

        protected class EnumLister : ExpressionVisitor
        {
            protected readonly HashSet<TEnum> NameFormatterValues = new HashSet<TEnum>();

            public virtual IEnumerable<TEnum> NameFormatterEnumValues(Expression expression)
            {
                Visit(expression);
                return NameFormatterValues;
            }

            protected override Expression VisitUnary(UnaryExpression expression)
            {
                if (expression.Operand.Type != typeof(TEnum)) return base.VisitUnary(expression);

                var operand = expression.Operand as ConstantExpression;
                if (operand != null) NameFormatterValues.Add((TEnum)operand.Value);
                else Assert(
                        expression.Operand.Type == typeof(TEnum) && expression.Operand.GetType() != typeof(ConstantExpression),
                        "expression.Operand.Type == typeof(TEnum) && expression.Operand.GetType() != typeof(ConstantExpression)",
                        $"expression: {expression}, operand: {expression.Operand}");

                return base.VisitUnary(expression);
            }
        }

        protected class EnumModifier : ExpressionVisitor
        {
            protected readonly CompositeNameGenerator<TEnum, TNameProvider> Parent;

            public EnumModifier(CompositeNameGenerator<TEnum, TNameProvider> parent) { Parent = parent; }

            public virtual Expression Modify(Expression expression) { return Visit(expression); }

            protected override Expression VisitUnary(UnaryExpression expression)
            {
                if (expression.Operand.Type != typeof(TEnum)) return base.VisitUnary(expression);

                var operand = expression.Operand as ConstantExpression;
                if (operand != null)
                {
                    var enumerator = Parent.Enumerators[(TEnum)operand.Value];
                    if (enumerator.MoveNext()) return expression.Update(Constant(enumerator.Current, enumerator.Current.GetType()));

                    throw new NoMoreNamesException($"Enumerator for {typeof(TEnum)} value {operand.Value} exhausted");
                }

                Assert(
                    expression.Operand.Type == typeof(TEnum) && expression.Operand.GetType() != typeof(ConstantExpression),
                    "expression.Operand.Type == typeof(TEnum) && expression.Operand.GetType() != typeof(ConstantExpression)",
                    $"expression: {expression}, operand: {expression.Operand}");

                return base.VisitUnary(expression);
            }
        }
    }
}