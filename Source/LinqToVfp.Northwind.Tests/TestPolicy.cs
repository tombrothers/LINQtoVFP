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
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;
using System.Reflection;

namespace LinqToVfp.Northwind.Tests {
    class TestPolicy : VfpQueryPolicy {
        HashSet<string> included;

        internal TestPolicy(params string[] includedRelationships) {
            this.included = new HashSet<string>(includedRelationships);
        }

        public override bool IsIncluded(MemberInfo member) {
            return this.included.Contains(member.Name);
        }
    }

    class TestMapping : ImplicitMapping {
        //protected override bool IsGenerated(MappingEntity entity, MemberInfo member) {
        //    return member.Name == "OrderID" && member.DeclaringType.Name == "Order";
        //}
    }
}
