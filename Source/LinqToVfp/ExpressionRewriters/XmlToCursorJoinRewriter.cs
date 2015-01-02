using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using IQToolkit.Data.Common;

namespace LinqToVfp.ExpressionRewriters {
    internal class XmlToCursorJoinRewriter : VfpExpressionVisitor {
        private XmlToCursorJoinRewriter() {
        }

        internal static Expression Rewrite(Expression expression) {
            return new XmlToCursorJoinRewriter().Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select) {
            select = (SelectExpression)base.VisitSelect(select);

            var inExpressions = InExpressionGatherer.Gather(select.Where);

            if (!inExpressions.Any()) {
                return select;
            }

            var newSelect = AddInnerJoins(select, inExpressions);

            return InExpressionRemover.Remove(newSelect, inExpressions);
        }

        private static SelectExpression AddInnerJoins(SelectExpression select, IEnumerable<InExpression> inExpressions) {
            foreach (var inExpression in inExpressions) {
                var joinExpression = new JoinExpression(JoinType.InnerJoin,
                                                        select.From,
                                                        inExpression.Select,
                                                        Expression.MakeBinary(ExpressionType.Equal, inExpression.Expression, inExpression.Select.Columns[0].Expression));

                select = select.SetFrom(joinExpression);
            }

            return select;
        }

        private class InExpressionRemover : VfpExpressionVisitor {
            private readonly IEnumerable<string> _inExpressions;

            public static Expression Remove(Expression expression, ReadOnlyCollection<InExpression> inExpressions) {
                var visitor = new InExpressionRemover(inExpressions);

                return visitor.Visit(expression);
            }

            private InExpressionRemover(ReadOnlyCollection<InExpression> inExpressions) {
                _inExpressions = inExpressions.Select(VfpFormatter.Format);
            }

            protected override Expression VisitSelect(SelectExpression select) {
                select = (SelectExpression)base.VisitSelect(select);

                return IsValidInExpression(select.Where as InExpression) ? select.SetWhere(null) : select;
            }

            protected override Expression VisitBinary(BinaryExpression binaryExpression) {
                binaryExpression = (BinaryExpression)base.VisitBinary(binaryExpression);

                if (binaryExpression.NodeType == ExpressionType.And || binaryExpression.NodeType == ExpressionType.AndAlso) {
                    if (IsValidInExpression(binaryExpression.Left as InExpression)) {
                        return binaryExpression.Right;
                    }

                    if (IsValidInExpression(binaryExpression.Right as InExpression)) {
                        return binaryExpression.Left;
                    }
                }

                return binaryExpression;
            }

            private bool IsValidInExpression(InExpression expression) {
                return expression != null && _inExpressions.Contains(VfpFormatter.Format(expression));
            }
        }

        private class InExpressionGatherer : VfpExpressionVisitor {
            private readonly IList<InExpression> _expressions = new List<InExpression>();
            private bool _canRewrite = true;

            public static ReadOnlyCollection<InExpression> Gather(Expression expression) {
                var gatherer = new InExpressionGatherer();

                if (expression == null) {
                    return gatherer._expressions.ToReadOnly();
                }

                gatherer.Visit(expression);

                if (!gatherer._canRewrite) {
                    gatherer._expressions.Clear();
                }

                return gatherer._expressions.ToReadOnly();
            }

            protected override Expression VisitBinary(BinaryExpression binaryExpression) {
                binaryExpression = (BinaryExpression)base.VisitBinary(binaryExpression);

                if (binaryExpression.NodeType == ExpressionType.Or || binaryExpression.NodeType == ExpressionType.OrElse) {
                    _canRewrite = false;
                }

                return binaryExpression;
            }

            protected override Expression VisitUnary(UnaryExpression unaryExpression) {
                unaryExpression = (UnaryExpression)base.VisitUnary(unaryExpression);

                if (unaryExpression.NodeType == ExpressionType.Not && unaryExpression.Operand is InExpression) {
                    _canRewrite = false;
                }

                return unaryExpression;
            }

            protected override Expression VisitIn(InExpression expression) {
                if (HasNoValues(expression) && HasXmlToCursorExpression(expression.Select)) {
                    _expressions.Add(expression);
                }

                return expression;
            }

            private static bool HasNoValues(InExpression expression) {
                return expression.Values == null;
            }

            private static bool HasXmlToCursorExpression(Expression expression) {
                return XmlToCursorExpressionGatherer.Gather(expression).Any();
            }
        }

        private class XmlToCursorExpressionGatherer : VfpExpressionVisitor {
            private readonly IList<XmlToCursorExpression> _expressions = new List<XmlToCursorExpression>();

            public static IEnumerable<XmlToCursorExpression> Gather(Expression expression) {
                var gatherer = new XmlToCursorExpressionGatherer();

                if (expression == null) {
                    return gatherer._expressions.ToReadOnly();
                }

                gatherer.Visit(expression);

                return gatherer._expressions.ToReadOnly();
            }

            protected override Expression VisitXmlToCursor(XmlToCursorExpression expression) {
                _expressions.Add(expression);

                return base.VisitXmlToCursor(expression);
            }
        }
    }
}