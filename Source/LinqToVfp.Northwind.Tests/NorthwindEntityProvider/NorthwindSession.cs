/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using IQToolkit;
using IQToolkitContrib;

namespace LinqToVfp.Northwind.Tests.NorthwindEntityProvider {
    public class NorthwindSession : DbEntitySessionBase, INorthwindSession {
        public NorthwindSession(VfpQueryProvider provider)
            : base(provider) {
        }

        public ISessionTable<Customer> Customers {
            get { return this.GetTable<Customer>(); }
        }

        public ISessionTable<Order> Orders {
            get { return this.GetTable<Order>(); }
        }

        public ISessionTable<OrderDetail> OrderDetails {
            get { return this.GetTable<OrderDetail>(); }
        }
    }
}
