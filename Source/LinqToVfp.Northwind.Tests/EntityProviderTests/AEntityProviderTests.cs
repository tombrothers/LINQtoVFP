/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */

using System.IO;
using LinqToVfp.Northwind.Tests.NorthwindEntityProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToVfp.Northwind.Tests.EntityProviderTests {
    [TestClass]
    public abstract class AEntityProviderTests : TestBase {
        protected NorthwindDataContext Northwind { get; private set; }

        [TestInitialize]
        public virtual void TestInitialize() {
            var connectionString = $@"Provider=VFPOLEDB.1;Data Source={Path.Combine(TestContext.TestDeploymentDir, "Northwind.dbc")};Exclusive=false";

            Northwind = new NorthwindDataContext(connectionString) {
                Provider = {AutoRightTrimStrings = true}
            };
        }
    }
}