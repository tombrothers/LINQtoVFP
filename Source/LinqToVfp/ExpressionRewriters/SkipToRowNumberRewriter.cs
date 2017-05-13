/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Linq;
using System.Linq.Expressions;
using IQToolkit;
using IQToolkit.Data.Common;

namespace LinqToVfp.ExpressionRewriters {
    internal class SkipToRowNumberRewriter : VfpExpressionVisitor {
        private bool _hasRowNumberExpression;
        private readonly VfpLanguage _language;

        private SkipToRowNumberRewriter(VfpLanguage language) {
            _language = language;
        }

        public static Expression Rewrite(Expression expression, out bool hasRowNumberExpression) {
            var rewriter = new SkipToRowNumberRewriter(VfpLanguage.Default);

            expression = rewriter.Visit(expression);
            hasRowNumberExpression = rewriter._hasRowNumberExpression;

            return expression;
        }

        protected override Expression VisitSelect(SelectExpression select) {
            select = (SelectExpression)base.VisitSelect(select);

            if (select.Skip != null) {
                var newSelect = select.SetSkip(null).SetTake(null).AddRedundantSelect(_language, new TableAlias());

                _hasRowNumberExpression = true;

                var colType = _language.TypeSystem.GetColumnType(typeof(int));
                
                newSelect = newSelect.AddColumn(new ColumnDeclaration("rownum", new RowNumberExpression(select.OrderBy), colType));

                // add layer for WHERE clause that references new rownum column
                newSelect = newSelect.AddRedundantSelect(_language, new TableAlias());
                newSelect = newSelect.RemoveColumn(newSelect.Columns.Single(c => c.Name == "rownum"));

                var newAlias = ((SelectExpression)newSelect.From).Alias;
                var rowNumberColumn = new ColumnExpression(typeof(int), colType, newAlias, "rownum");

                Expression where;
                
                if (select.Take != null) {
                    where = new BetweenExpression(rowNumberColumn, Expression.Add(select.Skip, Expression.Constant(1)), Expression.Add(select.Skip, select.Take));
                }
                else {
                    where = rowNumberColumn.GreaterThan(select.Skip);
                }

                if (newSelect.Where != null) {
                    where = newSelect.Where.And(where);
                }

                newSelect = newSelect.SetWhere(where);

                select = newSelect;
            }

            return select;
        }
    }
}