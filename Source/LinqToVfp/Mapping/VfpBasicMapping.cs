/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using IQToolkit.Data.Common;

namespace LinqToVfp {
    public abstract class VfpBasicMapping : BasicMapping {
        public override QueryMapper CreateMapper(QueryTranslator translator) {
            return new VfpBasicMapper(this, translator);
        }
    }
}
