/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using IQToolkit.Data;

namespace LinqToVfp {
    public class VfpDbQueryType : DbQueryType {
        public VfpDbQueryType(DbQueryType queryType)
            : base(queryType.SqlDbType, queryType.NotNull, queryType.Length, queryType.Precision, queryType.Scale) {
        }
    }
}