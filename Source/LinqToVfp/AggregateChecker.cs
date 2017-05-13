/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Linq.Expressions;
using IQToolkit.Data.Common;

namespace LinqToVfp {
    class AggregateChecker : VfpExpressionVisitor {
        bool hasAggregate = false;
        private AggregateChecker() {
        }

        internal static bool HasAggregates(SelectExpression expression) {
            AggregateChecker checker = new AggregateChecker();
            checker.Visit(expression);
            return checker.hasAggregate;
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate) {
            this.hasAggregate = true;
            return aggregate;
        }

        protected override Expression VisitSelect(SelectExpression select) {
            // only consider aggregates in these locations
            this.Visit(select.Where);
            this.VisitOrderBy(select.OrderBy);
            this.VisitColumnDeclarations(select.Columns);
            return select;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery) {
            // don't count aggregates in subqueries
            return subquery;
        }
    }
}
