/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IQToolkit.Data.Common;

namespace LinqToVfp {
    internal class ReferencedColumnGatherer : VfpExpressionVisitor {
        HashSet<ColumnExpression> columns = new HashSet<ColumnExpression>();
        bool first = true;

        public static HashSet<ColumnExpression> Gather(Expression expression) {
            var visitor = new ReferencedColumnGatherer();
            visitor.Visit(expression);
            return visitor.columns;
        }

        protected override Expression VisitColumn(ColumnExpression column) {
            this.columns.Add(column);
            return column;
        }

        protected override Expression VisitSelect(SelectExpression select) {
            if (first) {
                first = false;
                return base.VisitSelect(select);
            }
            return select;
        }
    }
}
