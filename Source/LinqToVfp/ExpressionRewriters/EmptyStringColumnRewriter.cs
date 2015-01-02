/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqToVfp.ExpressionRewriters {
    internal class EmptyStringColumnRewriter : VfpExpressionVisitor {
        private VfpLanguage language;

        private EmptyStringColumnRewriter(VfpLanguage language) {
            this.language = language;
        }

        internal static Expression Rewrite(Expression expression) {
            return new EmptyStringColumnRewriter(VfpLanguage.Default).Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b) {
            return base.VisitBinary(b);
        }
    }
}