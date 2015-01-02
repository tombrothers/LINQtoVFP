/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
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
        private class EntityFinder : VfpExpressionVisitor {
            private MappingEntity entity;

            public static MappingEntity Find(Expression expression) {
                var finder = new EntityFinder();
                finder.Visit(expression);
                return finder.entity;
            }

            protected override Expression Visit(Expression exp) {
                if (this.entity == null) {
                    return base.Visit(exp);
                }

                return exp;
            }

            protected override Expression VisitEntity(EntityExpression entity) {
                if (this.entity == null) {
                    this.entity = entity.Entity;
                }

                return entity;
            }

            protected override NewExpression VisitNew(NewExpression nex) {
                return nex;
            }

            protected override Expression VisitMemberInit(MemberInitExpression init) {
                return init;
            }
        }
    }
}
