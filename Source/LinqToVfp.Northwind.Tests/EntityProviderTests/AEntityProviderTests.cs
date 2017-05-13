/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToVfp.Northwind.Tests.NorthwindEntityProvider;
using System.IO;

namespace LinqToVfp.Northwind.Tests.EntityProviderTests {
    [TestClass]
    public abstract class AEntityProviderTests : TestBase {
        #region Northwind

        private NorthwindDataContext northwind;

        protected NorthwindDataContext Northwind {
            get {
                return this.northwind;
            }
        }

        #endregion

        [TestInitialize]
        public virtual void TestInitialize() {
            string connectionString = @"Provider=VFPOLEDB.1;Data Source=" + Path.GetFullPath("Northwind.dbc") + ";Exclusive=false";
            this.northwind = new LinqToVfp.Northwind.Tests.NorthwindEntityProvider.NorthwindDataContext(connectionString);
            //this.northwind.Provider.Log = new TestContextWriter(this.TestContext);
            this.northwind.Provider.AutoRightTrimStrings = true;
        }
    }
}
