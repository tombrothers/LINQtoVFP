/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
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
        private class OuterParameterizer : VfpExpressionVisitor {
            private int parameterCounter;
            private TableAlias outerAlias;
            private Dictionary<ColumnExpression, NamedValueExpression> map = new Dictionary<ColumnExpression, NamedValueExpression>();

            internal static Expression Parameterize(TableAlias outerAlias, Expression expr, int parameterCounter) {
                OuterParameterizer op = new OuterParameterizer();
                op.outerAlias = outerAlias;
                op.parameterCounter = parameterCounter;
                return op.Visit(expr);
            }

            protected override Expression VisitProjection(ProjectionExpression proj) {
                SelectExpression select = (SelectExpression)this.Visit(proj.Select);
                return this.UpdateProjection(proj, select, proj.Projector, proj.Aggregator);
            }

            protected override Expression VisitColumn(ColumnExpression column) {
                if (column.Alias == this.outerAlias) {
                    NamedValueExpression nv;
                    if (!this.map.TryGetValue(column, out nv)) {
                        string name = "@__Param__" + (this.parameterCounter++) + "__";
                        nv = new NamedValueExpression(name, column.QueryType, column);
                        this.map.Add(column, nv);
                    }

                    return nv;
                }

                return column;
            }
        }
    }
}
