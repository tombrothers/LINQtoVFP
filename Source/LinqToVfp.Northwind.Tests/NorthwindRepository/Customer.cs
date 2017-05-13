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

namespace LinqToVfp.Northwind.Tests.NorthwindRepository {
	public partial class Customer {
		public string City { get; set; }
		public string CompanyName { get; set; }
		public string ContactName { get; set; }
		public string Country { get; set; }
		public string CustomerID { get; set; }
		public string Phone { get; set; }
		public List<Order> Orders { get; set; }					
	}
}