/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Collections.Generic;
using System.Linq.Expressions;
using IQToolkit.Data.Common;
using IQToolkit;

namespace LinqToVfp.ExpressionRewriters {
    /// <summary>
    /// Removes joins expressions that are identical to joins that already exist
    /// </summary>
    internal class RedundantJoinRemover : VfpExpressionVisitor {
        Dictionary<TableAlias, TableAlias> map;

        private RedundantJoinRemover() {
            this.map = new Dictionary<TableAlias, TableAlias>();
        }

        public static Expression Remove(Expression expression) {
            return new RedundantJoinRemover().Visit(expression);
        }

        protected override Expression VisitJoin(JoinExpression join) {
            Expression result = base.VisitJoin(join);
            join = result as JoinExpression;
            if (join != null) {
                AliasedExpression right = join.Right as AliasedExpression;
                if (right != null) {
                    AliasedExpression similarRight = (AliasedExpression)this.FindSimilarRight(join.Left as JoinExpression, join);
                    if (similarRight != null) {
                        this.map.Add(right.Alias, similarRight.Alias);
                        return join.Left;
                    }
                }
            }
            return result;
        }

        private Expression FindSimilarRight(JoinExpression join, JoinExpression compareTo) {
            if (join == null)
                return null;
            if (join.Join == compareTo.Join) {
                if (join.Right.NodeType == compareTo.Right.NodeType
                    && DbExpressionComparer.AreEqual(join.Right, compareTo.Right)) {
                    if (join.Condition == compareTo.Condition)
                        return join.Right;
                    var scope = new ScopedDictionary<TableAlias, TableAlias>(null);
                    scope.Add(((AliasedExpression)join.Right).Alias, ((AliasedExpression)compareTo.Right).Alias);
                    if (DbExpressionComparer.AreEqual(null, scope, join.Condition, compareTo.Condition))
                        return join.Right;
                }
            }
            Expression result = FindSimilarRight(join.Left as JoinExpression, compareTo);
            if (result == null) {
                result = FindSimilarRight(join.Right as JoinExpression, compareTo);
            }
            return result;
        }

        protected override Expression VisitColumn(ColumnExpression column) {
            TableAlias mapped;
            if (this.map.TryGetValue(column.Alias, out mapped)) {
                return new ColumnExpression(column.Type, column.QueryType, mapped, column.Name);
            }
            return column;
        }
    }
}
