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

namespace LinqToVfp {
    internal class XmlToCursorExpression : VfpExpression {
        public Expression Xml { get; private set; }
        public Expression CursorName { get; private set; }

        public XmlToCursorExpression(Expression xml, Expression cursorName)
            : base(VfpExpressionType.XmlToCursor, typeof(int)) {
            this.Xml = xml;
            this.CursorName = cursorName;
        }
    }
}