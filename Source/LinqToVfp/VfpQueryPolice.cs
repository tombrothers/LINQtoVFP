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
    public class VfpQueryPolice : QueryPolice {
        public VfpQueryPolice(QueryPolicy policy, QueryTranslator translator) :
            base(policy, translator) {
        }

        public override Expression BuildExecutionPlan(Expression query, Expression provider) {
            return VfpExecutionBuilder.Build(this.Translator.Linguist, this.Policy, query, provider);
        }
    }
}
