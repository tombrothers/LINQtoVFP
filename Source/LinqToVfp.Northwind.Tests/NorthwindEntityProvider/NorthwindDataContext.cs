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
using IQToolkit;
using IQToolkit.Data.Common;
using LinqToVfp;

namespace LinqToVfp.Northwind.Tests.NorthwindEntityProvider {
	public partial class NorthwindDataContext {
		public VfpQueryProvider Provider { get; private set; }

        public NorthwindDataContext (string connectionString)
            : this(connectionString, typeof(NorthwindDataContextAttributes).FullName) {
        }

        public NorthwindDataContext (string connectionString, string mappingId)
            : this(VfpQueryProvider.Create(connectionString, mappingId)) {
        }

        public NorthwindDataContext (VfpQueryProvider provider) {
            this.Provider = provider;
        }
        
        public virtual IEntityTable<Category> Categories {
            get { return this.Provider.GetTable<Category>("Categories"); }
        }
        
        public virtual IEntityTable<Customer> Customers {
            get { return this.Provider.GetTable<Customer>("Customers"); }
        }
        
        public virtual IEntityTable<EmpT> EmpTs {
            get { return this.Provider.GetTable<EmpT>("EmpTs"); }
        }
        
        public virtual IEntityTable<OrderDetail> OrderDetails {
            get { return this.Provider.GetTable<OrderDetail>("OrderDetails"); }
        }
        
        public virtual IEntityTable<Order> Orders {
            get { return this.Provider.GetTable<Order>("Orders"); }
        }
        
        public virtual IEntityTable<Product> Products {
            get { return this.Provider.GetTable<Product>("Products"); }
        }
	}
}
