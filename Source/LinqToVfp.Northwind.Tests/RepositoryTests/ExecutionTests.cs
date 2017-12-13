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
using LinqToVfp.Northwind.Tests.NorthwindRepository;
using System.Data.OleDb;

namespace LinqToVfp.Northwind.Tests.RepositoryTests {
    [TestClass]
    public class ExecutionTests : ARepositoryTests {
        [TestMethod]
        public void TestContainsWithLocalStringCollectionThasHasManyItems() {
            var query = this.Northwind.List<Customer>().OrderBy(c => c.CustomerID).Select(c => c.CustomerID);
            var idList = query.ToList();
            idList.AddRange(idList); // just doubling the number of ids

            var ids = idList.ToArray();
            var list = this.Northwind.List<Customer>().Where(c => ids.Contains(c.CustomerID)).ToList();

            Assert.AreEqual(91, list.Count);
        }

        [TestMethod]
        public void TestContainsWithLocalIntCollectionThasHasManyItems() {
            var ids = this.Northwind.List<Order>().OrderBy(o => o.OrderID).Take(160).Select(o => o.OrderID).ToArray();
            var list = this.Northwind.List<Order>().Where(o => ids.Contains(o.OrderID)).ToList();

            Assert.AreEqual(160, list.Count);
        }

        /*
        [TestMethod]
        public void TestCompiledQuery() {
            var fn = QueryCompiler.Compile((string id) => this.Northwind.List<Customer>().Where(c => c.CustomerID == id));
            var items = fn("ALKFI").ToList();
        }

        [TestMethod]
        public void TestCompiledQuerySingleton() {
            var fn = QueryCompiler.Compile((string id) => this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == id));
            Customer cust = fn("ALKFI");
        }

        [TestMethod]
        public void TestCompiledQueryCount() {
            var fn = QueryCompiler.Compile((string id) => this.Northwind.List<Customer>().Count(c => c.CustomerID == id));
            int n = fn("ALKFI");
        }

        [TestMethod]
        public void TestCompiledQueryIsolated() {
            var fn = QueryCompiler.Compile((NorthwindDataContext n, string id) => n.Customers.Where(c => c.CustomerID == id));
            var items = fn(this.Northwind, "ALFKI").ToList();
        }

        [TestMethod]
        public void TestCompiledQueryIsolatedWithHeirarchy() {
            var fn = QueryCompiler.Compile((NorthwindDataContext n, string id) => n.Customers.Where(c => c.CustomerID == id).Select(c => n.Orders.Where(o => o.CustomerID == c.CustomerID)));
            var items = fn(this.Northwind, "ALFKI").ToList();
        }
        */

        [TestMethod]
        public void TestWhere() {
            Assert.AreEqual(6, this.Northwind.List<Customer>().Where(c => c.City == "London").Count());
        }

        [TestMethod]
        public void TestWhereTrue() {
            var x = this.Northwind.List<Customer>().Where(c => true);

            Assert.AreEqual(91, this.Northwind.List<Customer>().Where(c => true).Count());
        }

        [TestMethod]
        public void TestCompareEntityEqual() {
            Customer customer = new Customer { CustomerID = "ALFKI" };
            var list = this.Northwind.List<Customer>().Where(c => c == customer).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("ALFKI", list[0].CustomerID);
        }

        [TestMethod]
        public void TestCompareEntityNotEqual() {
            Customer customer = new Customer { CustomerID = "ALFKI" };
            Assert.AreEqual(90, this.Northwind.List<Customer>().Where(c => c != customer).Count());
        }

        [TestMethod]
        public void TestCompareConstructedEqual() {
            Assert.AreEqual(6, this.Northwind.List<Customer>().Where(c => new { x = c.City } == new { x = "London" }).Count());
        }

        [TestMethod]
        public void TestCompareConstructedMultiValueEqual() {
            Assert.AreEqual(6, this.Northwind.List<Customer>().Where(c => new { x = c.City, y = c.Country } == new { x = "London", y = "UK" }).Count());
        }

        [TestMethod]
        public void TestCompareConstructedMultiValueNotEqual() {
            Assert.AreEqual(85, this.Northwind.List<Customer>().Where(c => new { x = c.City, y = c.Country } != new { x = "London", y = "UK" }).Count());
        }

        [TestMethod]
        public void TestSelectScalar() {
            var list = this.Northwind.List<Customer>().Where(c => c.City == "London").Select(c => c.City).ToList();

            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("London", list[0]);
            Assert.IsTrue(list.All(x => x == "London"));
        }

        [TestMethod]
        public void TestSelectAnonymousOne() {
            var list = this.Northwind.List<Customer>().Where(c => c.City == "London").Select(c => new { c.City }).ToList();

            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("London", list[0].City);
            Assert.IsTrue(list.All(x => x.City == "London"));
        }

        [TestMethod]
        public void TestSelectAnonymousTwo() {
            var list = this.Northwind.List<Customer>().Where(c => c.City == "London").Select(c => new { c.City, c.Phone }).ToList();

            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("London", list[0].City);
            Assert.IsTrue(list.All(x => x.City == "London"));
            Assert.IsTrue(list.All(x => x.Phone != null));
        }

        [TestMethod]
        public void TestSelectCustomerTable() {
            var list = this.Northwind.List<Customer>().ToList();
            Assert.AreEqual(91, list.Count);
        }

        [TestMethod]
        public void TestSelectAnonymousWithObject() {
            var list = this.Northwind.List<Customer>().Where(c => c.City == "London").Select(c => new { c.City, c }).ToList();

            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("London", list[0].City);
            Assert.IsTrue(list.All(x => x.City == "London"));
            Assert.IsTrue(list.All(x => x.c.City == x.City));
        }

        [TestMethod]
        public void TestSelectAnonymousLiteral() {
            var list = this.Northwind.List<Customer>().Where(c => c.City == "London").Select(c => new { X = 10 }).ToList();

            Assert.AreEqual(6, list.Count);
            Assert.IsTrue(list.All(x => x.X == 10));
        }

        [TestMethod]
        public void TestSelectConstantInt() {
            var list = this.Northwind.List<Customer>().Select(c => 10).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(list.All(x => x == 10));
        }

        [TestMethod]
        public void TestSelectConstantNullString() {
            var list = this.Northwind.List<Customer>().Select(c => (string)null).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(list.All(x => x == null));
        }

        [TestMethod]
        public void TestSelectLocal() {
            int x = 10;
            var list = this.Northwind.List<Customer>().Select(c => x).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(list.All(y => y == 10));
        }

