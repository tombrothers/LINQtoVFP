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
using IQToolkit.Data.Common;

namespace LinqToVfp {
    internal abstract class VfpExpression : DbExpression  {
        protected VfpExpression(VfpExpressionType eType, Type type)
            : base((DbExpressionType)eType, type) {
        }
    }
}
