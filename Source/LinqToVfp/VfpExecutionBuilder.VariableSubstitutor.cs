/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Collections.Generic;
using System.Linq.Expressions;
using IQToolkit.Data.Common;

namespace LinqToVfp {
    internal partial class VfpExecutionBuilder {
        private class VariableSubstitutor : VfpExpressionVisitor {
            private Dictionary<string, Expression> map;

            private VariableSubstitutor(Dictionary<string, Expression> map) {
                this.map = map;
            }

            public static Expression Substitute(Dictionary<string, Expression> map, Expression expression) {
                return new VariableSubstitutor(map).Visit(expression);
            }

            protected override Expression VisitVariable(VariableExpression vex) {
                Expression sub;
                if (this.map.TryGetValue(vex.Name, out sub)) {
                    return sub;
                }

                return vex;
            }
        }
    }
}
