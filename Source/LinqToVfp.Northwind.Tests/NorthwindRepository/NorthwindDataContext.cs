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
using System.IO;
using IQToolkitContrib;
using LinqToVfp;

namespace LinqToVfp.Northwind.Tests.NorthwindRepository {
    public partial class NorthwindDataContext : IQToolkitContrib.DataContext {
        public NorthwindDataContext(string connectionString, TestContextWriter testContextWriter)
            : this(CreateRepository(connectionString), testContextWriter) {
        }

        public NorthwindDataContext(IRepository repository, TestContextWriter testContextWriter)
            : this(repository) {
            
            if (this.Provider != null) {
                this.Provider.Log = testContextWriter;
            }            
        }

        public NorthwindDataContext(VfpQueryProvider provider) : this(new DbEntityRepository(provider)) {
        }
    }
}