        [TestMethod]
        public void TestSelectNestedCollection() {
            var list = (
                from c in this.Northwind.List<Customer>()
                where c.CustomerID == "ALFKI"
                select this.Northwind.List<Order>().Where(o => o.CustomerID == c.CustomerID).Select(o => o.OrderID)
                ).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].Count());
        }

        [TestMethod]
        public void TestSelectNestedCollectionInAnonymousType() {
            var list = (
                from c in this.Northwind.List<Customer>()
                where c.CustomerID == "ALFKI"
                select new { Foos = this.Northwind.List<Order>().Where(o => o.CustomerID == c.CustomerID).Select(o => o.OrderID).ToList() }
                ).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].Foos.Count);
        }

        [TestMethod]
        public void TestJoinCustomerOrders() {
            var list = (
                from c in this.Northwind.List<Customer>()
                where c.CustomerID == "ALFKI"
                join o in this.Northwind.List<Order>() on c.CustomerID equals o.CustomerID
                select new { c.ContactName, o.OrderID }
                ).ToList();

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        public void TestJoinMultiKey() {
            var list = (
                from c in this.Northwind.List<Customer>()
                where c.CustomerID == "ALFKI"
                join o in this.Northwind.List<Order>() on new { a = c.CustomerID, b = c.CustomerID } equals new { a = o.CustomerID, b = o.CustomerID }
                select new { c, o }
                ).ToList();

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        public void TestJoinIntoCustomersOrdersCount() {
            var list = (
                from c in this.Northwind.List<Customer>()
                where c.CustomerID == "ALFKI"
                join o in this.Northwind.List<Order>() on c.CustomerID equals o.CustomerID into ords
                select new { cust = c, ords = ords.Count() }
                ).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].ords);
        }

        [TestMethod]
        public void TestJoinIntoDefaultIfEmpty() {
            var list = (
                from c in this.Northwind.List<Customer>()
                where c.CustomerID == "PARIS"
                join o in this.Northwind.List<Order>() on c.CustomerID equals o.CustomerID into ords
                from o in ords.DefaultIfEmpty()
                select new { c, o }
                ).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(null, list[0].o);
        }

        [TestMethod]
        public void TestMultipleJoinsWithJoinConditionsInWhere() {
            // this should reduce to inner joins
            var list = (
                from c in this.Northwind.List<Customer>()
                from o in this.Northwind.List<Order>()
                from d in this.Northwind.List<OrderDetail>()
                where o.CustomerID == c.CustomerID && o.OrderID == d.OrderID
                where c.CustomerID == "ALFKI"
                select d
                ).ToList();

            Assert.AreEqual(12, list.Count);
        }

        [TestMethod]
        public void TestOrderBy() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.CustomerID).Select(c => c.CustomerID).ToList();
            var sorted = list.OrderBy(c => c).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestOrderByOrderBy() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.Phone).OrderBy(c => c.CustomerID).ToList();
            var sorted = list.OrderBy(c => c.CustomerID).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestOrderByThenBy() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.CustomerID).ThenBy(c => c.Phone).ToList();
            var sorted = list.OrderBy(c => c.CustomerID).ThenBy(c => c.Phone).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }


        [TestMethod]
        public void TestOrderByDescending() {
            var list = this.Northwind.List<Customer>().OrderByDescending(c => c.CustomerID).ToList();
            var sorted = list.OrderByDescending(c => c.CustomerID).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestOrderByDescendingThenBy() {
            var list = this.Northwind.List<Customer>().OrderByDescending(c => c.CustomerID).ThenBy(c => c.Country).ToList();
            var sorted = list.OrderByDescending(c => c.CustomerID).ThenBy(c => c.Country).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestOrderByDescendingThenByDescending() {
            var list = this.Northwind.List<Customer>().OrderByDescending(c => c.CustomerID).ThenByDescending(c => c.Country).ToList();
            var sorted = list.OrderByDescending(c => c.CustomerID).ThenByDescending(c => c.Country).ToList();

            Assert.AreEqual(91, list.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestOrderByJoin() {
            var list = (
                from c in this.Northwind.List<Customer>().OrderBy(c => c.CustomerID)
                join o in this.Northwind.List<Order>().OrderBy(o => o.OrderID) on c.CustomerID equals o.CustomerID
                select new { c.CustomerID, o.OrderID }
                ).ToList();

            var sorted = list.OrderBy(x => x.CustomerID).ThenBy(x => x.OrderID);

            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestOrderBySelectMany() {
            var list = (
                from c in this.Northwind.List<Customer>().OrderBy(c => c.CustomerID)
                from o in this.Northwind.List<Order>().OrderBy(o => o.OrderID)
                where c.CustomerID == o.CustomerID
                select new { c.CustomerID, o.OrderID }
                ).ToList();

            var sorted = list.OrderBy(x => x.CustomerID).ThenBy(x => x.OrderID).ToList();

            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestGroupBy() {
            Assert.AreEqual(69, this.Northwind.List<Customer>().GroupBy(c => c.City).ToList().Count);
        }

        [TestMethod]
        public void TestGroupByOne() {
            var list = this.Northwind.List<Customer>().Where(c => c.City == "London").GroupBy(c => c.City).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].Count());
        }

        [TestMethod]
        public void TestGroupBySelectMany() {
            var list = this.Northwind.List<Customer>().GroupBy(c => c.City).SelectMany(g => g).ToList();
            Assert.AreEqual(91, list.Count);
        }

        [TestMethod]
        public void TestGroupBySum() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID).Select(g => g.Sum(o => (o.CustomerID == "ALFKI" ? 1 : 1))).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0]);
        }

        [TestMethod]
        public void TestGroupByCount() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID).Select(g => g.Count()).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0]);
        }

        [TestMethod]
        public void TestGroupByLongCount() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID).Select(g => g.LongCount()).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6L, list[0]);
        }

        [TestMethod]
        public void TestGroupBySumMinMaxAvg() {
            var list =
                this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID).Select(g =>
                    new {
                        Sum = g.Sum(o => (o.CustomerID == "ALFKI" ? 1 : 1)),
                        Min = g.Min(o => o.OrderID),
                        Max = g.Max(o => o.OrderID),
                        Avg = g.Average(o => o.OrderID)
                    }).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].Sum);
        }

        [TestMethod]
        public void TestGroupByWithResultSelector() {
            var list =
                this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID, (k, g) =>
                    new {
                        Sum = g.Sum(o => (o.CustomerID == "ALFKI" ? 1 : 1)),
                        Min = g.Min(o => o.OrderID),
                        Max = g.Max(o => o.OrderID),
                        Avg = g.Average(o => o.OrderID)
                    }).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].Sum);
        }

        [TestMethod]
        public void TestGroupByWithElementSelectorSum() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID, o => (o.CustomerID == "ALFKI" ? 1 : 1)).Select(g => g.Sum()).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0]);
        }

        [TestMethod]
        public void TestGroupByWithElementSelector() {
            // note: groups are retrieved through a separately execute subquery per row
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID, o => (o.CustomerID == "ALFKI" ? 1 : 1)).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].Count());
            Assert.AreEqual(6, list[0].Sum());
        }

        [TestMethod]
        public void TestGroupByWithElementSelectorSumMax() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID, o => (o.CustomerID == "ALFKI" ? 1 : 1)).Select(g => new { Sum = g.Sum(), Max = g.Max() }).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0].Sum);
            Assert.AreEqual(1, list[0].Max);
        }

        [TestMethod]
        public void TestGroupByWithAnonymousElement() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID, o => new { X = (o.CustomerID == "ALFKI" ? 1 : 1) }).Select(g => g.Sum(x => x.X)).ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(6, list[0]);
        }

        [TestMethod]
        public void TestGroupByWithTwoPartKey() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => new { o.CustomerID, o.OrderDate }).Select(g => g.Sum(o => (o.CustomerID == "ALFKI" ? 1 : 1))).ToList();

            Assert.AreEqual(6, list.Count);
        }


        [TestMethod]
        public void TestOrderByGroupBy() {
            // note: order-by is lost when group-by is applied (the sequence of groups is not ordered)
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").OrderBy(o => o.OrderID).GroupBy(o => o.CustomerID).ToList();
            Assert.AreEqual(1, list.Count);

            var grp = list[0].ToList();
            var sorted = grp.OrderBy(o => o.OrderID);
            Assert.IsTrue(Enumerable.SequenceEqual(grp, sorted));
        }

        [TestMethod]
        public void TestOrderByGroupBySelectMany() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").OrderBy(o => o.OrderID).GroupBy(o => o.CustomerID).SelectMany(g => g).ToList();
            Assert.AreEqual(6, list.Count);

            var sorted = list.OrderBy(o => o.OrderID).ToList();
            Assert.IsTrue(Enumerable.SequenceEqual(list, sorted));
        }

        [TestMethod]
        public void TestSumWithNoArg() {
            var sum = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").Select(o => (o.CustomerID == "ALFKI" ? 1 : 1)).Sum();
            Assert.AreEqual(6, sum);
        }

        [TestMethod]
        public void TestSumWithArg() {
            var sum = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").Sum(o => (o.CustomerID == "ALFKI" ? 1 : 1));
            Assert.AreEqual(6, sum);
        }

        [TestMethod]
        public void TestCountWithNoPredicate() {
            var cnt = this.Northwind.List<Order>().Count();
            Assert.AreEqual(830, cnt);
        }

        [TestMethod]
        public void TestCountWithPredicate() {
            var cnt = this.Northwind.List<Order>().Count(o => o.CustomerID == "ALFKI");
            Assert.AreEqual(6, cnt);
        }

        [TestMethod]
        public void TestDistinctNoDupes() {
            var list = this.Northwind.List<Customer>().Distinct().ToList();
            Assert.AreEqual(91, list.Count);
        }

        [TestMethod]
        public void TestDistinctScalar() {
            var list = this.Northwind.List<Customer>().Select(c => c.City).Distinct().ToList();
            Assert.AreEqual(69, list.Count);
        }

        [TestMethod]
        public void TestOrderByDistinct() {
            var list = this.Northwind.List<Customer>().Where(c => c.City.StartsWith("P")).OrderBy(c => c.City).Select(c => c.City).Distinct().ToList();
            var sorted = list.OrderBy(x => x).ToList();
            Assert.AreEqual(list[0], sorted[0]);
            Assert.AreEqual(list[list.Count - 1], sorted[list.Count - 1]);
        }

        [TestMethod]
        public void TestDistinctOrderBy() {
            var list = this.Northwind.List<Customer>().Where(c => c.City.StartsWith("P")).Select(c => c.City).Distinct().OrderBy(c => c).ToList();
            var sorted = list.OrderBy(x => x).ToList();
            Assert.AreEqual(list[0], sorted[0]);
            Assert.AreEqual(list[list.Count - 1], sorted[list.Count - 1]);
        }

        [TestMethod]
        public void TestDistinctGroupBy() {
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").Distinct().GroupBy(o => o.CustomerID).ToList();

            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestGroupByDistinct() {
            // distinct after group-by should not do anything
            var list = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").GroupBy(o => o.CustomerID).Distinct().ToList();

            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestDistinctCount() {
            var cnt = this.Northwind.List<Customer>().Distinct().Count();
            Assert.AreEqual(91, cnt);
        }

        [TestMethod]
        public void TestSelectDistinctCount() {
            // cannot do: SELECT COUNT(DISTINCT some-colum) FROM some-table
            // because COUNT(DISTINCT some-column) does not count nulls
            var cnt = this.Northwind.List<Customer>().Select(c => c.City).Distinct().Count();
            Assert.AreEqual(69, cnt);
        }

        [TestMethod]
        public void TestSelectSelectDistinctCount() {
            var cnt = this.Northwind.List<Customer>().Select(c => c.City).Select(c => c).Distinct().Count();
            Assert.AreEqual(69, cnt);
        }

        [TestMethod]
        public void TestDistinctCountPredicate() {
            var cnt = this.Northwind.List<Customer>().Select(c => new { c.City, c.Country }).Distinct().Count(c => c.City == "London");
            Assert.AreEqual(1, cnt);
        }

        [TestMethod]
        public void TestDistinctSumWithArg() {
            var sum = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").Distinct().Sum(o => (o.CustomerID == "ALFKI" ? 1 : 1));
            Assert.AreEqual(6, sum);
        }

        [TestMethod]
        public void TestSelectDistinctSum() {
            var sum = this.Northwind.List<Order>().Where(o => o.CustomerID == "ALFKI").Select(o => o.OrderID).Distinct().Sum();
            Assert.AreEqual(64835, sum);
        }

        [TestMethod]
        public void TestTake() {
            var list = this.Northwind.List<Order>().OrderBy(o => o.CustomerID).Take(5).ToList();
            Assert.AreEqual(5, list.Count);
        }

        [TestMethod]
        public void TestTakeDistinct() {
            // distinct must be forced to apply after top has been computed
            var list = this.Northwind.List<Order>().OrderBy(o => o.CustomerID).Select(o => o.CustomerID).Take(5).Distinct().ToList();
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestDistinctTake() {
            // top must be forced to apply after distinct has been computed
            var list = this.Northwind.List<Order>().OrderBy(o => o.CustomerID).Select(o => o.CustomerID).Distinct().Take(5).OrderBy(o => o).ToList();
            Assert.AreEqual(5, list.Count);
        }

        [TestMethod]
        public void TestDistinctTakeCount() {
            var cnt = this.Northwind.List<Order>().Distinct().OrderBy(o => o.CustomerID).Select(o => o.CustomerID).Take(5).Count();
            Assert.AreEqual(5, cnt);
        }

        [TestMethod]
        public void TestTakeDistinctCount() {
            var cnt = this.Northwind.List<Order>().OrderBy(o => o.CustomerID).Select(o => o.CustomerID).Take(5).Distinct().Count();
            Assert.AreEqual(1, cnt);
        }

        [TestMethod]
        public void TestFirst() {
            var first = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).First();
            Assert.AreNotEqual(null, first);
            Assert.AreEqual("ROMEY", first.CustomerID);
        }

        [TestMethod]
        public void TestFirstPredicate() {
            var first = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).First(c => c.City == "London");
            Assert.AreNotEqual(null, first);
            Assert.AreEqual("EASTC", first.CustomerID);
        }

        [TestMethod]
        public void TestWhereFirst() {
            var first = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Where(c => c.City == "London").First();
            Assert.AreNotEqual(null, first);
            Assert.AreEqual("EASTC", first.CustomerID);
        }

        [TestMethod]
        public void TestFirstOrDefault() {
            var first = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).FirstOrDefault();
            Assert.AreNotEqual(null, first);
            Assert.AreEqual("ROMEY", first.CustomerID);
        }

        [TestMethod]
        public void TestFirstOrDefaultPredicate() {
            var first = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).FirstOrDefault(c => c.City == "London");
            Assert.AreNotEqual(null, first);
            Assert.AreEqual("EASTC", first.CustomerID);
        }

        [TestMethod]
        public void TestWhereFirstOrDefault() {
            var first = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Where(c => c.City == "London").FirstOrDefault();
            Assert.AreNotEqual(null, first);
            Assert.AreEqual("EASTC", first.CustomerID);
        }

        [TestMethod]
        public void TestFirstOrDefaultPredicateNoMatch() {
            var first = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).FirstOrDefault(c => c.City == "SpongeBob");
            Assert.AreEqual(null, first);
        }

        [TestMethod]
        public void TestReverse() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Reverse().ToList();
            Assert.AreEqual(91, list.Count);
            Assert.AreEqual("WOLZA", list[0].CustomerID);
            Assert.AreEqual("ROMEY", list[90].CustomerID);
        }

        [TestMethod]
        public void TestReverseReverse() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Reverse().Reverse().ToList();
            Assert.AreEqual(91, list.Count);
            Assert.AreEqual("ROMEY", list[0].CustomerID);
            Assert.AreEqual("WOLZA", list[90].CustomerID);
        }

        [TestMethod]
        public void TestReverseWhereReverse() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Reverse().Where(c => c.City == "London").Reverse().ToList();
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("EASTC", list[0].CustomerID);
            Assert.AreEqual("BSBEV", list[5].CustomerID);
        }

        [TestMethod]
        public void TestReverseTakeReverse() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Reverse().Take(5).Reverse().ToList();
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("CHOPS", list[0].CustomerID);
            Assert.AreEqual("WOLZA", list[4].CustomerID);
        }

        [TestMethod]
        public void TestReverseWhereTakeReverse() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Reverse().Where(c => c.City == "London").Take(5).Reverse().ToList();
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("CONSH", list[0].CustomerID);
            Assert.AreEqual("BSBEV", list[4].CustomerID);
        }

        [TestMethod]
        public void TestLast() {
            var last = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Last();
            Assert.AreNotEqual(null, last);
            Assert.AreEqual("WOLZA", last.CustomerID);
        }

        [TestMethod]
        public void TestLastPredicate() {
            var last = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Last(c => c.City == "London");
            Assert.AreNotEqual(null, last);
            Assert.AreEqual("BSBEV", last.CustomerID);
        }

        [TestMethod]
        public void TestWhereLast() {
            var last = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Where(c => c.City == "London").Last();
            Assert.AreNotEqual(null, last);
            Assert.AreEqual("BSBEV", last.CustomerID);
        }

        [TestMethod]
        public void TestLastOrDefault() {
            var last = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).LastOrDefault();
            Assert.AreNotEqual(null, last);
            Assert.AreEqual("WOLZA", last.CustomerID);
        }

        [TestMethod]
        public void TestLastOrDefaultPredicate() {
            var last = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).LastOrDefault(c => c.City == "London");
            Assert.AreNotEqual(null, last);
            Assert.AreEqual("BSBEV", last.CustomerID);
        }

        [TestMethod]
        public void TestWhereLastOrDefault() {
            var last = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).Where(c => c.City == "London").LastOrDefault();
            Assert.AreNotEqual(null, last);
            Assert.AreEqual("BSBEV", last.CustomerID);
        }

        [TestMethod]
        public void TestLastOrDefaultNoMatches() {
            var last = this.Northwind.List<Customer>().OrderBy(c => c.ContactName).LastOrDefault(c => c.City == "SpongeBob");
            Assert.AreEqual(null, last);
        }

        [TestMethod]
        public void TestSingleFails() {
            try {
                var single = this.Northwind.List<Customer>().Single();
            }
            catch (InvalidOperationException ex) {
                if (ex.Message.Contains("Sequence contains more than one element")) {
                    return;
                }

                throw;
            }
            throw new Exception("The following Exception was not thrown.\rInvalidOperationException: Sequence contains more than one element ");
        }

        [TestMethod]
        public void TestSinglePredicate() {
            var single = this.Northwind.List<Customer>().Single(c => c.CustomerID == "ALFKI");
            Assert.AreNotEqual(null, single);
            Assert.AreEqual("ALFKI", single.CustomerID);
        }

        [TestMethod]
        public void TestWhereSingle() {
            var single = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Single();
            Assert.AreNotEqual(null, single);
            Assert.AreEqual("ALFKI", single.CustomerID);
        }

        [TestMethod]
        public void TestSingleOrDefaultFails() {
            try {
                var single = this.Northwind.List<Customer>().SingleOrDefault();
            }
            catch (InvalidOperationException ex) {
                if (ex.Message.Contains("Sequence contains more than one element")) {
                    return;
                }

                throw;
            }
            throw new Exception("The following Exception was not thrown.\rInvalidOperationException: Sequence contains more than one element ");
        }

        [TestMethod]
        public void TestSingleOrDefaultPredicate() {
            var single = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI");
            Assert.AreNotEqual(null, single);
            Assert.AreEqual("ALFKI", single.CustomerID);
        }

        [TestMethod]
        public void TestWhereSingleOrDefault() {
            var single = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault();
            Assert.AreNotEqual(null, single);
            Assert.AreEqual("ALFKI", single.CustomerID);
        }

        [TestMethod]
        public void TestSingleOrDefaultNoMatches() {
            var single = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "SpongeBob");
            Assert.AreEqual(null, single);
        }

        [TestMethod]
        public void TestAnyTopLevel() {
            var any = this.Northwind.List<Customer>().Any();
            Assert.IsTrue(any);
        }

        [TestMethod]
        public void TestAnyWithSubquery() {
            var list = this.Northwind.List<Customer>().Where(c => c.Orders.Any(o => o.CustomerID == "ALFKI")).ToList();
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestAnyWithSubqueryNoPredicate() {
            // customers with at least one order
            var list = this.Northwind.List<Customer>().Where(c => this.Northwind.List<Order>().Where(o => o.CustomerID == c.CustomerID).Any()).ToList();
            Assert.AreEqual(89, list.Count);
        }

        [TestMethod]
        public void TestAnyWithLocalCollection() {
            // get customers for any one of these IDs
            string[] ids = new[] { "ALFKI", "WOLZA", "NOONE" };
            var list = this.Northwind.List<Customer>().Where(c => ids.Any(id => c.CustomerID == id)).ToList();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void TestAllWithSubquery() {
            var list = this.Northwind.List<Customer>().Where(c => c.Orders.All(o => o.CustomerID == "ALFKI")).ToList();
            // includes customers w/ no orders
            Assert.AreEqual(3, list.Count);
        }

        [TestMethod]
        public void TestAllWithLocalCollection() {
            // get all customers with a name that contains both 'm' and 'd'  (don't use vowels since these often depend on collation)
            string[] patterns = new[] { "m", "d" };

            var list = this.Northwind.List<Customer>().Where(c => patterns.All(p => c.ContactName.ToLower().Contains(p))).Select(c => c.ContactName).ToList();
            var local = this.Northwind.List<Customer>().AsEnumerable().Where(c => patterns.All(p => c.ContactName.ToLower().Contains(p))).Select(c => c.ContactName).ToList();

            Assert.AreEqual(local.Count, list.Count);
        }

        [TestMethod]
        public void TestAllTopLevel() {
            // all customers have name length > 0?
            var all = this.Northwind.List<Customer>().All(c => c.ContactName.Length > 0);
            Assert.IsTrue(all);
        }

        [TestMethod]
        public void TestAllTopLevelNoMatches() {
            // all customers have name with 'a'
            var all = this.Northwind.List<Customer>().All(c => c.ContactName.Contains("a"));
            Assert.IsFalse(all);
        }

        [TestMethod]
        public void TestContainsWithSubquery() {
            // this is the long-way to determine all customers that have at least one order
            var list = this.Northwind.List<Customer>().Where(c => this.Northwind.List<Order>().Select(o => o.CustomerID).Contains(c.CustomerID)).ToList();
            Assert.AreEqual(89, list.Count);
        }

        [TestMethod]
        public void TestContainsWithLocalCollection() {
            string[] ids = new[] { "ALFKI", "WOLZA", "NOONE" };
            var list = this.Northwind.List<Customer>().Where(c => ids.Contains(c.CustomerID)).ToList();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void TestContainsTopLevel() {
            var contains = this.Northwind.List<Customer>().Select(c => c.CustomerID).Contains("ALFKI");
            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void TestSkipTake() {
            var list = this.Northwind.List<Customer>().OrderBy(c => c.CustomerID).Skip(5).Take(10).ToList();
            Assert.AreEqual(10, list.Count);
            Assert.AreEqual("BLAUS", list[0].CustomerID);
            Assert.AreEqual("COMMI", list[9].CustomerID);
        }

        [TestMethod]
        public void TestDistinctSkipTake() {
            var list = this.Northwind.List<Customer>().Select(c => c.City).Distinct().OrderBy(c => c).Skip(5).Take(10).ToList();
            Assert.AreEqual(10, list.Count);
            var hs = new HashSet<string>(list);
            Assert.AreEqual(10, hs.Count);
        }

        [TestMethod]
        public void TestCoalesce() {
            var list = this.Northwind.List<Customer>().Select(c => new { City = (c.City == "London" ? null : c.City), Country = (c.CustomerID == "EASTC" ? null : c.Country) })
                         .Where(x => (x.City ?? "NoCity") == "NoCity").ToList();
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual(null, list[0].City);
        }

        [TestMethod]
        public void TestCoalesce2() {
            var list = this.Northwind.List<Customer>().Select(c => new { City = (c.City == "London" ? null : c.City), Country = (c.CustomerID == "EASTC" ? null : c.Country) })
                         .Where(x => (x.City ?? x.Country ?? "NoCityOrCountry") == "NoCityOrCountry").ToList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(null, list[0].City);
            Assert.AreEqual(null, list[0].Country);
        }

        // framework function tests
        [TestMethod]
        public void TestStringLength() {
            var list = this.Northwind.List<Customer>().Where(c => c.City.Trim().Length == 7).ToList();
            Assert.AreEqual(9, list.Count);
        }

        [TestMethod]
        public void TestStringStartsWithLiteral() {
            var list = this.Northwind.List<Customer>().Where(c => c.ContactName.StartsWith("M")).ToList();
            Assert.AreEqual(12, list.Count);
        }

        [TestMethod]
        public void TestStringStartsWithColumn() {
            var list = this.Northwind.List<Customer>().Where(c => c.ContactName.StartsWith(c.ContactName)).ToList();
            Assert.AreEqual(91, list.Count);
        }

        [TestMethod]
        public void TestStringEndsWithLiteral() {
            var list = this.Northwind.List<Customer>().Where(c => c.ContactName.EndsWith("s")).ToList();
            Assert.AreEqual(9, list.Count);
        }

        [TestMethod]
        public void TestStringEndsWithColumn() {
            var list = this.Northwind.List<Customer>().Where(c => c.ContactName.EndsWith(c.ContactName)).ToList();
            Assert.AreEqual(91, list.Count);
        }

        [TestMethod]
        public void TestStringContainsLiteral() {
            var list = this.Northwind.List<Customer>().Where(c => c.ContactName.Contains("nd")).Select(c => c.ContactName).ToList();
            var local = this.Northwind.List<Customer>().AsEnumerable().Where(c => c.ContactName.ToLower().Contains("nd")).Select(c => c.ContactName).ToList();
            Assert.AreEqual(local.Count, list.Count);
        }

        [TestMethod]
        public void TestStringContainsColumn() {
            var list = this.Northwind.List<Customer>().Where(c => c.ContactName.Contains(c.ContactName)).ToList();
            Assert.AreEqual(91, list.Count);
        }

        [TestMethod]
        public void TestStringConcatImplicit2Args() {
            var list = this.Northwind.List<Customer>().Where(c => c.ContactName.Trim() + "X" == "Maria AndersX").ToList();
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestStringConcatExplicit2Args() {
            var list = this.Northwind.List<Customer>().Where(c => string.Concat(c.ContactName.Trim(), "X") == "Maria AndersX").ToList();
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestStringConcatExplicit3Args() {
            var list = this.Northwind.List<Customer>().Where(c => string.Concat(c.ContactName.Trim(), "X", c.Country) == "Maria AndersXGermany").ToList();
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestStringConcatExplicitNArgs() {
            var list = this.Northwind.List<Customer>().Where(c => string.Concat(new string[] { c.ContactName.Trim(), "X", c.Country }) == "Maria AndersXGermany").ToList();
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestStringIsNullOrEmpty() {
            var list = this.Northwind.List<Customer>().Select(c => c.City == "London" ? null : c.CustomerID).Where(x => string.IsNullOrEmpty(x)).ToList();
            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        public void TestStringToUpper() {
            var str = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Max(c => (c.CustomerID == "ALFKI" ? "abc" : "abc").ToUpper());
            Assert.AreEqual("ABC", str);
        }

        [TestMethod]
        public void TestStringToLower() {
            var str = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Max(c => (c.CustomerID == "ALFKI" ? "ABC" : "ABC").ToLower());
            Assert.AreEqual("abc", str);
        }

        [TestMethod]
        public void TestStringSubstring() {
            var list = this.Northwind.List<Customer>().Where(c => c.City.Substring(2, 4) == "attl").ToList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Seattle", list[0].City);
        }

        [TestMethod]
        public void TestStringSubstringNoLength() {
            var list = this.Northwind.List<Customer>().Where(c => c.City.Substring(4) == "tle").ToList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Seattle", list[0].City);
        }

        [TestMethod]
        public void TestStringIndexOf() {
            var n = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => c.ContactName.IndexOf("ar"));
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        public void TestStringIndexOfChar() {
            var n = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => c.ContactName.IndexOf('r'));
            Assert.AreEqual(2, n);
        }

        [TestMethod]
        public void TestStringIndexOfWithStart() {
            var n = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => c.ContactName.IndexOf("a", 3));
            Assert.AreEqual(4, n);
        }

        [TestMethod]
        public void TestStringTrim() {
            var notrim = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Max(c => ("  " + c.City + " "));
            var trim = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Max(c => ("  " + c.City + " ").Trim());
            Assert.AreNotEqual(notrim, trim);
            Assert.AreEqual(notrim.Trim(), trim);
        }

        [TestMethod]
        public void TestMathAbs() {
            var neg1 = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Abs((c.CustomerID == "ALFKI") ? -1 : 0));
            var pos1 = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Abs((c.CustomerID == "ALFKI") ? 1 : 0));
            Assert.AreEqual(Math.Abs(-1), neg1);
            Assert.AreEqual(Math.Abs(1), pos1);
        }

        [TestMethod]
        public void TestMathAtan() {
            var zero = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Atan((c.CustomerID == "ALFKI") ? 0.0 : 0.0));
            var one = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Atan((c.CustomerID == "ALFKI") ? 1.0 : 1.0));
            Assert.AreEqual(Math.Atan(0.0), zero, 0.0001);
            Assert.AreEqual(Math.Atan(1.0), one, 0.0001);
        }

        [TestMethod]
        public void TestMathCos() {
            var zero = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Cos((c.CustomerID == "ALFKI") ? 0.0 : 0.0));
            var pi = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Cos((c.CustomerID == "ALFKI") ? Math.PI : Math.PI));
            Assert.AreEqual(Math.Cos(0.0), zero, 0.0001);
            Assert.AreEqual(Math.Cos(Math.PI), pi, 0.0001);
        }

        [TestMethod]
        public void TestMathSin() {
            var zero = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Sin((c.CustomerID == "ALFKI") ? 0.0 : 0.0));
            var pi = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Sin((c.CustomerID == "ALFKI") ? Math.PI : Math.PI));
            var pi2 = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Sin(((c.CustomerID == "ALFKI") ? Math.PI : Math.PI) / 2.0));
            Assert.AreEqual(Math.Sin(0.0), zero);
            Assert.AreEqual(Math.Sin(Math.PI), pi, 0.0001);
            Assert.AreEqual(Math.Sin(Math.PI / 2.0), pi2, 0.0001);
        }

        [TestMethod]
        public void TestMathTan() {
            var zero = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Tan((c.CustomerID == "ALFKI") ? 0.0 : 0.0));
            var pi = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Tan((c.CustomerID == "ALFKI") ? Math.PI : Math.PI));
            Assert.AreEqual(Math.Tan(0.0), zero, 0.0001);
            Assert.AreEqual(Math.Tan(Math.PI), pi, 0.0001);
        }

        [TestMethod]
        public void TestMathExp() {
            var zero = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Exp((c.CustomerID == "ALFKI") ? 0.0 : 0.0));
            var one = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Exp((c.CustomerID == "ALFKI") ? 1.0 : 1.0));
            var two = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Exp((c.CustomerID == "ALFKI") ? 2.0 : 2.0));
            Assert.AreEqual(Math.Exp(0.0), zero, 0.0001);
            Assert.AreEqual(Math.Exp(1.0), one, 0.0001);
            Assert.AreEqual(Math.Exp(2.0), two, 0.0001);
        }

        [TestMethod]
        public void TestMathLog() {
            var one = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Log((c.CustomerID == "ALFKI") ? 1.0 : 1.0));
            var e = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Log((c.CustomerID == "ALFKI") ? Math.E : Math.E));
            Assert.AreEqual(Math.Log(1.0), one, 0.0001);
            Assert.AreEqual(Math.Log(Math.E), e, 0.0001);
        }

        [TestMethod]
        public void TestMathSqrt() {
            var one = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Sqrt((c.CustomerID == "ALFKI") ? 1.0 : 1.0));
            var four = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Sqrt((c.CustomerID == "ALFKI") ? 4.0 : 4.0));
            var nine = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Sqrt((c.CustomerID == "ALFKI") ? 9.0 : 9.0));
            Assert.AreEqual(1.0, one);
            Assert.AreEqual(2.0, four);
            Assert.AreEqual(3.0, nine);
        }

        [TestMethod]
        public void TestMathPow() {
            // 2^n
            var zero = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Pow((c.CustomerID == "ALFKI") ? 2.0 : 2.0, 0.0));
            var one = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Pow((c.CustomerID == "ALFKI") ? 2.0 : 2.0, 1.0));
            var two = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Pow((c.CustomerID == "ALFKI") ? 2.0 : 2.0, 2.0));
            var three = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Pow((c.CustomerID == "ALFKI") ? 2.0 : 2.0, 3.0));
            Assert.AreEqual(1.0, zero);
            Assert.AreEqual(2.0, one);
            Assert.AreEqual(4.0, two);
            Assert.AreEqual(8.0, three);
        }

        [TestMethod]
        public void TestMathRoundDefault() {
            var four = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Round((c.CustomerID == "ALFKI") ? 3.4 : 3.4));
            var six = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Round((c.CustomerID == "ALFKI") ? 3.6 : 3.6));
            Assert.AreEqual(3.0, four);
            Assert.AreEqual(4.0, six);
        }

        [TestMethod]
        public void TestMathFloor() {
            // The difference between floor and truncate is how negatives are handled.  Floor drops the decimals and moves the
            // value to the more negative, so Floor(-3.4) is -4.0 and Floor(3.4) is 3.0.
            var four = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Floor((c.CustomerID == "ALFKI" ? 3.4 : 3.4)));
            var six = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Floor((c.CustomerID == "ALFKI" ? 3.6 : 3.6)));
            var nfour = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Floor((c.CustomerID == "ALFKI" ? -3.4 : -3.4)));
            Assert.AreEqual(Math.Floor(3.4), four);
            Assert.AreEqual(Math.Floor(3.6), six);
            Assert.AreEqual(Math.Floor(-3.4), nfour);
        }

        [TestMethod]
        public void TestDecimalFloor() {
            var four = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Floor((c.CustomerID == "ALFKI" ? 3.4m : 3.4m)));
            var six = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Floor((c.CustomerID == "ALFKI" ? 3.6m : 3.6m)));
            var nfour = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Floor((c.CustomerID == "ALFKI" ? -3.4m : -3.4m)));
            Assert.AreEqual(decimal.Floor(3.4m), four);
            Assert.AreEqual(decimal.Floor(3.6m), six);
            Assert.AreEqual(decimal.Floor(-3.4m), nfour);
        }

        //[TestMethod]
        //public void TestMathTruncate() {
        //    // The difference between floor and truncate is how negatives are handled.  Truncate drops the decimals, 
        //    // therefore a truncated negative often has a more positive value than non-truncated (never has a less positive),
        //    // so Truncate(-3.4) is -3.0 and Truncate(3.4) is 3.0.
        //    var four = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Truncate((c.CustomerID == "ALFKI") ? 3.4 : 3.4));
        //    var six = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Truncate((c.CustomerID == "ALFKI") ? 3.6 : 3.6));
        //    var neg4 = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Truncate((c.CustomerID == "ALFKI") ? -3.4 : -3.4));
        //    Assert.AreEqual(Math.Truncate(3.4), four);
        //    Assert.AreEqual(Math.Truncate(3.6), six);
        //    Assert.AreEqual(Math.Truncate(-3.4), neg4);
        //}

        [TestMethod]
        public void TestStringCompareTo() {
            var lt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => c.City.CompareTo("Seattle"));
            var gt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => c.City.CompareTo("Aaa"));
            var eq = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => c.City.CompareTo("Berlin"));
            Assert.AreEqual(-1, lt);
            Assert.AreEqual(1, gt);
            Assert.AreEqual(0, eq);
        }

        [TestMethod]
        public void TestStringCompareToLT() {
            var cmpLT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Seattle") < 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Berlin") < 0);
            Assert.AreNotEqual(null, cmpLT);
            Assert.AreEqual(null, cmpEQ);
        }

        [TestMethod]
        public void TestStringCompareToLE() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Seattle") <= 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Berlin") <= 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Aaa") <= 0);
            Assert.AreNotEqual(null, cmpLE);
            Assert.AreNotEqual(null, cmpEQ);
            Assert.AreEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestStringCompareToGT() {
            var cmpLT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Aaa") > 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Berlin") > 0);
            Assert.AreNotEqual(null, cmpLT);
            Assert.AreEqual(null, cmpEQ);
        }

        [TestMethod]
        public void TestStringCompareToGE() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Seattle") >= 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Berlin") >= 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Aaa") >= 0);
            Assert.AreEqual(null, cmpLE);
            Assert.AreNotEqual(null, cmpEQ);
            Assert.AreNotEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestStringCompareToEQ() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Seattle") == 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Berlin") == 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Aaa") == 0);
            Assert.AreEqual(null, cmpLE);
            Assert.AreNotEqual(null, cmpEQ);
            Assert.AreEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestStringCompareToNE() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Seattle") != 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Berlin") != 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => c.City.CompareTo("Aaa") != 0);
            Assert.AreNotEqual(null, cmpLE);
            Assert.AreEqual(null, cmpEQ);
            Assert.AreNotEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestStringCompare() {
            var lt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => string.Compare(c.City, "Seattle"));
            var gt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => string.Compare(c.City, "Aaa"));
            var eq = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => string.Compare(c.City, "Berlin"));
            Assert.AreEqual(-1, lt);
            Assert.AreEqual(1, gt);
            Assert.AreEqual(0, eq);
        }

        [TestMethod]
        public void TestStringCompareLT() {
            var cmpLT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Seattle") < 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Berlin") < 0);
            Assert.AreNotEqual(null, cmpLT);
            Assert.AreEqual(null, cmpEQ);
        }

        [TestMethod]
        public void TestStringCompareLE() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Seattle") <= 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Berlin") <= 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Aaa") <= 0);
            Assert.AreNotEqual(null, cmpLE);
            Assert.AreNotEqual(null, cmpEQ);
            Assert.AreEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestStringCompareGT() {
            var cmpLT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Aaa") > 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Berlin") > 0);
            Assert.AreNotEqual(null, cmpLT);
            Assert.AreEqual(null, cmpEQ);
        }

        [TestMethod]
        public void TestStringCompareGE() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Seattle") >= 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Berlin") >= 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Aaa") >= 0);
            Assert.AreEqual(null, cmpLE);
            Assert.AreNotEqual(null, cmpEQ);
            Assert.AreNotEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestStringCompareEQ() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Seattle") == 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Berlin") == 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Aaa") == 0);
            Assert.AreEqual(null, cmpLE);
            Assert.AreNotEqual(null, cmpEQ);
            Assert.AreEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestStringCompareNE() {
            var cmpLE = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Seattle") != 0);
            var cmpEQ = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Berlin") != 0);
            var cmpGT = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").SingleOrDefault(c => string.Compare(c.City, "Aaa") != 0);
            Assert.AreNotEqual(null, cmpLE);
            Assert.AreEqual(null, cmpEQ);
            Assert.AreNotEqual(null, cmpGT);
        }

        [TestMethod]
        public void TestIntCompareTo() {
            // prove that x.CompareTo(y) works for types other than string
            var eq = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => (c.CustomerID == "ALFKI" ? 10 : 10).CompareTo(10));
            var gt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => (c.CustomerID == "ALFKI" ? 10 : 10).CompareTo(9));
            var lt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => (c.CustomerID == "ALFKI" ? 10 : 10).CompareTo(11));
            Assert.AreEqual(0, eq);
            Assert.AreEqual(1, gt);
            Assert.AreEqual(-1, lt);
        }

        [TestMethod]
        public void TestDecimalCompare() {
            // prove that type.Compare(x,y) works with decimal
            var eq = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Compare((c.CustomerID == "ALFKI" ? 10m : 10m), 10m));
            var gt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Compare((c.CustomerID == "ALFKI" ? 10m : 10m), 9m));
            var lt = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Compare((c.CustomerID == "ALFKI" ? 10m : 10m), 11m));
            Assert.AreEqual(0, eq);
            Assert.AreEqual(1, gt);
            Assert.AreEqual(-1, lt);
        }

        [TestMethod]
        public void TestDecimalAdd() {
            var onetwo = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Add((c.CustomerID == "ALFKI" ? 1m : 1m), 2m));
            Assert.AreEqual(3m, onetwo);
        }

        [TestMethod]
        public void TestDecimalSubtract() {
            var onetwo = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Subtract((c.CustomerID == "ALFKI" ? 1m : 1m), 2m));
            Assert.AreEqual(-1m, onetwo);
        }

        [TestMethod]
        public void TestDecimalMultiply() {
            var onetwo = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Multiply((c.CustomerID == "ALFKI" ? 1m : 1m), 2m));
            Assert.AreEqual(2m, onetwo);
        }

        [TestMethod]
        public void TestDecimalDivide() {
            var onetwo = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Divide((c.CustomerID == "ALFKI" ? 1.0m : 1.0m), 2.0m));
            Assert.AreEqual(0.5m, onetwo);
        }

        [TestMethod]
        public void TestDecimalNegate() {
            var one = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Negate((c.CustomerID == "ALFKI" ? 1m : 1m)));
            Assert.AreEqual(-1m, one);
        }

        [TestMethod]
        public void TestDecimalRoundDefault() {
            var four = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Round((c.CustomerID == "ALFKI" ? 3.4m : 3.4m)));
            var six = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Round((c.CustomerID == "ALFKI" ? 3.5m : 3.5m)));
            Assert.AreEqual(3.0m, four);
            Assert.AreEqual(4.0m, six);
        }

        //[TestMethod]
        //public void TestDecimalTruncate() {
        //    // The difference between floor and truncate is how negatives are handled.  Truncate drops the decimals, 
        //    // therefore a truncated negative often has a more positive value than non-truncated (never has a less positive),
        //    // so Truncate(-3.4) is -3.0 and Truncate(3.4) is 3.0.
        //    var four = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => decimal.Truncate((c.CustomerID == "ALFKI") ? 3.4m : 3.4m));
        //    var six = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Truncate((c.CustomerID == "ALFKI") ? 3.6m : 3.6m));
        //    var neg4 = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => Math.Truncate((c.CustomerID == "ALFKI") ? -3.4m : -3.4m));
        //    Assert.AreEqual(decimal.Truncate(3.4m), four);
        //    Assert.AreEqual(decimal.Truncate(3.6m), six);
        //    Assert.AreEqual(decimal.Truncate(-3.4m), neg4);
        //}

        [TestMethod]
        public void TestDecimalLT() {
            // prove that decimals are treated normally with respect to normal comparison operators
            var alfki = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 1.0m : 3.0m) < 2.0m);
            Assert.AreNotEqual(null, alfki);
        }

        [TestMethod]
        public void TestIntLessThan() {
            var alfki = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 1 : 3) < 2);
            var alfkiN = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 3 : 1) < 2);
            Assert.AreNotEqual(null, alfki);
            Assert.AreEqual(null, alfkiN);
        }

        [TestMethod]
        public void TestIntLessThanOrEqual() {
            var alfki = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 1 : 3) <= 2);
            var alfki2 = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 2 : 3) <= 2);
            var alfkiN = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 3 : 1) <= 2);
            Assert.AreNotEqual(null, alfki);
            Assert.AreNotEqual(null, alfki2);
            Assert.AreEqual(null, alfkiN);
        }

        [TestMethod]
        public void TestIntGreaterThan() {
            var alfki = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 3 : 1) > 2);
            var alfkiN = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 1 : 3) > 2);
            Assert.AreNotEqual(null, alfki);
            Assert.AreEqual(null, alfkiN);
        }

        [TestMethod]
        public void TestIntGreaterThanOrEqual() {
            var alfki = this.Northwind.List<Customer>().Single(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 3 : 1) >= 2);
            var alfki2 = this.Northwind.List<Customer>().Single(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 3 : 2) >= 2);
            var alfkiN = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 1 : 3) > 2);
            Assert.AreNotEqual(null, alfki);
            Assert.AreNotEqual(null, alfki2);
            Assert.AreEqual(null, alfkiN);
        }

        [TestMethod]
        public void TestIntEqual() {
            var alfki = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 1 : 1) == 1);
            var alfkiN = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 1 : 1) == 2);
            Assert.AreNotEqual(null, alfki);
            Assert.AreEqual(null, alfkiN);
        }

        [TestMethod]
        public void TestIntNotEqual() {
            var alfki = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 2 : 2) != 1);
            var alfkiN = this.Northwind.List<Customer>().SingleOrDefault(c => c.CustomerID == "ALFKI" && (c.CustomerID == "ALFKI" ? 2 : 2) != 2);
            Assert.AreNotEqual(null, alfki);
            Assert.AreEqual(null, alfkiN);
        }

        [TestMethod]
        public void TestIntAdd() {
            var three = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 1 : 1) + 2);
            Assert.AreEqual(3, three);
        }

        [TestMethod]
        public void TestIntSubtract() {
            var negone = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 1 : 1) - 2);
            Assert.AreEqual(-1, negone);
        }

        [TestMethod]
        public void TestIntMultiply() {
            var six = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 2 : 2) * 3);
            Assert.AreEqual(6, six);
        }

        [TestMethod]
        public void TestIntDivide() {
            var one = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 3.0 : 3.0) / 2);
            Assert.AreEqual(1.5, one);
        }

        [TestMethod]
        public void TestIntModulo() {
            var three = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 7 : 7) % 4);
            Assert.AreEqual(3, three);
        }

        [TestMethod]
        public void TestIntLeftShift() {
            var eight = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 1 : 1) << 3);
            Assert.AreEqual(8, eight);
        }

        [TestMethod]
        public void TestIntRightShift() {
            var eight = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 32 : 32) >> 2);
            Assert.AreEqual(8, eight);
        }

        [TestMethod]
        public void TestIntBitwiseAnd() {
            var band = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 6 : 6) & 3);
            Assert.AreEqual(2, band);
        }

        [TestMethod]
        public void TestIntBitwiseOr() {
            var eleven = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 10 : 10) | 3);
            Assert.AreEqual(11, eleven);
        }

        [TestMethod]
        public void TestIntBitwiseExclusiveOr() {
            var zero = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ((c.CustomerID == "ALFKI") ? 1 : 1) ^ 1);
            Assert.AreEqual(0, zero);
        }

        [TestMethod]
        public void TestIntBitwiseNot() {
            var bneg = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => ~((c.CustomerID == "ALFKI") ? -1 : -1));
            Assert.AreEqual(~-1, bneg);
        }

        [TestMethod]
        public void TestIntNegate() {
            var neg = this.Northwind.List<Customer>().Where(c => c.CustomerID == "ALFKI").Sum(c => -((c.CustomerID == "ALFKI") ? 1 : 1));
            Assert.AreEqual(-1, neg);
        }

        [TestMethod]
        public void TestAnd() {
            var custs = this.Northwind.List<Customer>().Where(c => c.Country == "USA" && c.City.StartsWith("A")).Select(c => c.City).ToList();
            Assert.AreEqual(2, custs.Count);
            Assert.IsTrue(custs.All(c => c.StartsWith("A")));
        }

        [TestMethod]
        public void TestOr() {
            var custs = this.Northwind.List<Customer>().Where(c => c.Country == "USA" || c.City.StartsWith("A")).Select(c => c.City).ToList();
            Assert.AreEqual(14, custs.Count);
        }

        [TestMethod]
        public void TestNot() {
            var custs = this.Northwind.List<Customer>().Where(c => !(c.Country == "USA")).Select(c => c.Country).ToList();
            Assert.AreEqual(78, custs.Count);
        }

        [TestMethod]
        public void TestEqualLiteralNull() {
            var q = this.Northwind.List<Customer>().Select(c => c.CustomerID == "ALFKI" ? null : c.CustomerID).Where(x => x == null);
            Assert.IsTrue(this.Northwind.Provider.GetQueryText(q.Expression).Contains("IS NULL"));
            var n = q.Count();
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        public void TestEqualLiteralNullReversed() {
            var q = this.Northwind.List<Customer>().Select(c => c.CustomerID == "ALFKI" ? null : c.CustomerID).Where(x => null == x);
            Assert.IsTrue(this.Northwind.Provider.GetQueryText(q.Expression).Contains("IS NULL"));
            var n = q.Count();
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        public void TestNotEqualLiteralNull() {
            var q = this.Northwind.List<Customer>().Select(c => c.CustomerID == "ALFKI" ? null : c.CustomerID).Where(x => x != null);
            Assert.IsTrue(this.Northwind.Provider.GetQueryText(q.Expression).Contains("IS NOT NULL"));
            var n = q.Count();
            Assert.AreEqual(90, n);
        }

        [TestMethod]
        public void TestNotEqualLiteralNullReversed() {
            var q = this.Northwind.List<Customer>().Select(c => c.CustomerID == "ALFKI" ? null : c.CustomerID).Where(x => null != x);
            Assert.IsTrue(this.Northwind.Provider.GetQueryText(q.Expression).Contains("IS NOT NULL"));
            var n = q.Count();
            Assert.AreEqual(90, n);
        }

        [TestMethod]
        public void TestConditionalResultsArePredicates() {
            bool value = this.Northwind.List<Order>().Where(c => c.CustomerID == "ALFKI").Max(c => (c.CustomerID == "ALFKI" ? string.Compare(c.CustomerID, "POTATO") < 0 : string.Compare(c.CustomerID, "POTATO") > 0));
            Assert.IsTrue(value);
        }

        [TestMethod]
        public void TestSelectManyJoined() {
            var cods =
                (from c in this.Northwind.List<Customer>()
                 from o in this.Northwind.List<Order>().Where(o => o.CustomerID == c.CustomerID)
                 select new { c.ContactName, o.OrderDate }).ToList();
            Assert.AreEqual(830, cods.Count);
        }

        [TestMethod]
        public void TestSelectManyJoinedDefaultIfEmpty() {
            var cods = (
                from c in this.Northwind.List<Customer>()
                from o in this.Northwind.List<Order>().Where(o => o.CustomerID == c.CustomerID).DefaultIfEmpty()
                select new { c.ContactName, o.OrderDate }
                ).ToList();
            Assert.AreEqual(832, cods.Count);
        }

        [TestMethod]
        public void TestSelectWhereAssociation() {
            var ords = (
                from o in this.Northwind.List<Order>()
                where o.Customer.City == "Seattle"
                select o
                ).ToList();
            Assert.AreEqual(14, ords.Count);
        }

        [TestMethod]
        public void TestSelectWhereAssociationTwice() {
            var n = this.Northwind.List<Order>().Where(c => c.CustomerID == "WHITC").Count();
            var ords = (
                from o in this.Northwind.List<Order>()
                where o.Customer.Country == "USA" && o.Customer.City == "Seattle"
                select o
                ).ToList();
            Assert.AreEqual(n, ords.Count);
        }

        [TestMethod]
        public void TestSelectAssociation() {
            var custs = (
                from o in this.Northwind.List<Order>()
                where o.CustomerID == "ALFKI"
                select o.Customer
                ).ToList();
            Assert.AreEqual(6, custs.Count);
            Assert.IsTrue(custs.All(c => c.CustomerID == "ALFKI"));
        }

        [TestMethod]
        public void TestSelectAssociations() {
            var doubleCusts = (
                from o in this.Northwind.List<Order>()
                where o.CustomerID == "ALFKI"
                select new { A = o.Customer, B = o.Customer }
                ).ToList();

            Assert.AreEqual(6, doubleCusts.Count);
            Assert.IsTrue(doubleCusts.All(c => c.A.CustomerID == "ALFKI" && c.B.CustomerID == "ALFKI"));
        }

        [TestMethod]
        public void TestSelectAssociationsWhereAssociations() {
            var stuff = (
                from o in this.Northwind.List<Order>()
                where o.Customer.Country == "USA"
                where o.Customer.City != "Seattle"
                select new { A = o.Customer, B = o.Customer }
                ).ToList();
            Assert.AreEqual(108, stuff.Count);
        }

        [TestMethod]
        public void TestCustomersIncludeOrders() {
            NorthwindDataContext nw = new NorthwindDataContext(this.Northwind.Provider.New(new TestPolicy("Orders")));

            var custs = nw.List<Customer>().Where(c => c.CustomerID == "ALFKI").ToList();
            Assert.AreEqual(1, custs.Count);
            Assert.AreNotEqual(null, custs[0].Orders);
            Assert.AreEqual(6, custs[0].Orders.Count);
        }

        [TestMethod]
        public void TestCustomersIncludeOrdersAndDetails() {
            NorthwindDataContext nw = new NorthwindDataContext(this.Northwind.Provider.New(new TestPolicy("Orders", "Details")));

            var custs = nw.List<Customer>().Where(c => c.CustomerID == "ALFKI").ToList();
            Assert.AreEqual(1, custs.Count);
            Assert.AreNotEqual(null, custs[0].Orders);
            Assert.AreEqual(6, custs[0].Orders.Count);
            Assert.IsTrue(custs[0].Orders.Any(o => o.OrderID == 10643));
            Assert.AreNotEqual(null, custs[0].Orders.Single(o => o.OrderID == 10643).Details);
            Assert.AreEqual(3, custs[0].Orders.Single(o => o.OrderID == 10643).Details.Count);
        }
    }
}