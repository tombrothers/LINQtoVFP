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

namespace LinqToVfp.Northwind.Tests.EntityProviderTests {
    [TestClass]
    public class Translation_DateTimeTests : ATranslationTests {
        [TestMethod]
        public void Translation_DateTimeTest_ConstructYMD() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate == new DateTime(o.OrderDate.Year, 1, 1)));
        }

        [TestMethod]
        public void Translation_DateTimeTest_ConstructYMDHMS() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate == new DateTime(o.OrderDate.Year, 1, 1, 10, 25, 55)));
        }

        [TestMethod]
        public void Translation_DateTimeTest_Day() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.Day == 5));
        }

        [TestMethod]
        public void Translation_DateTimeTest_Month() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.Month == 12));
        }

        [TestMethod]
        public void Translation_DateTimeTest_Year() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.Year == 1997));
        }

        [TestMethod]
        public void Translation_DateTimeTest_Hour() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.Hour == 6));
        }

        [TestMethod]
        public void Translation_DateTimeTest_Minute() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.Minute == 32));
        }

        [TestMethod]
        public void Translation_DateTimeTest_Second() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.Second == 47));
        }

        // No milliseconds in vfp
        //public void TestDateTimeMillisecond() {
        //    TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.Millisecond == 200));
        //}

        [TestMethod]
        public void Translation_DateTimeTest_DayOfYear() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.DayOfYear == 360));
        }

        [TestMethod]
        public void Translation_DateTimeTest_DayOfWeek() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderDate.DayOfWeek == DayOfWeek.Friday));
        }
    }
}
