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
using System.Linq;
using IQToolkit;
using LinqToVfp.Northwind.Tests.NorthwindEntityProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToVfp.Northwind.Tests.EntityProviderTests {
    [TestClass]
    public class CUDTests : AEntityProviderTests {
        [TestCleanup]
        public void TestCleanup() {
            CopyData(BackupPath, DbcPath);
        }

        [TestMethod]
        public void TestPack() {
            var context = new NorthwindDataContext(Path.GetFullPath("Northwind.dbc"));
            context.Customers.Delete(context.Customers.Where(x => x.CustomerID == "PARIS").OrderBy(x => x.CustomerID).Last());
            Assert.AreEqual(90, context.Customers.Count());
            var connectionString = @"Provider=VFPOLEDB;Data Source=" + Path.GetFullPath("Northwind.dbc") + ";Deleted=false;";
            context = new NorthwindDataContext(connectionString);
            Assert.AreEqual(91, context.Customers.Count());
            context.Provider.Pack(context.Customers);
            Assert.AreEqual(90, context.Customers.Count());
        }
        [TestMethod]
        public void TestZap() {
            var context = new NorthwindDataContext(Path.GetFullPath("Northwind.dbc"));
            Assert.IsTrue(context.Customers.Any());
            context.Provider.Zap(context.Customers);
            Assert.IsFalse(context.Customers.Any());
        }

        [TestMethod]
        public void TestInsertCustomerNoResultWithRollbackTransaction() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            this.Northwind.Provider.Connection.Open();

            this.Northwind.Provider.Transaction = this.Northwind.Provider.Connection.BeginTransaction();
            var result = this.Northwind.Customers.Insert(cust);
            this.Northwind.Provider.Transaction.Rollback();
            this.Northwind.Provider.Connection.Close();
            Assert.AreEqual(1, result);  // returns 1 for success

            var cust2 = this.Northwind.Customers.GetById(cust.CustomerID);
            Assert.IsNull(cust2);
        }

        [TestMethod]
        public void TestInsertCustomerNoResultWithCommitTransaction() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            this.Northwind.Provider.Connection.Open();

            this.Northwind.Provider.Transaction = this.Northwind.Provider.Connection.BeginTransaction();
            var result = this.Northwind.Customers.Insert(cust);
            this.Northwind.Provider.Transaction.Commit();
            this.Northwind.Provider.Connection.Close();
            Assert.AreEqual(1, result);  // returns 1 for success

            var cust2 = this.Northwind.Customers.GetById(cust.CustomerID);
            Assert.AreEqual(cust.CustomerID, cust2.CustomerID);
        }

        [TestMethod]
        public void TestInsertCustomerNoResult() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };
            var result = this.Northwind.Customers.Insert(cust);
            Assert.AreEqual(1, result);  // returns 1 for success
        }

        [TestMethod]
        public void TestInsertCustomerWithResult() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };
            var result = this.Northwind.Customers.Insert(cust, c => c.City);
            Assert.AreEqual(result, "Seattle");  // should be value we asked for
        }

        [TestMethod]
        public void TestBatchInsertCustomersNoResult() {
            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Seattle",
                    Country = "USA"
                });
            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.Insert(c));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchInsertCustomersWithResult() {
            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Seattle",
                    Country = "USA"
                });
            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.Insert(c, d => d.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, "Seattle")));
        }

        [TestMethod]
        public void TestInsertOrderWithNoResult() {
            this.TestInsertCustomerNoResult(); // create customer "XX1"
            var order = new Order {
                CustomerID = "XX1",
                OrderDate = DateTime.Today,
            };
            var result = this.Northwind.Orders.Insert(order);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestInsertOrderWithGeneratedIDResult() {
            CopyData(BackupPath, DbcPath);
            this.TestInsertCustomerNoResult(); // create customer "XX1"
            var order = new Order {
                CustomerID = "XX1",
                OrderDate = DateTime.Today,
            };

            var result = this.Northwind.Orders.Insert(order, o => o.OrderID);
            order.OrderDate = DateTime.Today.AddDays(1);
            this.Northwind.Orders.Update(order);
            Assert.AreNotEqual(1, result);
        }

        [TestMethod]
        public void TestUpdateCustomerNoResult() {
            this.TestInsertCustomerNoResult(); // create customer "XX1"

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.Update(cust);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestUpdateCustomerWithResult() {
            this.TestInsertCustomerNoResult(); // create customer "XX1"

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.Update(cust, null, c => c.City);
            Assert.AreEqual("Portland", result);
        }

        [TestMethod]
        public void TestUpdateCustomerWithUpdateCheckThatDoesNotSucceed() {
            this.TestInsertCustomerNoResult(); // create customer "XX1"

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.Update(cust, d => d.City == "Detroit");
            Assert.AreEqual(0, result); // 0 for failure
        }

        [TestMethod]
        public void TestUpdateCustomerWithUpdateCheckThatSucceeds() {
            this.TestInsertCustomerNoResult(); // create customer "XX1"

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.Update(cust, d => d.City == "Seattle");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestBatchUpdateCustomer() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Seattle",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.Update(c));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchUpdateCustomerWithUpdateCheck() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var pairs = Enumerable.Range(1, n).Select(
                i => new {
                    original = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Seattle",
                        Country = "USA"
                    },
                    current = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Portland",
                        Country = "USA"
                    }
                });

            var results = this.Northwind.Customers.Batch(pairs, (u, x) => u.Update(x.current, d => d.City == x.original.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchUpdateCustomerWithResult() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Portland",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.Update(c, null, d => d.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, "Portland")));
        }

        [TestMethod]
        public void TestBatchUpdateCustomerWithUpdateCheckAndResult() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var pairs = Enumerable.Range(1, n).Select(
                i => new {
                    original = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Seattle",
                        Country = "USA"
                    },
                    current = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Portland",
                        Country = "USA"
                    }
                });

            var results = this.Northwind.Customers.Batch(pairs, (u, x) => u.Update(x.current, d => d.City == x.original.City, d => d.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, "Portland")));
        }

        [TestMethod]
        public void TestUpsertNewCustomerNoResult() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.InsertOrUpdate(cust);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestUpsertExistingCustomerNoResult() {
            this.TestInsertCustomerNoResult();

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.InsertOrUpdate(cust);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestUpsertNewCustomerWithResult() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.InsertOrUpdate(cust, null, d => d.City);
            Assert.AreEqual("Seattle", result);
        }

        [TestMethod]
        public void TestUpsertExistingCustomerWithResult() {
            this.TestInsertCustomerNoResult();

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.InsertOrUpdate(cust, null, d => d.City);
            Assert.AreEqual("Portland", result);
        }

        [TestMethod]
        public void TestUpsertNewCustomerWithUpdateCheck() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.InsertOrUpdate(cust, d => d.City == "Portland");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestUpsertExistingCustomerWithUpdateCheck() {
            this.TestInsertCustomerNoResult();

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Portland", // moved to Portland!
                Country = "USA"
            };

            var result = this.Northwind.Customers.InsertOrUpdate(cust, d => d.City == "Seattle");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestBatchUpsertNewCustomersNoResult() {
            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Portland",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.InsertOrUpdate(c));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchUpsertExistingCustomersNoResult() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Portland",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.InsertOrUpdate(c));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchUpsertNewCustomersWithResult() {
            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Portland",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.InsertOrUpdate(c, null, d => d.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, "Portland")));
        }

        [TestMethod]
        public void TestBatchUpsertExistingCustomersWithResult() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Portland",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.InsertOrUpdate(c, null, d => d.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, "Portland")));
        }

        [TestMethod]
        public void TestBatchUpsertNewCustomersWithUpdateCheck() {
            int n = 10;
            var pairs = Enumerable.Range(1, n).Select(
                i => new {
                    original = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Seattle",
                        Country = "USA"
                    },
                    current = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Portland",
                        Country = "USA"
                    }
                });

            var results = this.Northwind.Customers.Batch(pairs, (u, x) => u.InsertOrUpdate(x.current, d => d.City == x.original.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchUpsertExistingCustomersWithUpdateCheck() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var pairs = Enumerable.Range(1, n).Select(
                i => new {
                    original = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Seattle",
                        Country = "USA"
                    },
                    current = new Customer {
                        CustomerID = "XX" + i,
                        CompanyName = "Company" + i,
                        ContactName = "Contact" + i,
                        City = "Portland",
                        Country = "USA"
                    }
                });

            var results = this.Northwind.Customers.Batch(pairs, (u, x) => u.InsertOrUpdate(x.current, d => d.City == x.original.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestDeleteCustomer() {
            this.TestInsertCustomerNoResult();

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            var result = this.Northwind.Customers.Delete(cust);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDeleteCustomerForNonExistingCustomer() {
            this.TestInsertCustomerNoResult();

            var cust = new Customer {
                CustomerID = "XX2",
                CompanyName = "Company2",
                ContactName = "Contact2",
                City = "Seattle",
                Country = "USA"
            };

            var result = this.Northwind.Customers.Delete(cust);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestDeleteCustomerWithDeleteCheckThatSucceeds() {
            this.TestInsertCustomerNoResult();

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            var result = this.Northwind.Customers.Delete(cust, d => d.City == "Seattle");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDeleteCustomerWithDeleteCheckThatDoesNotSucceed() {
            this.TestInsertCustomerNoResult();

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            var result = this.Northwind.Customers.Delete(cust, d => d.City == "Portland");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestBatchDeleteCustomers() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Seattle",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.Delete(c));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchDeleteCustomersWithDeleteCheck() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Seattle",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.Delete(c, d => d.City == c.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 1)));
        }

        [TestMethod]
        public void TestBatchDeleteCustomersWithDeleteCheckThatDoesNotSucceed() {
            this.TestBatchInsertCustomersNoResult();

            int n = 10;
            var custs = Enumerable.Range(1, n).Select(
                i => new Customer {
                    CustomerID = "XX" + i,
                    CompanyName = "Company" + i,
                    ContactName = "Contact" + i,
                    City = "Portland",
                    Country = "USA"
                });

            var results = this.Northwind.Customers.Batch(custs, (u, c) => u.Delete(c, d => d.City == c.City));
            Assert.AreEqual(n, results.Count());
            Assert.IsTrue(results.All(r => object.Equals(r, 0)));
        }

        [TestMethod]
        public void TestDeleteWhere() {
            this.TestBatchInsertCustomersNoResult();

            var result = this.Northwind.Customers.Delete(c => c.CustomerID.StartsWith("XX"));
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestSessionIdentityCache() {
            NorthwindSession ns = new NorthwindSession(this.Northwind.Provider);

            // both objects should the same instance
            var cust = ns.Customers.Single(c => c.CustomerID == "ALFKI");
            var cust2 = ns.Customers.Single(c => c.CustomerID == "ALFKI");

            Assert.AreNotEqual(null, cust);
            Assert.AreNotEqual(null, cust2);
            Assert.AreEqual(cust, cust2);
        }

        [TestMethod]
        public void TestSessionProviderNotIdentityCached() {
            NorthwindSession ns = new NorthwindSession(this.Northwind.Provider);
            NorthwindDataContext db2 = new NorthwindDataContext(this.Northwind.Provider);

            var cust = ns.Customers.Single(c => c.CustomerID == "ALFKI");
            var cust2 = ns.Customers.ProviderTable.Single(c => c.CustomerID == "ALFKI");

            Assert.AreNotEqual(null, cust);
            Assert.AreNotEqual(null, cust2);
            Assert.AreEqual(cust.CustomerID, cust2.CustomerID);
            Assert.AreNotEqual(cust, cust2);
        }

        [TestMethod]
        public void TestSessionInsertCustomer() {
            NorthwindSession ns = new NorthwindSession(this.Northwind.Provider);

            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            ns.Customers.InsertOnSubmit(cust);
            ns.SubmitChanges();
        }

        [TestMethod]
        public void TestSessionUpdateCustomer() {
            this.Northwind.Customers.Insert(
                new Customer {
                    CustomerID = "XX1",
                    CompanyName = "Company1",
                    ContactName = "Contact1",
                    City = "Seattle",
                    Country = "USA"
                });

            var ns = new NorthwindSession(this.Northwind.Provider);

            // fetch the previously inserted customer
            var cust = ns.Customers.Single(c => c.CustomerID == "XX1");
            cust.ContactName = "Contact Modified";

            ns.SubmitChanges();

            var cust2 = this.Northwind.Customers.Single(c => c.CustomerID == "XX1");

            Assert.AreEqual("Contact Modified", cust2.ContactName);
        }

        [TestMethod]
        public void TestSessionSubmitActionOnModify() {
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            this.Northwind.Customers.Insert(cust);

            NorthwindSession ns = new NorthwindSession(this.Northwind.Provider);
            Assert.AreEqual(SubmitAction.None, ns.Customers.GetSubmitAction(cust));

            // fetch the previously inserted customer
            cust = ns.Customers.Single(c => c.CustomerID == "XX1");
            Assert.AreEqual(SubmitAction.None, ns.Customers.GetSubmitAction(cust));

            cust.ContactName = "Contact Modified";
            Assert.AreEqual(SubmitAction.Update, ns.Customers.GetSubmitAction(cust));

            ns.SubmitChanges();
            Assert.AreEqual(SubmitAction.None, ns.Customers.GetSubmitAction(cust));

            // prove actually modified by fetching through provider
            var cust2 = this.Northwind.Customers.Single(c => c.CustomerID == "XX1");
            Assert.AreEqual("Contact Modified", cust2.ContactName);

            // ready to be submitted again!
            cust.City = "SeattleX";
            Assert.AreEqual(SubmitAction.Update, ns.Customers.GetSubmitAction(cust));
        }

        [TestMethod]
        public void TestSessionSubmitActionOnInsert() {
            NorthwindSession ns = new NorthwindSession(this.Northwind.Provider);
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };
            Assert.AreEqual(SubmitAction.None, ns.Customers.GetSubmitAction(cust));

            ns.Customers.InsertOnSubmit(cust);
            Assert.AreEqual(SubmitAction.Insert, ns.Customers.GetSubmitAction(cust));

            ns.SubmitChanges();
            Assert.AreEqual(SubmitAction.None, ns.Customers.GetSubmitAction(cust));

            cust.City = "SeattleX";
            Assert.AreEqual(SubmitAction.Update, ns.Customers.GetSubmitAction(cust));
        }

        [TestMethod]
        public void TestSessionSubmitActionOnInsertOrUpdate() {
            NorthwindSession ns = new NorthwindSession(this.Northwind.Provider);
            var cust = new Customer {
                CustomerID = "XX1",
                CompanyName = "Company1",
                ContactName = "Contact1",
                City = "Seattle",
                Country = "USA"
            };

            Assert.AreEqual(SubmitAction.None, ns.Customers.GetSubmitAction(cust));

            ns.Customers.InsertOrUpdateOnSubmit(cust);
            Assert.AreEqual(SubmitAction.InsertOrUpdate, ns.Customers.GetSubmitAction(cust));

            ns.SubmitChanges();
            Assert.AreEqual(SubmitAction.None, ns.Customers.GetSubmitAction(cust));

            cust.City = "SeattleX";
            Assert.AreEqual(SubmitAction.Update, ns.Customers.GetSubmitAction(cust));
        }
    }
}
