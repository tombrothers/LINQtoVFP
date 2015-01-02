/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using IQToolkit;
using IQToolkitContrib;

namespace LinqToVfp {
    public class VfpEntitySession : DbEntitySessionBase, IVfpEntitySession {
        VfpQueryProvider IVfpEntitySession.Provider {
            get { return ((IEntitySession)this).Provider as VfpQueryProvider; }
        }

        public VfpEntitySession(VfpQueryProvider provider)
            : base(provider) {
        }
    }
}