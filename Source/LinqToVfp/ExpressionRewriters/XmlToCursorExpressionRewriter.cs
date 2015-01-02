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
using IQToolkit.Data.Common;
using VfpClient.Utils;

namespace LinqToVfp.ExpressionRewriters {
    internal class XmlToCursorExpressionRewriter : VfpExpressionVisitor {
        private readonly VfpLanguage _language;

        private XmlToCursorExpressionRewriter(VfpLanguage language) {
            _language = language;
        }

        internal static Expression Rewrite(Expression expression) {
            return new XmlToCursorExpressionRewriter(VfpLanguage.Default).Visit(expression);
        }

        protected override Expression VisitIn(InExpression expression) {
            if (!ShouldRewrite(expression)) {
                return base.VisitIn(expression);
            }

            Array array = expression.Values.OfType<ConstantExpression>().Select(item => item.Value).Distinct().ToArray();

            var vfpDataXml = new ArrayXmlToCursor(array);
            var tableAlias = new TableAlias();
            var columnType = _language.TypeSystem.GetColumnType(vfpDataXml.ItemType);
            var columnExpression = new ColumnExpression(vfpDataXml.ItemType, columnType, tableAlias, ArrayXmlToCursor.ColumnName);

            var columns = new List<ColumnDeclaration> {
                new ColumnDeclaration(string.Empty, columnExpression, columnType)
            };

            var xml = Expression.Constant(vfpDataXml.Xml);
            var cursorName = Expression.Constant("curTemp_" + DateTime.Now.ToString("ddHHssmm"));
            var check = Expression.GreaterThan(new XmlToCursorExpression(xml, cursorName), Expression.Constant(0));
            var from = Expression.Condition(check, cursorName, Expression.Constant(string.Empty));
            var select = new SelectExpression(tableAlias, columns, from, null);

            return new InExpression(expression.Expression, select);
        }

        private static bool ShouldRewrite(InExpression @in) {
            if (@in.Values == null || @in.Values.Count == 0 || @in.Values[0].NodeType != ExpressionType.Constant) {
                return false;
            }

            const int MINLENGTH = 250;

            var values = new StringBuilder();

            foreach (var value in @in.Values.Where(value => value != null).Distinct()) {
                values.Append(value);
                values.Append(",");

                if (values.Length > MINLENGTH) {
                    return true;
                }
            }

            return false;
        }
    }
}