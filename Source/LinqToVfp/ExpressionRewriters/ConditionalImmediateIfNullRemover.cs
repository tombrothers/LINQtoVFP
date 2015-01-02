using System.Linq.Expressions;
using IQToolkit.Data.Common;

namespace LinqToVfp.ExpressionRewriters {
    internal class ConditionalImmediateIfNullRemover : VfpExpressionVisitor {
        internal static Expression Remove(Expression expression) {
            return new ConditionalImmediateIfNullRemover().Visit(expression);
        }

        protected override Expression VisitConditional(ConditionalExpression expression) {
            if (expression.Test.NodeType == ExpressionType.Equal && expression.IfTrue.NodeType == ExpressionType.Constant && ((ConstantExpression)expression.IfTrue).Value == null) {
                var methodCallExpression = expression.IfFalse as MethodCallExpression;

                if (methodCallExpression != null) {
                    if (methodCallExpression.Object.NodeType == ExpressionType.Conditional) {
                        return Visit(base.VisitConditional(expression));
                    }

                    var be = (BinaryExpression)expression.Test;
                    var columnExpression = be.Left as ColumnExpression;

                    return columnExpression == null ? expression.IfFalse : Visit(expression.IfFalse);

                }
            }

            return base.VisitConditional(expression);
        }
    }
}