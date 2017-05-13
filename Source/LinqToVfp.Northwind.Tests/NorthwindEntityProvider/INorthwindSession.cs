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
using IQToolkit;

namespace LinqToVfp.Northwind.Tests.NorthwindEntityProvider {
    public interface INorthwindSession {
        void SubmitChanges();
        ISessionTable<Customer> Customers { get; }
        ISessionTable<Order> Orders { get; }
        ISessionTable<OrderDetail> OrderDetails { get; }
    }
}
