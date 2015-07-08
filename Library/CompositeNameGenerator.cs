using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameProvider
{
    using System.CodeDom;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Security.Policy;

    public class CompositeNameGenerator<TEnum>
        where TEnum : struct, IFormattable, IConvertible, IComparable
    {
        public class NameSegment
        {
            public TEnum NameSegmentType { get; protected set; }

            public NameSegment(TEnum nameSegmentType)
            {
                NameSegmentType = nameSegmentType;
            }
        }

        public Func<List<NameSegment>, string> Converter { get; protected set; } = null;

        public delegate string FormatString(string format, params object[] args);

        public Expression<Func<List<NameSegment>, FormatString>> ConversionExpression;


    }

    public static class CompositeNameGeneratorExtensions
    {
        public delegate string FormatString(string format, params object[] args);

        public static void SetConverter<TEnum>(
            this CompositeNameGenerator<TEnum> @this,
            Expression<Func<string>> conversionExpression)
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            var body = conversionExpression.Body;
            var bodyExpr = (MethodCallExpression)body;
            var convert = (UnaryExpression)bodyExpr.Arguments[1];
            convert = convert.Update(Expression.Constant("Hello"));
            var arguments = bodyExpr.Arguments.ToList();
            arguments.RemoveAt(1);
            arguments.Insert(1, convert);
            var d = new ReadOnlyCollection<Expression>(arguments);
            //conversionExpression = conversionExpression.Update(bodyExpr.Update(bodyExpr.Object, d), null);

            //var uu = (Expression<Func<string>>)(new EnumModifier<TEnum>(conversionExpression, @this).Modify(conversionExpression));
            //var u2 = uu.Compile();


        }

        public class EnumModifier<TEnum> : ExpressionVisitor where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            public Expression Modify(Expression expression)
            {
                return Visit(expression);
            }

            protected override Expression VisitUnary(UnaryExpression b)
            {
                if (b.Operand.Type == typeof(TEnum))
                {
                    return b.Update(Expression.Constant("Hello", typeof(string)));
                }

                return base.VisitUnary(b);
            }
        }

        
    }
}
