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
using System.Linq;
using System.Reflection;
using LINQPad;

namespace LinqToVfpLinqPadDriver.Schema {
    public class EntityMemberProvider : ICustomMemberProvider {
        private readonly object _entity;
        private readonly PropertyInfo[] _properties;

        public EntityMemberProvider(object entity) {
            _entity = entity;
            _properties = (from propertyInfo in entity.GetType().GetProperties()
                           let lazeLoadedAttribute = propertyInfo.GetCustomAttributes(false).Where(a => a.GetType().FullName.EndsWith(".LazyLoadedAttribute")).FirstOrDefault()
                           where lazeLoadedAttribute == null
                           select propertyInfo).ToArray();
        }

        public IEnumerable<string> GetNames() {
            return _properties.Select(x => x.Name);
        }

        public IEnumerable<Type> GetTypes() {
            return _properties.Select(x => x.PropertyType);
        }

        public IEnumerable<object> GetValues() {
            return _properties.Select(x => x.GetValue(_entity, null));
        }
    }
}