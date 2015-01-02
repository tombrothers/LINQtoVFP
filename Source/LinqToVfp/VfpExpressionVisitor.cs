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
    internal class VfpExpressionVisitor : DbExpressionVisitor {
        protected override Expression Visit(Expression expression) {
            if (expression == null) {
                return null;
            }

            if (expression.NodeType.IsVfpExpression()) {
                switch ((VfpExpressionType)expression.NodeType) {
                    case VfpExpressionType.XmlToCursor:
                        return VisitXmlToCursor((XmlToCursorExpression)expression);
                }
            }

            return base.Visit(expression);
        }

        protected virtual Expression VisitXmlToCursor(XmlToCursorExpression expression) {
            return new XmlToCursorExpression(Visit(expression.Xml), Visit(expression.CursorName));
        }
    }
}