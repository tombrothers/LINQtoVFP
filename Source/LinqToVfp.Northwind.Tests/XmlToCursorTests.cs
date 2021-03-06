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
using System.Linq;
using LinqToVfp.Northwind.Tests.NorthwindRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VfpClient.Utils;

namespace LinqToVfp.Northwind.Tests {
    [TestClass]
    public class XmlToCursorTests : TestBase {
        protected NorthwindDataContext Northwind { get; private set; }
        
        [TestMethod]
        public void XmlToCursorTests_IntStringArray_Test() {
            string[] customerIds = { "1", "2" };
            var xmlToCursor = new ArrayXmlToCursor(customerIds);
            var list = Northwind.List<Customer>().Where(customer => customerIds.Contains(customer.CustomerID)).ToList();

            Assert.AreEqual(typeof(string[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(string), xmlToCursor.ItemType);
            Assert.AreEqual(0, list.Count);
        }
        
        [TestMethod]
        public void XmlToCursorTests_StringArray_Test() {
            string[] customerIds = { "VINET", "WARTH", null };
            var xmlToCursor = new ArrayXmlToCursor(customerIds);
            var list = Northwind.List<Customer>().Where(customer => customerIds.Contains(customer.CustomerID)).ToList();

            Assert.AreEqual(typeof(string[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(string), xmlToCursor.ItemType);
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void XmlToCursorTests_DoubleArray_Test() {
            double[] orderIds = { 10248, 1, 10249 };
            var xmlToCursor = new ArrayXmlToCursor(orderIds);
            var list = Northwind.List<Order>().Where(order => orderIds.Contains(order.OrderID)).ToList();

            Assert.AreEqual(typeof(double[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(double), xmlToCursor.ItemType);
            Assert.AreEqual(2, list.Count);

        }

        [TestMethod]
        public void XmlToCursorTests_DecimalArray_Test() {
            decimal[] orderIds = { 10248, 1, 10249 };
            var xmlToCursor = new ArrayXmlToCursor(orderIds);
            var list = Northwind.List<Order>().Where(order => orderIds.Contains(order.OrderID)).ToList();

            Assert.AreEqual(typeof(decimal[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(decimal), xmlToCursor.ItemType);
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void XmlToCursorTests_FloatArray_Test() {
            float[] orderIds = { 10248, 1, 10249 };
            var xmlToCursor = new ArrayXmlToCursor(orderIds);
            var list = Northwind.List<Order>().Where(order => orderIds.Contains(order.OrderID)).ToList();

            Assert.AreEqual(typeof(float[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(float), xmlToCursor.ItemType);
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void XmlToCursorTests_LongArray_Test() {
            long[] orderIds = { 10248, 1, 10249 };
            var xmlToCursor = new ArrayXmlToCursor(orderIds);
            var list = Northwind.List<Order>().Where(order => orderIds.Contains(order.OrderID)).ToList();

            Assert.AreEqual(typeof(long[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(long), xmlToCursor.ItemType);
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void XmlToCursorTests_IntArray_Test() {
            int[] orderIds = { 10248, 1, 10249 };
            var xmlToCursor = new ArrayXmlToCursor(orderIds);
            var list = Northwind.List<Order>().Where(order => orderIds.Contains(order.OrderID)).ToList();

            Assert.AreEqual(typeof(int[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(int), xmlToCursor.ItemType);
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void XmlToCursorTests_BooleanIntArray_Test() {
            int[] orderIds = { 0, 1 };
            var xmlToCursor = new ArrayXmlToCursor(orderIds);
            var list = Northwind.List<Order>().Where(order => orderIds.Contains(order.OrderID)).ToList();
            
            Assert.AreEqual(typeof(int[]), xmlToCursor.ArrayType);
            Assert.AreEqual(typeof(int), xmlToCursor.ItemType);
            Assert.AreEqual(0, list.Count);
        }
        
        [TestInitialize]
        public void TestInitialize() {
            var connectionString = $@"Provider=VFPOLEDB.1;Data Source={Path.Combine(TestContext.TestDeploymentDir, "Northwind.dbc")};Exclusive=false";

            Northwind = new NorthwindDataContext(connectionString) {
                Provider = {
                    Log = new TestContextWriter(TestContext),
                    AutoRightTrimStrings = true
                }
            };
        }
    }
}