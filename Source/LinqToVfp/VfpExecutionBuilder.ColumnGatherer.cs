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

namespace LinqToVfp {
    internal partial class VfpExecutionBuilder {
        internal class ColumnGatherer : VfpExpressionVisitor {
            private Dictionary<string, ColumnExpression> columns = new Dictionary<string, ColumnExpression>();

            internal static IEnumerable<ColumnExpression> Gather(Expression expression) {
                var gatherer = new ColumnGatherer();
                gatherer.Visit(expression);
                return gatherer.columns.Values;
            }

            protected override Expression VisitColumn(ColumnExpression column) {
                if (!this.columns.ContainsKey(column.Name)) {
                    this.columns.Add(column.Name, column);
                }

                return column;
            }
        }
    }
}
