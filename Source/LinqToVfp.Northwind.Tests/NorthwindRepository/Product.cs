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
using System.Collections.Generic;

namespace LinqToVfp.Northwind.Tests.NorthwindRepository {
	public partial class Product {
		public bool Discontinued { get; set; }
		public int ProductID { get; set; }
		public string ProductName { get; set; }					
	}
}