/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Collections.Generic;
using System.Reflection;
using IQToolkit;
using IQToolkit.Data.Common;

namespace LinqToVfp {
    public abstract class VfpAdvancedMapping : VfpBasicMapping {
        public abstract bool IsNestedEntity(MappingEntity entity, MemberInfo member);
        public abstract IList<MappingTable> GetTables(MappingEntity entity);
        public abstract string GetAlias(MappingTable table);
        public abstract string GetAlias(MappingEntity entity, MemberInfo member);
        public abstract string GetTableName(MappingTable table);
        public abstract bool IsExtensionTable(MappingTable table);
        public abstract string GetExtensionRelatedAlias(MappingTable table);
        public abstract IEnumerable<string> GetExtensionKeyColumnNames(MappingTable table);
        public abstract IEnumerable<MemberInfo> GetExtensionRelatedMembers(MappingTable table);

        protected VfpAdvancedMapping() {
        }

        public override bool IsRelationship(MappingEntity entity, MemberInfo member) {
            return base.IsRelationship(entity, member)
                || this.IsNestedEntity(entity, member);
        }

        public override object CloneEntity(MappingEntity entity, object instance) {
            object clone = base.CloneEntity(entity, instance);

            // need to clone nested entities too
            foreach (var mi in this.GetMappedMembers(entity)) {
                if (this.IsNestedEntity(entity, mi)) {
                    MappingEntity nested = this.GetRelatedEntity(entity, mi);
                    var nestedValue = mi.GetValue(instance);
                    if (nestedValue != null) {
                        var nestedClone = this.CloneEntity(nested, mi.GetValue(instance));
                        mi.SetValue(clone, nestedClone);
                    }
                }
            }

            return clone;
        }

        public override bool IsModified(MappingEntity entity, object instance, object original) {
            if (base.IsModified(entity, instance, original)) {
                return true;
            }

            // need to check nested entities too
            foreach (var mi in this.GetMappedMembers(entity)) {
                if (this.IsNestedEntity(entity, mi)) {
                    MappingEntity nested = this.GetRelatedEntity(entity, mi);
                    if (this.IsModified(nested, mi.GetValue(instance), mi.GetValue(original))) {
                        return true;
                    }
                }
            }

            return false;
        }

        public override QueryMapper CreateMapper(QueryTranslator translator) {
            return new VfpAdvancedMapper(this, translator);
        }
    }
}