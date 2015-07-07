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

            var uu = (Expression<Func<string>>)(new EnumModifier<TEnum>(conversionExpression, @this).Modify(conversionExpression));
            var u2 = uu.Compile();


        }

        public class EnumModifier<TEnum> : ExpressionVisitor where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            public Expression<Func<string>> OuterExpression { get; set; }
            public CompositeNameGenerator<TEnum> CompositeNameGenerator { get; set; } 

            public EnumModifier(Expression<Func<string>> outerExpression, CompositeNameGenerator<TEnum> compositeNameGenerator)
            {
                OuterExpression = outerExpression;
                CompositeNameGenerator = compositeNameGenerator;
            }

            public Expression Modify(Expression expression)
            {
                return Visit(expression);
            }

            public override Expression Visit(Expression node)
            {
                return base.Visit(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                return base.VisitBinary(node);
            }

            protected override Expression VisitBlock(BlockExpression node)
            {
                return base.VisitBlock(node);
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                return base.VisitConditional(node);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Value is TEnum)
                {
                 // return Expression.Constant("Hello", typeof(string));
                }
                

                return base.VisitConstant(node);
            }

            protected override Expression VisitDebugInfo(DebugInfoExpression node)
            {
                return base.VisitDebugInfo(node);
            }

            protected override Expression VisitDynamic(DynamicExpression node)
            {
                return base.VisitDynamic(node);
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                return base.VisitDefault(node);
            }

            protected override Expression VisitExtension(Expression node)
            {
                return base.VisitExtension(node);
            }

            protected override Expression VisitGoto(GotoExpression node)
            {
                return base.VisitGoto(node);
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                return base.VisitInvocation(node);
            }

            protected override LabelTarget VisitLabelTarget(LabelTarget node)
            {
                return base.VisitLabelTarget(node);
            }

            protected override Expression VisitLabel(LabelExpression node)
            {
                return base.VisitLabel(node);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                //var outerLambda = OuterExpression;

                //if (node == (LambdaExpression)OuterExpression) return base.VisitLambda(node);

                

                return base.VisitLambda(node);
            }

            protected override Expression VisitLoop(LoopExpression node)
            {
                return base.VisitLoop(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                return base.VisitMember(node);
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                return base.VisitIndex(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                //var a = Expression.Lambda(node).Compile();
                //var b = a.DynamicInvoke(node.Arguments);
                return base.VisitMethodCall(node);
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                return base.VisitNewArray(node);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                return base.VisitNew(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(node);
            }

            protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                return base.VisitRuntimeVariables(node);
            }

            protected override SwitchCase VisitSwitchCase(SwitchCase node)
            {
                return base.VisitSwitchCase(node);
            }

            protected override Expression VisitSwitch(SwitchExpression node)
            {
                return base.VisitSwitch(node);
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                return base.VisitCatchBlock(node);
            }

            protected override Expression VisitTry(TryExpression node)
            {
                return base.VisitTry(node);
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                return base.VisitTypeBinary(node);
            }

            protected override Expression VisitUnary(UnaryExpression b)
            {
               // if (b.Operand.Type == typeof(Func<IEnumerable<TEnum>>))
               // {
               //     var compiledLambda = (Func<IEnumerable<TEnum>>)((LambdaExpression)b.Operand).Compile();
               //     var results = compiledLambda();

               //     return Expression.Convert(Expression.Constant("Blah"), typeof(object));
               //     //return Expression.Constant("Blah");
               // }

                if (b.Operand.Type == typeof(TEnum))
                {
                    return b.Update(Expression.Constant("Hello", typeof(string)));
                }

                //if (b.NodeType == ExpressionType.Constant)
                //{
                //    //Expression subExpression = this.Visit(b.Operand);

                //    // Make this binary expression an OrElse operation instead of an AndAlso operation. 
                //    return Expression.Constant("Hello", b.Type);
                //}

                

                return base.VisitUnary(b);
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                return base.VisitMemberInit(node);
            }

            protected override Expression VisitListInit(ListInitExpression node)
            {
                return base.VisitListInit(node);
            }

            protected override ElementInit VisitElementInit(ElementInit node)
            {
                return base.VisitElementInit(node);
            }

            protected override MemberBinding VisitMemberBinding(MemberBinding node)
            {
                return base.VisitMemberBinding(node);
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                return base.VisitMemberAssignment(node);
            }

            protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
            {
                return base.VisitMemberMemberBinding(node);
            }

            protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
            {
                return base.VisitMemberListBinding(node);
            }
        }

        
    }
}
