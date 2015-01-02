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
using System.Linq;
using LinqToVfp.Northwind.Tests.NorthwindEntityProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToVfp.Northwind.Tests.EntityProviderTests {
    [TestClass]
    public class TranslationTests : ATranslationTests {
        [TestMethod]
        public void TestWhereWithCurrency() {
            var query = this.Northwind.Orders.Where(o => o.CustomerID == "VINET" && o.Freight == 32.38M);
            var list = query.ToList();
            TestQuery(query);
        }

        [TestMethod]
        public void TestWhereTrimStartChars() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.TrimStart("Lo".ToCharArray()) == "London"));
        }

        [TestMethod]
        public void TestWhereTrimStart() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.TrimStart() == "London"));
        }

        [TestMethod]
        public void TestWhereTrimEndChars() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.TrimEnd("on".ToCharArray()) == "London"));
        }

        [TestMethod]
        public void TestWhereTrimEnd() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.TrimEnd() == "London"));
        }

        [TestMethod]
        public void TestWhereBooleanColumnIsFalse() {
            TestQuery(this.Northwind.Products.Where(c => !c.Discontinued));
        }

        [TestMethod]
        public void TestWhereBooleanColumnIsTrue() {
            TestQuery(this.Northwind.Products.Where(c => c.Discontinued));
        }

        [TestMethod]
        public void TestWhere() {
            TestQuery(this.Northwind.Customers.Where(c => c.City == "London"));
        }

        [TestMethod]
        public void TestWhereTrue() {
            TestQuery(this.Northwind.Customers.Where(c => true));
        }

        [TestMethod]
        public void TestWhereFalse() {
            TestQuery(this.Northwind.Customers.Where(c => false));
        }

        [TestMethod]
        public void TestCompareEntityEqual() {
            Customer alfki = new Customer { CustomerID = "ALFKI" };
            TestQuery(
                this.Northwind.Customers.Where(c => c == alfki)
                );
        }

        [TestMethod]
        public void TestCompareEntityNotEqual() {
            Customer alfki = new Customer { CustomerID = "ALFKI" };
            TestQuery(
                this.Northwind.Customers.Where(c => c != alfki)
                );
        }

        [TestMethod]
        public void TestCompareConstructedEqual() {
            TestQuery(
                this.Northwind.Customers.Where(c => new { x = c.City } == new { x = "London" })
                );
        }

        [TestMethod]
        public void TestCompareConstructedMultiValueEqual() {
            TestQuery(
                this.Northwind.Customers.Where(c => new { x = c.City, y = c.Country } == new { x = "London", y = "UK" })
                );
        }

        [TestMethod]
        public void TestCompareConstructedMultiValueNotEqual() {
            TestQuery(
                this.Northwind.Customers.Where(c => new { x = c.City, y = c.Country } != new { x = "London", y = "UK" })
                );
        }

        [TestMethod]
        public void TestCompareConstructed() {
            TestQuery(
                this.Northwind.Customers.Where(c => new { x = c.City } == new { x = "London" })
                );
        }

        [TestMethod]
        public void TestSelectScalar() {
            TestQuery(this.Northwind.Customers.Select(c => c.City));
        }

        [TestMethod]
        public void TestSelectAnonymousOne() {
            TestQuery(this.Northwind.Customers.Select(c => new { c.City }));
        }

        [TestMethod]
        public void TestSelectAnonymousTwo() {
            TestQuery(this.Northwind.Customers.Select(c => new { c.City, c.Phone }));
        }

        [TestMethod]
        public void TestSelectAnonymousThree() {
            TestQuery(this.Northwind.Customers.Select(c => new { c.City, c.Phone, c.Country }));
        }

        [TestMethod]
        public void TestSelectCustomerTable() {
            TestQuery(this.Northwind.Customers);
        }

        [TestMethod]
        public void TestSelectCustomerIDentity() {
            TestQuery(this.Northwind.Customers.Select(c => c));
        }

        [TestMethod]
        public void TestSelectAnonymousWithObject() {
            TestQuery(this.Northwind.Customers.Select(c => new { c.City, c }));
        }

        [TestMethod]
        public void TestSelectAnonymousNested() {
            TestQuery(this.Northwind.Customers.Select(c => new { c.City, Country = new { c.Country } }));
        }

        [TestMethod]
        public void TestSelectAnonymousEmpty() {
            TestQuery(this.Northwind.Customers.Select(c => new { }));
        }

        [TestMethod]
        public void TestSelectAnonymousLiteral() {
            TestQuery(this.Northwind.Customers.Select(c => new { X = 10 }));
        }

        [TestMethod]
        public void TestSelectConstantInt() {
            TestQuery(this.Northwind.Customers.Select(c => 0));
        }

        [TestMethod]
        public void TestSelectConstantNullString() {
            TestQuery(this.Northwind.Customers.Select(c => (string)null));
        }

        [TestMethod]
        public void TestSelectLocal() {
            int x = 10;
            TestQuery(this.Northwind.Customers.Select(c => x));
        }

        [TestMethod]
        public void TestSelectNestedCollection() {
            TestQuery(
                from c in this.Northwind.Customers
                where c.City == "London"
                select this.Northwind.Orders.Where(o => o.CustomerID == c.CustomerID && o.OrderDate.Year == 1997).Select(o => o.OrderID)
                );
        }

        [TestMethod]
        public void TestSelectNestedCollectionInAnonymousType() {
            TestQuery(
                from c in this.Northwind.Customers
                where c.CustomerID == "ALFKI"
                select new { Foos = this.Northwind.Orders.Where(o => o.CustomerID == c.CustomerID && o.OrderDate.Year == 1997).Select(o => o.OrderID) }
                );
        }

        [TestMethod]
        public void TestJoinCustomerOrders() {
            TestQuery(
                from c in this.Northwind.Customers
                join o in this.Northwind.Orders on c.CustomerID equals o.CustomerID
                select new { c.ContactName, o.OrderID }
                );
        }

        [TestMethod]
        public void TestJoinMultiKey() {
            TestQuery(
                from c in this.Northwind.Customers
                join o in this.Northwind.Orders on new { a = c.CustomerID, b = c.CustomerID } equals new { a = o.CustomerID, b = o.CustomerID }
                select new { c, o }
                );
        }

        [TestMethod]
        public void TestJoinIntoCustomersOrders() {
            TestQuery(
                from c in this.Northwind.Customers
                join o in this.Northwind.Orders on c.CustomerID equals o.CustomerID into ords
                select new { cust = c, ords = ords.ToList() }
                );
        }

        [TestMethod]
        public void TestJoinIntoCustomersOrdersCount() {
            TestQuery(
                from c in this.Northwind.Customers
                join o in this.Northwind.Orders on c.CustomerID equals o.CustomerID into ords
                select new { cust = c, ords = ords.Count() }
                );
        }

        [TestMethod]
        public void TestJoinIntoDefaultIfEmpty() {
            TestQuery(
                from c in this.Northwind.Customers
                join o in this.Northwind.Orders on c.CustomerID equals o.CustomerID into ords
                from o in ords.DefaultIfEmpty()
                select new { c, o }
                );
        }

        [TestMethod]
        public void TestSelectManyCustomerOrders() {
            TestQuery(
                from c in this.Northwind.Customers
                from o in this.Northwind.Orders
                where c.CustomerID == o.CustomerID
                select new { c.ContactName, o.OrderID }
                );
        }

        [TestMethod]
        public void TestMultipleJoinsWithJoinConditionsInWhere() {
            // this should reduce to inner joins
            TestQuery(
                from c in this.Northwind.Customers
                from o in this.Northwind.Orders
                from d in this.Northwind.OrderDetails
                where o.CustomerID == c.CustomerID && o.OrderID == d.OrderID
                where c.CustomerID == "ALFKI"
                select d.ProductID
                );
        }

        [TestMethod]
        public void TestMultipleJoinsWithMissingJoinCondition() {
            // this should force a naked cross join
            TestQuery(
                from c in this.Northwind.Customers
                from o in this.Northwind.Orders
                from d in this.Northwind.OrderDetails
                where o.CustomerID == c.CustomerID /*&& o.OrderID == d.OrderID*/
                where c.CustomerID == "ALFKI"
                select d.ProductID
                );
        }

        [TestMethod]
        public void TestOrderBy() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.CustomerID)
                );
        }

        [TestMethod]
        public void TestOrderBySelect() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.CustomerID).Select(c => c.ContactName)
                );
        }

        [TestMethod]
        public void TestOrderByOrderBy() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.CustomerID).OrderBy(c => c.Country).Select(c => c.City)
                );
        }

        [TestMethod]
        public void TestOrderByThenBy() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.CustomerID).ThenBy(c => c.Country).Select(c => c.City)
                );
        }

        [TestMethod]
        public void TestOrderByDescending() {
            TestQuery(
                this.Northwind.Customers.OrderByDescending(c => c.CustomerID).Select(c => c.City)
                );
        }

        [TestMethod]
        public void TestOrderByDescendingThenBy() {
            TestQuery(
                this.Northwind.Customers.OrderByDescending(c => c.CustomerID).ThenBy(c => c.Country).Select(c => c.City)
                );
        }

        [TestMethod]
        public void TestOrderByDescendingThenByDescending() {
            TestQuery(
                this.Northwind.Customers.OrderByDescending(c => c.CustomerID).ThenByDescending(c => c.Country).Select(c => c.City)
                );
        }

        [TestMethod]
        public void TestOrderByJoin() {
            TestQuery(
                from c in this.Northwind.Customers.OrderBy(c => c.CustomerID)
                join o in this.Northwind.Orders.OrderBy(o => o.OrderID) on c.CustomerID equals o.CustomerID
                select new { c.CustomerID, o.OrderID }
                );
        }

        [TestMethod]
        public void TestOrderBySelectMany() {
            TestQuery(
                from c in this.Northwind.Customers.OrderBy(c => c.CustomerID)
                from o in this.Northwind.Orders.OrderBy(o => o.OrderID)
                where c.CustomerID == o.CustomerID
                select new { c.ContactName, o.OrderID }
                );
        }

        [TestMethod]
        public void TestGroupBy() {
            TestQuery(
                this.Northwind.Customers.GroupBy(c => c.City)
                );
        }

        [TestMethod]
        public void TestGroupBySelectMany() {
            TestQuery(
                this.Northwind.Customers.GroupBy(c => c.City).SelectMany(g => g)
                );
        }

        [TestMethod]
        public void TestGroupBySum() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID).Select(g => g.Sum(o => o.OrderID))
                );
        }

        [TestMethod]
        public void TestGroupByCount() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID).Select(g => g.Count())
                );
        }

        [TestMethod]
        public void TestGroupByLongCount() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID).Select(g => g.LongCount())
                );
        }

        [TestMethod]
        public void TestGroupBySumMinMaxAvg() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID).Select(g =>
                    new {
                        Sum = g.Sum(o => o.OrderID),
                        Min = g.Min(o => o.OrderID),
                        Max = g.Max(o => o.OrderID),
                        Avg = g.Average(o => o.OrderID)
                    })
                );
        }

        [TestMethod]
        public void TestGroupByWithResultSelector() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID, (k, g) =>
                    new {
                        Sum = g.Sum(o => o.OrderID),
                        Min = g.Min(o => o.OrderID),
                        Max = g.Max(o => o.OrderID),
                        Avg = g.Average(o => o.OrderID)
                    })
                );
        }

        [TestMethod]
        public void TestGroupByWithElementSelectorSum() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID, o => o.OrderID).Select(g => g.Sum())
                );
        }

        [TestMethod]
        public void TestGroupByWithElementSelector() {
            // note: groups are retrieved through a separately execute subquery per row
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID, o => o.OrderID)
                );
        }

        [TestMethod]
        public void TestGroupByWithElementSelectorSumMax() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID, o => o.OrderID).Select(g => new { Sum = g.Sum(), Max = g.Max() })
                );
        }

        [TestMethod]
        public void TestGroupByWithAnonymousElement() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID, o => new { o.OrderID }).Select(g => g.Sum(x => x.OrderID))
                );
        }

        [TestMethod]
        public void TestGroupByWithTwoPartKey() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => new { o.CustomerID, o.OrderDate }).Select(g => g.Sum(o => o.OrderID))
                );
        }

        [TestMethod]
        public void TestOrderByGroupBy() {
            // note: order-by is lost when group-by is applied (the sequence of groups is not ordered)
            TestQuery(
                this.Northwind.Orders.OrderBy(o => o.OrderID).GroupBy(o => o.CustomerID).Select(g => g.Sum(o => o.OrderID))
                );
        }

        [TestMethod]
        public void TestOrderByGroupBySelectMany() {
            // note: order-by is preserved within grouped sub-collections
            TestQuery(
                this.Northwind.Orders.OrderBy(o => o.OrderID).GroupBy(o => o.CustomerID).SelectMany(g => g)
                );
        }

        [TestMethod]
        public void TestSumWithNoArg() {
            TestQuery(
                () => this.Northwind.Orders.Select(o => o.OrderID).Sum()
                );
        }

        [TestMethod]
        public void TestSumWithArg() {
            TestQuery(
                () => this.Northwind.Orders.Sum(o => o.OrderID)
                );
        }

        [TestMethod]
        public void TestCountWithNoPredicate() {
            TestQuery(
                () => this.Northwind.Orders.Count()
                );
        }

        [TestMethod]
        public void TestCountWithPredicate() {
            TestQuery(
                () => this.Northwind.Orders.Count(o => o.CustomerID == "ALFKI")
                );
        }

        [TestMethod]
        public void TestDistinct() {
            TestQuery(
                this.Northwind.Customers.Distinct()
                );
        }

        [TestMethod]
        public void TestDistinctScalar() {
            TestQuery(
                this.Northwind.Customers.Select(c => c.City).Distinct()
                );
        }

        [TestMethod]
        public void TestOrderByDistinct() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.CustomerID).Select(c => c.City).Distinct()
                );
        }

        [TestMethod]
        public void TestDistinctOrderBy() {
            TestQuery(
                this.Northwind.Customers.Select(c => c.City).Distinct().OrderBy(c => c)
                );
        }

        [TestMethod]
        public void TestDistinctGroupBy() {
            TestQuery(
                this.Northwind.Orders.Distinct().GroupBy(o => o.CustomerID)
                );
        }

        [TestMethod]
        public void TestGroupByDistinct() {
            TestQuery(
                this.Northwind.Orders.GroupBy(o => o.CustomerID).Distinct()
                );

        }

        [TestMethod]
        public void TestDistinctCount() {
            TestQuery(
                () => this.Northwind.Customers.Distinct().Count()
                );
        }

        [TestMethod]
        public void TestSelectDistinctCount() {
            // cannot do: SELECT COUNT(DISTINCT some-colum) FROM some-table
            // because COUNT(DISTINCT some-column) does not count nulls
            TestQuery(
                () => this.Northwind.Customers.Select(c => c.City).Distinct().Count()
                );
        }

        [TestMethod]
        public void TestSelectSelectDistinctCount() {
            TestQuery(
                () => this.Northwind.Customers.Select(c => c.City).Select(c => c).Distinct().Count()
                );
        }

        [TestMethod]
        public void TestDistinctCountPredicate() {
            TestQuery(
                () => this.Northwind.Customers.Distinct().Count(c => c.CustomerID == "ALFKI")
                );
        }

        [TestMethod]
        public void TestDistinctSumWithArg() {
            TestQuery(
                () => this.Northwind.Orders.Distinct().Sum(o => o.OrderID)
                );
        }

        [TestMethod]
        public void TestSelectDistinctSum() {
            TestQuery(
                () => this.Northwind.Orders.Select(o => o.OrderID).Distinct().Sum()
                );
        }

        [TestMethod]
        public void TestTake() {
            TestQuery(
                this.Northwind.Orders.Take(5).OrderBy(d => d.OrderID)
                );
        }

        [TestMethod]
        public void TestTakeDistinct() {
            // distinct must be forced to apply after top has been computed
            TestQuery(
                this.Northwind.Orders.Take(5).OrderBy(d => d.OrderID).Distinct()
                );
        }

        [TestMethod]
        public void TestDistinctTake() {
            // top must be forced to apply after distinct has been computed
            TestQuery(
                this.Northwind.Orders.Distinct().Take(5).OrderBy(d => d.OrderID)
                );
        }

        [TestMethod]
        public void TestDistinctTakeCount() {
            TestQuery(
                () => this.Northwind.Orders.Distinct().OrderBy(d => d.OrderID).Take(5).Count()
                );
        }

        [TestMethod]
        public void TestTakeDistinctCount() {
            TestQuery(
                () => this.Northwind.Orders.Take(5).OrderBy(d => d.OrderID).Distinct().Count()
                );
        }

        [TestMethod]
        public void TestSkip() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Skip(5)
                );
        }

        [TestMethod]
        public void TestTakeSkip() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Take(10).Skip(5)
                );
        }

        [TestMethod]
        public void TestDistinctSkip() {
            TestQuery(
                this.Northwind.Customers.Distinct().OrderBy(c => c.ContactName).Skip(5)
                );
        }

        [TestMethod]
        public void TestSkipTake() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Skip(5).Take(10)
                );
        }

        [TestMethod]
        public void TestDistinctSkipTake() {
            TestQuery(
                this.Northwind.Customers.Distinct().OrderBy(c => c.ContactName).Skip(5).Take(10)
                );
        }

        [TestMethod]
        public void TestSkipDistinct() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Skip(5).Distinct()
                );
        }

        [TestMethod]
        public void TestSkipTakeDistinct() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Skip(5).Take(10).Distinct()
                );
        }

        [TestMethod]
        public void TestTakeSkipDistinct() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Take(10).Skip(5).Distinct()
                );
        }

        [TestMethod]
        public void TestFirst() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).First()
                );
        }

        [TestMethod]
        public void TestFirstPredicate() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).First(c => c.City == "London")
                );
        }

        [TestMethod]
        public void TestWhereFirst() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").First()
                );
        }

        [TestMethod]
        public void TestFirstOrDefault() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).FirstOrDefault()
                );
        }

        [TestMethod]
        public void TestFirstOrDefaultPredicate() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).FirstOrDefault(c => c.City == "London")
                );
        }

        [TestMethod]
        public void TestWhereFirstOrDefault() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").FirstOrDefault()
                );
        }

        [TestMethod]
        public void TestReverse() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Reverse()
                );
        }

        [TestMethod]
        public void TestReverseReverse() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Reverse().Reverse()
                );
        }

        [TestMethod]
        public void TestReverseWhereReverse() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Reverse().Where(c => c.City == "London").Reverse()
                );
        }

        [TestMethod]
        public void TestReverseTakeReverse() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Reverse().Take(5).Reverse()
                );
        }

        [TestMethod]
        public void TestReverseWhereTakeReverse() {
            TestQuery(
                this.Northwind.Customers.OrderBy(c => c.ContactName).Reverse().Where(c => c.City == "London").Take(5).Reverse()
                );
        }

        [TestMethod]
        public void TestLast() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).Last()
                );
        }

        [TestMethod]
        public void TestLastPredicate() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).Last(c => c.City == "London")
                );
        }

        [TestMethod]
        public void TestWhereLast() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").Last()
                );
        }

        [TestMethod]
        public void TestLastOrDefault() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).LastOrDefault()
                );
        }

        [TestMethod]
        public void TestLastOrDefaultPredicate() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).LastOrDefault(c => c.City == "London")
                );
        }

        [TestMethod]
        public void TestWhereLastOrDefault() {
            TestQuery(
                () => this.Northwind.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").LastOrDefault()
                );
        }

        [TestMethod]
        public void TestSingle() {
            TestQueryFails(
                () => this.Northwind.Customers.Single()
                );
        }

        [TestMethod]
        public void TestSinglePredicate() {
            TestQuery(
                () => this.Northwind.Customers.Single(c => c.CustomerID == "ALFKI")
                );
        }

        [TestMethod]
        public void TestWhereSingle() {
            TestQuery(
                () => this.Northwind.Customers.Where(c => c.CustomerID == "ALFKI").Single()
                );
        }

        [TestMethod]
        public void TestSingleOrDefault() {
            TestQueryFails(
                () => this.Northwind.Customers.SingleOrDefault()
                );
        }

        [TestMethod]
        public void TestSingleOrDefaultPredicate() {
            TestQuery(
                () => this.Northwind.Customers.SingleOrDefault(c => c.CustomerID == "ALFKI")
                );
        }

        [TestMethod]
        public void TestWhereSingleOrDefault() {
            TestQuery(
                () => this.Northwind.Customers.Where(c => c.CustomerID == "ALFKI").SingleOrDefault()
                );
        }

        [TestMethod]
        public void TestAnyWithSubquery() {
            TestQuery(
                this.Northwind.Customers.Where(c => this.Northwind.Orders.Where(o => o.CustomerID == c.CustomerID).Any(o => o.OrderDate.Year == 1997))
                );
        }

        [TestMethod]
        public void TestAnyWithSubqueryNoPredicate() {
            TestQuery(
                this.Northwind.Customers.Where(c => this.Northwind.Orders.Where(o => o.CustomerID == c.CustomerID).Any())
                );
        }

        [TestMethod]
        public void TestAnyWithLocalCollection() {
            string[] ids = new[] { "ABCDE", "ALFKI" };
            TestQuery(
                this.Northwind.Customers.Where(c => ids.Any(id => c.CustomerID == id))
                );
        }

        [TestMethod]
        public void TestAnyTopLevel() {
            TestQuery(
                () => this.Northwind.Customers.Any()
                );
        }

        [TestMethod]
        public void TestAllWithSubquery() {
            TestQuery(
                this.Northwind.Customers.Where(c => this.Northwind.Orders.Where(o => o.CustomerID == c.CustomerID).All(o => o.OrderDate.Year == 1997))
                );
        }

        [TestMethod]
        public void TestAllWithLocalCollection() {
            string[] patterns = new[] { "a", "e" };

            TestQuery(
                this.Northwind.Customers.Where(c => patterns.All(p => c.ContactName.Contains(p)))
                );
        }

        [TestMethod]
        public void TestAllTopLevel() {
            TestQuery(
                () => this.Northwind.Customers.All(c => c.ContactName.StartsWith("a"))
                );
        }

        [TestMethod]
        public void TestContainsWithSubquery() {
            TestQuery(
                this.Northwind.Customers.Where(c => this.Northwind.Orders.Select(o => o.CustomerID).Contains(c.CustomerID))
                );
        }

        [TestMethod]
        public void TestContainsWithLocalCollection() {
            string[] ids = new[] { "ABCDE", "ALFKI" };
            TestQuery(
                this.Northwind.Customers.Where(c => ids.Contains(c.CustomerID))
                );
        }

        [TestMethod]
        public void TestContainsTopLevel() {
            TestQuery(
                () => this.Northwind.Customers.Select(c => c.CustomerID).Contains("ALFKI")
                );
        }

        [TestMethod]
        public void TestCoalesce() {
            TestQuery(this.Northwind.Customers.Where(c => (c.City ?? "Seattle") == "Seattle"));
        }

        [TestMethod]
        public void TestCoalesce2() {
            TestQuery(this.Northwind.Customers.Where(c => (c.City ?? c.Country ?? "Seattle") == "Seattle"));
        }


        // framework function tests
        [TestMethod]
        public void TestStringLength() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Length == 7));
        }

        [TestMethod]
        public void TestStringStartsWithLiteral() {
            TestQuery(this.Northwind.Customers.Where(c => c.ContactName.StartsWith("M")));
        }

        [TestMethod]
        public void TestStringStartsWithColumn() {
            TestQuery(this.Northwind.Customers.Where(c => c.ContactName.StartsWith(c.ContactName)));
        }

        [TestMethod]
        public void TestStringEndsWithLiteral() {
            TestQuery(this.Northwind.Customers.Where(c => c.ContactName.EndsWith("s")));
        }

        [TestMethod]
        public void TestStringEndsWithColumn() {
            TestQuery(this.Northwind.Customers.Where(c => c.ContactName.EndsWith(c.ContactName)));
        }

        [TestMethod]
        public void TestStringContainsLiteral() {
            TestQuery(this.Northwind.Customers.Where(c => c.ContactName.Contains("and")));
        }

        [TestMethod]
        public void TestStringContainsColumn() {
            TestQuery(this.Northwind.Customers.Where(c => c.ContactName.Contains(c.ContactName)));
        }

        [TestMethod]
        public void TestStringConcatImplicit2Args() {
            TestQuery(this.Northwind.Customers.Where(c => c.ContactName + "X" == "X"));
        }

        [TestMethod]
        public void TestStringConcatExplicit2Args() {
            TestQuery(this.Northwind.Customers.Where(c => string.Concat(c.ContactName, "X") == "X"));
        }

        [TestMethod]
        public void TestStringConcatExplicit3Args() {
            TestQuery(this.Northwind.Customers.Where(c => string.Concat(c.ContactName, "X", c.Country) == "X"));
        }

        [TestMethod]
        public void TestStringConcatExplicitNArgs() {
            TestQuery(this.Northwind.Customers.Where(c => string.Concat(new string[] { c.ContactName, "X", c.Country }) == "X"));
        }

        [TestMethod]
        public void TestStringIsNullOrEmpty() {
            TestQuery(this.Northwind.Customers.Where(c => string.IsNullOrEmpty(c.City)));
        }

        [TestMethod]
        public void TestStringToUpper() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.ToUpper() == "SEATTLE"));
        }

        [TestMethod]
        public void TestStringToLower() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.ToLower() == "seattle"));
        }

        [TestMethod]
        public void TestStringSubstring() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Substring(0, 4) == "Seat"));
        }

        [TestMethod]
        public void TestStringSubstringNoLength() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Substring(4) == "tle"));
        }

        [TestMethod]
        public void TestStringIndexOf() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.IndexOf("tt") == 4));
        }

        [TestMethod]
        public void TestStringIndexOfChar() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.IndexOf('t') == 4));
        }

        [TestMethod]
        public void TestStringTrim() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Trim() == "Seattle"));
        }

        [TestMethod]
        public void TestStringToString() {
            // just to prove this is a no op
            TestQuery(this.Northwind.Customers.Where(c => c.City.ToString() == "Seattle"));
        }

        [TestMethod]
        public void TestStringReplace() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Replace("ea", "ae") == "Saettle"));
        }

        [TestMethod]
        public void TestStringReplaceChars() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Replace("e", "y") == "Syattly"));
        }

        [TestMethod]
        public void TestStringRemove() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Remove(1, 2) == "Sttle"));
        }

        [TestMethod]
        public void TestStringRemoveNoCount() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.Remove(4) == "Seat"));
        }

        [TestMethod]
        public void TestMathAbs() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Abs(o.OrderID) == 10));
        }

        [TestMethod]
        public void TestMathAtan() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Atan(o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathCos() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Cos(o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathSin() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Sin(o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathTan() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Tan(o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathExp() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Exp(o.OrderID < 1000 ? 1 : 2) == 0));
        }

        [TestMethod]
        public void TestMathLog() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Log(o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathSqrt() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Sqrt(o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathPow() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Pow(o.OrderID < 1000 ? 1 : 2, 3) == 0));
        }

        [TestMethod]
        public void TestMathRoundDefault() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Round((decimal)o.OrderID) == 0));
        }

        //[TestMethod]
        //public void TestMathAcos() {
        //    TestQuery(this.Northwind.Orders.Where(o => Math.Acos(o.OrderID) == 0));
        //}

        //[TestMethod]
        //public void TestMathAsin() {
        //    TestQuery(this.Northwind.Orders.Where(o => Math.Asin(o.OrderID) == 0));
        //}

        [TestMethod]
        public void TestMathAtan2() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Atan2(o.OrderID, 3) == 0));
        }

        [TestMethod]
        public void TestMathLog10() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Log10(o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathCeiling() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Ceiling((double)o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathRoundToPlace() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Round((decimal)o.OrderID, 2) == 0));
        }

        [TestMethod]
        public void TestMathFloor() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Floor((double)o.OrderID) == 0));
        }

        [TestMethod]
        public void TestMathTruncate() {
            TestQuery(this.Northwind.Orders.Where(o => Math.Truncate((double)o.OrderID) == 0));
        }

        [TestMethod]
        public void TestStringCompareToLT() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.CompareTo("Seattle") < 0));
        }

        [TestMethod]
        public void TestStringCompareToLE() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.CompareTo("Seattle") <= 0));
        }

        [TestMethod]
        public void TestStringCompareToGT() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.CompareTo("Seattle") > 0));
        }

        [TestMethod]
        public void TestStringCompareToGE() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.CompareTo("Seattle") >= 0));
        }

        [TestMethod]
        public void TestStringCompareToEQ() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.CompareTo("Seattle") == 0));
        }

        [TestMethod]
        public void TestStringCompareToNE() {
            TestQuery(this.Northwind.Customers.Where(c => c.City.CompareTo("Seattle") != 0));
        }

        [TestMethod]
        public void TestStringCompareLT() {
            TestQuery(this.Northwind.Customers.Where(c => string.Compare(c.City, "Seattle") < 0));
        }

        [TestMethod]
        public void TestStringCompareLE() {
            TestQuery(this.Northwind.Customers.Where(c => string.Compare(c.City, "Seattle") <= 0));
        }

        [TestMethod]
        public void TestStringCompareGT() {
            TestQuery(this.Northwind.Customers.Where(c => string.Compare(c.City, "Seattle") > 0));
        }

        [TestMethod]
        public void TestStringCompareGE() {
            TestQuery(this.Northwind.Customers.Where(c => string.Compare(c.City, "Seattle") >= 0));
        }

        [TestMethod]
        public void TestStringCompareEQ() {
            TestQuery(this.Northwind.Customers.Where(c => string.Compare(c.City, "Seattle") == 0));
        }

        [TestMethod]
        public void TestStringCompareNE() {
            TestQuery(this.Northwind.Customers.Where(c => string.Compare(c.City, "Seattle") != 0));
        }

        [TestMethod]
        public void TestIntCompareTo() {
            // prove that x.CompareTo(y) works for types other than string
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID.CompareTo(1000) == 0));
        }

        [TestMethod]
        public void TestDecimalCompare() {
            // prove that type.Compare(x,y) works with decimal
            TestQuery(this.Northwind.Orders.Where(o => decimal.Compare((decimal)o.OrderID, 0.0m) == 0));
        }

        [TestMethod]
        public void TestDecimalAdd() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Add(o.OrderID, 0.0m) == 0.0m));
        }

        [TestMethod]
        public void TestDecimalSubtract() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Subtract(o.OrderID, 0.0m) == 0.0m));
        }

        [TestMethod]
        public void TestDecimalMultiply() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Multiply(o.OrderID, 1.0m) == 1.0m));
        }

        [TestMethod]
        public void TestDecimalDivide() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Divide(o.OrderID, 1.0m) == 1.0m));
        }

        [TestMethod]
        public void TestDecimalRemainder() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Remainder(o.OrderID, 1.0m) == 0.0m));
        }

        [TestMethod]
        public void TestDecimalNegate() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Negate(o.OrderID) == 1.0m));
        }

        [TestMethod]
        public void TestDecimalRoundDefault() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Round(o.OrderID) == 0m));
        }

        [TestMethod]
        public void TestDecimalRoundPlaces() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Round(o.OrderID, 2) == 0.00m));
        }

        [TestMethod]
        public void TestDecimalTruncate() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Truncate(o.OrderID) == 0m));
        }

        [TestMethod]
        public void TestDecimalCeiling() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Ceiling(o.OrderID) == 0.0m));
        }

        [TestMethod]
        public void TestDecimalFloor() {
            TestQuery(this.Northwind.Orders.Where(o => decimal.Floor(o.OrderID) == 0.0m));
        }

        [TestMethod]
        public void TestDecimalLT() {
            // prove that decimals are treated normally with respect to normal comparison operators
            TestQuery(this.Northwind.Orders.Where(o => ((decimal)o.OrderID) < 0.0m));
        }

        [TestMethod]
        public void TestIntLessThan() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID < 0));
        }

        [TestMethod]
        public void TestIntLessThanOrEqual() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID <= 0));
        }

        [TestMethod]
        public void TestIntGreaterThan() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID > 0));
        }

        [TestMethod]
        public void TestIntGreaterThanOrEqual() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID >= 0));
        }

        [TestMethod]
        public void TestIntEqual() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID == 0));
        }

        [TestMethod]
        public void TestIntNotEqual() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID != 0));
        }

        [TestMethod]
        public void TestIntAdd() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID + 0 == 0));
        }

        [TestMethod]
        public void TestIntSubtract() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID - 0 == 0));
        }

        [TestMethod]
        public void TestIntMultiply() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID * 1 == 1));
        }

        [TestMethod]
        public void TestIntDivide() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID / 1 == 1));
        }

        [TestMethod]
        public void TestIntModulo() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID % 1 == 0));
        }

        [TestMethod]
        public void TestIntLeftShift() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID << 1 == 0));
        }

        [TestMethod]
        public void TestIntRightShift() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID >> 1 == 0));
        }

        [TestMethod]
        public void TestIntBitwiseAnd() {
            TestQuery(this.Northwind.Orders.Where(o => (o.OrderID & 1) == 0));
        }

        [TestMethod]
        public void TestIntBitwiseOr() {
            TestQuery(this.Northwind.Orders.Where(o => (o.OrderID | 1) == 1));
        }

        [TestMethod]
        public void TestIntBitwiseExclusiveOr() {
            TestQuery(this.Northwind.Orders.Where(o => (o.OrderID ^ 1) == 1));
        }

        [TestMethod]
        public void TestIntBitwiseNot() {
            TestQuery(this.Northwind.Orders.Where(o => ~o.OrderID == 0));
        }

        [TestMethod]
        public void TestIntNegate() {
            TestQuery(this.Northwind.Orders.Where(o => -o.OrderID == -1));
        }

        [TestMethod]
        public void TestAnd() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID > 0 && o.OrderID < 2000));
        }

        [TestMethod]
        public void TestOr() {
            TestQuery(this.Northwind.Orders.Where(o => o.OrderID < 5 || o.OrderID > 10));
        }

        [TestMethod]
        public void TestNot() {
            TestQuery(this.Northwind.Orders.Where(o => !(o.OrderID == 0)));
        }

        [TestMethod]
        public void TestEqualNull() {
            TestQuery(this.Northwind.Customers.Where(c => c.City == null));
        }

        [TestMethod]
        public void TestEqualNullReverse() {
            TestQuery(this.Northwind.Customers.Where(c => null == c.City));
        }

        [TestMethod]
        public void TestConditional() {
            TestQuery(this.Northwind.Orders.Where(o => (o.CustomerID == "ALFKI" ? 1000 : 0) == 1000));
        }

        [TestMethod]
        public void TestConditional2() {
            TestQuery(this.Northwind.Orders.Where(o => (o.CustomerID == "ALFKI" ? 1000 : o.CustomerID == "ABCDE" ? 2000 : 0) == 1000));
        }

        //[TestMethod]
        //public void TestConditionalTestIsValue() {
        //    TestQuery(this.Northwind.Orders.Where(o => (((bool)(object)o.OrderID) ? 100 : 200) == 100));
        //}

        [TestMethod]
        public void TestConditionalResultsArePredicates() {
            TestQuery(this.Northwind.Orders.Where(o => (o.CustomerID == "ALFKI" ? o.OrderID < 10 : o.OrderID > 10)));
        }

        [TestMethod]
        public void TestSelectManyJoined() {
            TestQuery(
                from c in this.Northwind.Customers
                from o in this.Northwind.Orders.Where(o => o.CustomerID == c.CustomerID)
                select new { c.ContactName, o.OrderDate }
                );
        }

        [TestMethod]
        public void TestSelectManyJoinedDefaultIfEmpty() {
            TestQuery(
                from c in this.Northwind.Customers
                from o in this.Northwind.Orders.Where(o => o.CustomerID == c.CustomerID).DefaultIfEmpty()
                select new { c.ContactName, o.OrderDate }
                );
        }

        [TestMethod]
        public void TestSelectWhereAssociation() {
            TestQuery(
                from o in this.Northwind.Orders
                where o.Customer.City == "Seattle"
                select o
                );
        }

        [TestMethod]
        public void TestSelectWhereAssociations() {
            TestQuery(
                from o in this.Northwind.Orders
                where o.Customer.City == "Seattle" && o.Customer.Phone != "555 555 5555"
                select o
                );
        }

        [TestMethod]
        public void TestSelectWhereAssociationTwice() {
            TestQuery(
                from o in this.Northwind.Orders
                where o.Customer.City == "Seattle" && o.Customer.Phone != "555 555 5555"
                select o
                );
        }

        [TestMethod]
        public void TestSelectAssociation() {
            TestQuery(
                from o in this.Northwind.Orders
                select o.Customer
                );
        }

        [TestMethod]
        public void TestSelectAssociations() {
            TestQuery(
                from o in this.Northwind.Orders
                select new { A = o.Customer, B = o.Customer }
                );
        }

        [TestMethod]
        public void TestSelectAssociationsWhereAssociations() {
            TestQuery(
                from o in this.Northwind.Orders
                where o.Customer.City == "Seattle"
                where o.Customer.Phone != "555 555 5555"
                select new { A = o.Customer, B = o.Customer }
                );
        }

        [TestMethod]
        public void TestCustomersIncludeOrders() {
            NorthwindDataContext nw = new NorthwindDataContext(this.Northwind.Provider.New(new TestPolicy("Orders")));

            TestQuery(
                nw.Customers
                );
        }

        [TestMethod]
        public void TestCustomersWhereIncludeOrders() {
            NorthwindDataContext nw = new NorthwindDataContext(this.Northwind.Provider.New(new TestPolicy("Orders")));

            TestQuery(
                from c in nw.Customers
                where c.City == "London"
                select c
                );
        }

        [TestMethod]
        public void TestCustomersIncludeOrdersAndDetails() {
            NorthwindDataContext nw = new NorthwindDataContext(this.Northwind.Provider.New(new TestPolicy("Orders", "Details")));

            TestQuery(
                nw.Customers
                );
        }

        [TestMethod]
        public void TestCustomersWhereIncludeOrdersAndDetails() {
            NorthwindDataContext nw = new NorthwindDataContext(this.Northwind.Provider.New(new TestPolicy("Orders", "Details")));

            TestQuery(
                from c in nw.Customers
                where c.City == "London"
                select c
                );
        }

        //[TestMethod]
        //public void TestInterfaceElementTypeAsGenericConstraint() {
        //    TestQuery(
        //        GetById(this.Northwind.Products, 5)
        //        );
        //}

        //private static IQueryable<T> GetById<T>(IQueryable<T> query, int id) where T : IEntity {
        //    return query.Where(x => x.ID == id);
        //}

        [TestMethod]
        public void TestXmlMappingSelectCustomers() {
            var nw = new NorthwindDataContext(this.Northwind.Provider.New(VfpXmlMapping.FromXml(Properties.Resources.NorthwindXml)));

            TestQuery(
                from c in this.Northwind.Customers
                where c.City == "London"
                select c.ContactName
                );
        }

        [TestMethod]
        public void TestSingletonAssociationWithMemberAccess() {
            TestQuery(
                from o in this.Northwind.Orders
                where o.Customer.City == "Seattle"
                where o.Customer.Phone != "555 555 5555"
                select new { A = o.Customer, B = o.Customer.City }
                );
        }

        [TestMethod]
        public void TestCompareDateTimesWithDifferentNullability() {
            TestQuery(
                from o in this.Northwind.Orders
                where o.OrderDate < DateTime.Today && ((DateTime?)o.OrderDate) < DateTime.Today
                select o
                );
        }

        [TestMethod]
        public void TestContainsWithEmptyLocalList() {
            var ids = new string[0];
            TestQuery(
                from c in this.Northwind.Customers
                where ids.Contains(c.CustomerID)
                select c
                );
        }

        [TestMethod]
        public void TestContainsWithSubQuery() {
            var custsInLondon = this.Northwind.Customers.Where(c => c.City == "London").Select(c => c.CustomerID);

            TestQuery(
                from c in this.Northwind.Customers
                where custsInLondon.Contains(c.CustomerID)
                select c
                );
        }

        [TestMethod]
        public void TestCombineQueriesDeepNesting() {
            var custs = this.Northwind.Customers.Where(c => c.ContactName.StartsWith("xxx"));
            var ords = this.Northwind.Orders.Where(o => custs.Any(c => c.CustomerID == o.CustomerID));
            TestQuery(
                this.Northwind.OrderDetails.Where(d => ords.Any(o => o.OrderID == d.OrderID))
                );
        }
    }
}

