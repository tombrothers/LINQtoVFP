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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LinqToVfp {
    /// <summary>
    /// Extended node types for custom expressions
    /// </summary>
    internal enum VfpExpressionType {
        XmlToCursor = 2000, // make sure these don't overlap with ExpressionType
    }

    internal static class DbExpressionTypeExtensions {
        public static bool IsVfpExpression(this ExpressionType et) {
            return ((int)et) >= 2000;
        }
    }
}
