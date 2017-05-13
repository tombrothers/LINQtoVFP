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

namespace LinqToVfp.Northwind.Tests.NorthwindEntityProvider {
    public partial class Product {
        public int? CategoryId { get; set; }
        public bool Discontinued { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Category Category { get; set; }				
        public List<OrderDetail> OrderDetails { get; set; }
    }
}