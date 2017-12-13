using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVfp.ExpressionRewriters
{
    internal class OneBasedIndexRewriter : VfpExpressionVisitor
    {
        private static readonly IEnumerable<string> MethodNames = new[]
        {
            "Substring",
            "Remove"
        };

        public static Expression Rewrite(Expression expression) => new OneBasedIndexRewriter().Visit(expression);

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if(methodCallExpression.Method.DeclaringType == typeof(string) &&
               MethodNames.Contains(methodCallExpression.Method.Name))
            {
                return Expression.Call(
                    methodCallExpression.Object,
                    methodCallExpression.Method,
                    AddOneToTheFirstArgument(methodCallExpression.Arguments));
            }

            return base.VisitMethodCall(methodCallExpression);
        }

        private static IEnumerable<Expression> AddOneToTheFirstArgument(IEnumerable<Expression> arguments) =>
            arguments.Select((expression, index) =>
            {
                if(index != 0 || !(expression is ConstantExpression constExpression))
                {
                    return expression;
                }

                var valueType = constExpression.Value.GetType().ToString();

                switch(valueType)
                {
                    case "System.Int16":
                        return Expression.Constant((short) constExpression.Value + 1);
                    case "System.UInt16":
                        return Expression.Constant((ushort) constExpression.Value + 1);
                    case "System.Int32":
                        return Expression.Constant((int) constExpression.Value + 1);
                    case "System.UInt32":
                        return Expression.Constant((uint) constExpression.Value + 1);
                    case "System.Int64":
                        return Expression.Constant((long) constExpression.Value + 1);
                    case "System.UInt64":
                        return Expression.Constant((ulong) constExpression.Value + 1);
                    default:
                        return expression;
                }
            });
    }
}