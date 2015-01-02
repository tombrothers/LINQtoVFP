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

namespace LinqToVfp.Northwind.Tests.NorthwindEntityProvider {
	public partial class EmpT {
        public int EmpTId { get; set; }
		public int Employeeid { get; set; }
		public string Territoryid { get; set; }					
	}
}