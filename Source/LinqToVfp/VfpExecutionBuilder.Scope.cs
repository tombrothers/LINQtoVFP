/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IQToolkit.Data.Common;

namespace LinqToVfp {
    internal partial class VfpExecutionBuilder {
        private class Scope {
            private Scope outer;
            private ParameterExpression fieldReader;
            internal TableAlias Alias { get; private set; }
            private Dictionary<string, int> nameMap;

            internal Scope(Scope outer, ParameterExpression fieldReader, TableAlias alias, IEnumerable<ColumnDeclaration> columns) {
                this.outer = outer;
                this.fieldReader = fieldReader;
                this.Alias = alias;
                this.nameMap = columns.Select((c, i) => new { c, i }).ToDictionary(x => x.c.Name, x => x.i);
            }

            internal bool TryGetValue(ColumnExpression column, out ParameterExpression fieldReader, out int ordinal) {
                for (Scope s = this; s != null; s = s.outer) {
                    if (column.Alias == s.Alias && this.nameMap.TryGetValue(column.Name, out ordinal)) {
                        fieldReader = this.fieldReader;
                        return true;
                    }
                }

                fieldReader = null;
                ordinal = 0;
                return false;
            }
        }
    }
}
