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
    public class Execution_DateTimeTests : AEntityProviderTests {
        [TestMethod]
        public void Execution_DateTimeTest_ConstructYMD() {
            var dt = this.Northwind.Customers.Where(c => c.CustomerID == "ALFKI").Max(c => new DateTime((c.CustomerID == "ALFKI") ? 1997 : 1997, 7, 4));
            Assert.AreEqual(1997, dt.Year);
            Assert.AreEqual(7, dt.Month);
            Assert.AreEqual(4, dt.Day);
            Assert.AreEqual(0, dt.Hour);
            Assert.AreEqual(0, dt.Minute);
            Assert.AreEqual(0, dt.Second);
        }

        [TestMethod]
        public void Execution_DateTimeTest_ConstructYMDHMS() {
            var dt = this.Northwind.Customers.Where(c => c.CustomerID == "ALFKI").Max(c => new DateTime((c.CustomerID == "ALFKI") ? 1997 : 1997, 7, 4, 3, 5, 6));
            Assert.AreEqual(1997, dt.Year);
            Assert.AreEqual(7, dt.Month);
            Assert.AreEqual(4, dt.Day);
            Assert.AreEqual(3, dt.Hour);
            Assert.AreEqual(5, dt.Minute);
            Assert.AreEqual(6, dt.Second);
        }

        [TestMethod]
        public void Execution_DateTimeTest_Day() {
            var v = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.Day);
            Assert.AreEqual(25, v);
        }

        [TestMethod]
        public void Execution_DateTimeTest_Month() {
            var v = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.Month);
            Assert.AreEqual(8, v);
        }

        [TestMethod]
        public void Execution_DateTimeTest_Year() {
            var v = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.Year);
            Assert.AreEqual(1997, v);
        }

        [TestMethod]
        public void Execution_DateTimeTest_Hour() {
            var hour = this.Northwind.Customers.Where(c => c.CustomerID == "ALFKI").Max(c => new DateTime((c.CustomerID == "ALFKI") ? 1997 : 1997, 7, 4, 3, 5, 6).Hour);
            Assert.AreEqual(3, hour);
        }

        [TestMethod]
        public void Execution_DateTimeTest_Minute() {
            var minute = this.Northwind.Customers.Where(c => c.CustomerID == "ALFKI").Max(c => new DateTime((c.CustomerID == "ALFKI") ? 1997 : 1997, 7, 4, 3, 5, 6).Minute);
            Assert.AreEqual(5, minute);
        }

        [TestMethod]
        public void Execution_DateTimeTest_Second() {
            var second = this.Northwind.Customers.Where(c => c.CustomerID == "ALFKI").Max(c => new DateTime((c.CustomerID == "ALFKI") ? 1997 : 1997, 7, 4, 3, 5, 6).Second);
            Assert.AreEqual(6, second);
        }

        [TestMethod]
        public void Execution_DateTimeTest_DayOfWeek() {
            var dow = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, dow);
        }

        [TestMethod]
        public void Execution_DateTimeTest_AddYears() {
            var date = this.Northwind.Orders
                                     .OrderBy(d => d.OrderID)
                                     .Where(o => o.OrderDate == new DateTime(1997, 8, 25))
                                     .Take(1)
                                     .Max(o => o.OrderDate.AddHours(9).AddMinutes(10).AddSeconds(11).AddYears(2));

            Assert.AreEqual(1999, date.Year);
            Assert.AreEqual(8, date.Month);
            Assert.AreEqual(25, date.Day);
            Assert.AreEqual(9, date.Hour);
            Assert.AreEqual(10, date.Minute);
            Assert.AreEqual(11, date.Second);
        }

        [TestMethod]
        public void Execution_DateTimeTest_AddMonths() {
            var date = this.Northwind.Orders
                                     .OrderBy(d => d.OrderID)
                                     .Where(o => o.OrderDate == new DateTime(1997, 8, 25))
                                     .Take(1)
                                     .Max(o => o.OrderDate.AddHours(9).AddMinutes(10).AddSeconds(11).AddMonths(5));

            Assert.AreEqual(1998, date.Year);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(25, date.Day);
            Assert.AreEqual(9, date.Hour);
            Assert.AreEqual(10, date.Minute);
            Assert.AreEqual(11, date.Second);
        }

        [TestMethod]
        public void Execution_DateTimeTest_AddDays() {
            DateTime testDate = new DateTime(1997, 8, 26, 7, 55, 12);
            var date = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.AddDays(1.33));
            
            Assert.AreEqual(testDate, date);
        }

        [TestMethod]
        public void Execution_DateTimeTest_AddHours() {
            DateTime testDate = new DateTime(1997, 8, 26, 7, 55, 12);
            var date = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.AddHours(31.92));

            Assert.AreEqual(testDate, date);
        }
        
        [TestMethod]
        public void Execution_DateTimeTest_AddMinutes() {
            DateTime testDate = new DateTime(1997, 8, 26, 7, 55, 12);
            var date = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.AddMinutes(1915.2));

            Assert.AreEqual(testDate, date);
        }

        [TestMethod]
        public void Execution_DateTimeTest_AddSeconds() {
            DateTime testDate = new DateTime(1997, 8, 26, 7, 55, 12);
            var date = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.AddSeconds(114912));

            Assert.AreEqual(testDate, date);
        }

        [TestMethod]
        public void Execution_DateTimeTest_Add() {
            DateTime testDate = new DateTime(1997, 8, 26, 7, 55, 12);
            var date = this.Northwind.Orders.OrderBy(d => d.OrderID).Where(o => o.OrderDate == new DateTime(1997, 8, 25)).Take(1).Max(o => o.OrderDate.Add(new TimeSpan(1, 7, 55, 12)));

            Assert.AreEqual(testDate, date);
        }
    }
}
