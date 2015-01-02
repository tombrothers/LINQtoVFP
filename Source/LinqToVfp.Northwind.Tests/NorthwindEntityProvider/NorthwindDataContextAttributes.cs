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
using IQToolkit;
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;

namespace LinqToVfp.Northwind.Tests.NorthwindEntityProvider {
	public partial class NorthwindDataContextAttributes : NorthwindDataContext {
        public NorthwindDataContextAttributes(string connectionString)
            : base(connectionString) {
        }
              
        [Table(Name="Categories")]
		[Column(Member="CategoryId", IsPrimaryKey=true, IsGenerated=true)]
		[Column(Member="CategoryName")]
        [Association(Member="Products", KeyMembers="CategoryId", RelatedEntityID="Products", RelatedKeyMembers="CategoryId")]
        public override IEntityTable<Category> Categories {
            get { return base.Categories; }
        }
              
        [Table(Name="Customers")]
		[Column(Member="City")]
		[Column(Member="CompanyName")]
		[Column(Member="ContactName")]
		[Column(Member="Country")]
		[Column(Member="CustomerID", IsPrimaryKey=true)]
		[Column(Member="Phone")]
        [Association(Member="Orders", KeyMembers="CustomerID", RelatedEntityID="Orders", RelatedKeyMembers="CustomerID")]
        public override IEntityTable<Customer> Customers {
            get { return base.Customers; }
        }
              
        [Table(Name="EmployeeTerritories")]
		[Column(Member="Employeeid")]
		[Column(Member="Territoryid")]
        [Column(Member = "EmpTId", Name = "Recno()", IsPrimaryKey = true, IsGenerated = false, Alias = "")]
        public override IEntityTable<EmpT> EmpTs {
            get { return base.EmpTs; }
        }
              
        [Table(Name="OrderDetails")]
		[Column(Member="OrderID", IsPrimaryKey=true, IsGenerated=true)]
		[Column(Member="ProductID", IsPrimaryKey=true, IsGenerated=true)]
		[Column(Member="Quantity")]
		[Column(Member="UnitPrice")]
        [Association(Member="Order", KeyMembers="OrderID", RelatedEntityID="Orders", RelatedKeyMembers="OrderID")]
        [Association(Member="Product", KeyMembers="ProductID", RelatedEntityID="Products", RelatedKeyMembers="ProductID")]
        public override IEntityTable<OrderDetail> OrderDetails {
            get { return base.OrderDetails; }
        }
              
        [Table(Name="Orders")]
		[Column(Member="CustomerID")]
		[Column(Member="Freight")]
		[Column(Member="OrderDate")]
		[Column(Member="OrderID", IsPrimaryKey=true, IsGenerated=true)]
		[Column(Member="ShipCountry")]
        [Association(Member="Customer", KeyMembers="CustomerID", RelatedEntityID="Customers", RelatedKeyMembers="CustomerID")]
        [Association(Member="Details", KeyMembers="OrderID", RelatedEntityID="OrderDetails", RelatedKeyMembers="OrderID")]
        public override IEntityTable<Order> Orders {
            get { return base.Orders; }
        }
              
        [Table(Name="Products")]
		[Column(Member="CategoryId")]
		[Column(Member="Discontinued")]
		[Column(Member="ProductID", IsPrimaryKey=true, IsGenerated=true)]
		[Column(Member="ProductName")]
        [Association(Member="Category", KeyMembers="CategoryId", RelatedEntityID="Categories", RelatedKeyMembers="CategoryId")]
        [Association(Member="OrderDetails", KeyMembers="ProductID", RelatedEntityID="OrderDetails", RelatedKeyMembers="ProductID")]
        public override IEntityTable<Product> Products {
            get { return base.Products; }
        }
    }            
  }