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
    internal partial class VfpExecutionBuilder {
        private class ProjectionFinder : VfpExpressionVisitor {
            private ProjectionExpression found = null;

            internal static ProjectionExpression FindProjection(Expression expression) {
                var finder = new ProjectionFinder();
                finder.Visit(expression);
                return finder.found;
            }

            protected override Expression VisitProjection(ProjectionExpression proj) {
                this.found = proj;
                return proj;
            }
        }
    }
}
