/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using IQToolkit.Data.Common;

namespace LinqToVfp {
    public class VfpQueryPolicy : QueryPolicy {
        public static new readonly VfpQueryPolicy Default = new VfpQueryPolicy();

        public override QueryPolice CreatePolice(QueryTranslator translator) {
            return new VfpQueryPolice(this, translator);
        }
    }
}
