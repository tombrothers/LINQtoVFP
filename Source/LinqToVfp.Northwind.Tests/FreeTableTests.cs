/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Linq;
using IQToolkit;
using IQToolkit.Data.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VfpClient;

namespace LinqToVfp.Northwind.Tests {
    [TestClass]
    public class FreeTableTests : TestBase {
        [TestMethod]
        [ExpectedException(typeof(VfpException))]
        public void DecimalReadExceptionTest() {
            var connectionString = this.TestContext.TestDeploymentDir + @"\Decimal";

            using (VfpQueryProvider provider = VfpQueryProvider.Create(connectionString, null)) {
                var result = provider.GetTable<DecimalTable>("DecimalTable")
                                     .OrderBy(x => x.nValue)
                                     .Select(x => x.nValue)
                                     .First();

                Assert.AreEqual(7654321.12345M, result);
            }
        }

        [TestMethod]
        public void DecimalFixTest() {
            var connectionString = this.TestContext.TestDeploymentDir + @"\Decimal";
            var context = new DecimalDataContext(connectionString);

            var result = context.Decimals
                                .OrderBy(x => x.nValue)
                                .Select(x => x.nValue)
                                .First();

            Assert.AreEqual(7654321.12345M, result);

        }
    }

    public class DecimalTable {
        public int nPk { get; set; }
        public decimal nValue { get; set; }
    }

    public partial class DecimalDataContext {
        public VfpQueryProvider Provider { get; private set; }

        public DecimalDataContext(string connectionString) {
            var mappingId = typeof(DecimalDataContextAttributes).FullName;

            this.Provider = VfpQueryProvider.Create(connectionString, mappingId);
        }

        public virtual IEntityTable<DecimalTable> Decimals {
            get { return this.Provider.GetTable<DecimalTable>("Decimals"); }
        }
    }

    public partial class DecimalDataContextAttributes : DecimalDataContext {
        public DecimalDataContextAttributes(string connectionString)
            : base(connectionString) {
        }

        [Table(Name = "DecimalTable")]
        [Column(Member = "nPk", IsPrimaryKey = true, IsGenerated = true)]
        [Column(Member = "nValue", DbType = "N(14, 7)")]
        public override IEntityTable<DecimalTable> Decimals {
            get { return base.Decimals; }
        }
    }
}