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
using LINQPad.Extensibility.DataContext.DbSchema;
using LinqToVfpLinqPadDriver.Schema;

namespace LinqToVfpLinqPadDriver.CodeGeneration {
    public class EntityCodeGen : CodeGenBase {
        private readonly SchemaObject _schemaObject;

        public EntityCodeGen(bool singularize, SchemaObject schemaObject)
            : base(singularize) {
            if (schemaObject == null) {
                throw new ArgumentNullException("schemaObject");
            }

            _schemaObject = schemaObject;
        }

        public override string ToString() {
            WriteLine("[EntityAttribute]");
            Write("public class ");
            Write(GetEntityClassName(_schemaObject));
            WriteLine(" {");
            WriteColumns();
            WriteAssociations();
            WriteLine("}");

            return base.ToString();
        }

        private void WriteColumns() {
            foreach (var column in _schemaObject.Columns.Values.Cast<VfpColumn>()) {
                WriteTab();
                Write("public global::System.");
                WriteClrType(column);
                Write(" ");
                Write(column.PropertyName);
                WriteLine(" { get; set; }");
            }
        }

        private void WriteClrType(Column column) {
            if (column.ClrType.IsGenericType && column.ClrType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                Write(column.ClrType.GetGenericArguments()[0].Name + "?");
            }
            else {
                Write(column.ClrType.Name);
            }
        }

        private void WriteAssociations() {
            var table = _schemaObject as Table;

            if (table == null) {
                return;
            }

            WriteAssociations(table.ParentRelations, true);
            WriteAssociations(table.ChildRelations, false);
        }

        private void WriteAssociations(IEnumerable<Relationship> relations, bool isParentRelations) {
            foreach (var relation in relations) {
                var isOneToOne = isParentRelations || relation.IsOneToOne;

                WriteLine(
@"        private {propertyType} _{propertyName};

        [LazyLoadedAttribute]
        public {propertyType} {propertyName} {
            get {
                return this._{propertyName} ?? this.Get{propertyName}();
            }
            set {
                this._{propertyName} = value;
            }
        }

        private {propertyType} Get{propertyName}() {
            return DataContext.Instance.{dataContextPropertyName}.Where(x => {whereCondition}).ToList(){firstOrDefault};
        }
"
                .Replace("{propertyName}", GetPropertyName(relation, isParentRelations))
                .Replace("{propertyType}", GetPropertyType(relation, isParentRelations, isOneToOne))
                .Replace("{dataContextPropertyName}", GetDataContextPropertyName(relation, isParentRelations))
                .Replace("{whereCondition}", GetWhereCondition(relation, isParentRelations, isOneToOne))
                .Replace("{firstOrDefault}", (isOneToOne ? ".FirstOrDefault()" : string.Empty)));
            }
        }

        private static string GetWhereCondition(Relationship relation, bool isParentRelations, bool isOneToOne) {
            var whereCondition = string.Empty;

            for (int index = 0, total = relation.ParentCols.Count; index < total; index++) {
                if (!string.IsNullOrEmpty(whereCondition)) {
                    whereCondition += " && ";
                }

                if (isParentRelations) {
                    whereCondition = "x." + relation.ParentCols[0].PropertyName + " == this." + relation.ChildCols[0].PropertyName;
                }
                else {
                    whereCondition = "x." + relation.ChildCols[0].PropertyName + " == this." + relation.ParentCols[0].PropertyName;
                }
            }

            if (string.IsNullOrEmpty(whereCondition)) {
                return "false";
            }

            return whereCondition;
        }

        private static string GetPropertyName(Relationship relation, bool isParentRelations) {
            return isParentRelations ? relation.PropNameForChild : relation.PropNameForParent;
        }

        private string GetPropertyType(Relationship relation, bool isParentRelations, bool isOneToOne) {
            var propertyType = string.Empty;

            if (!isOneToOne) {
                propertyType += "List<";
            }

            if (isParentRelations) {
                propertyType += GetEntityClassName(relation.ParentTable);
            }
            else {
                propertyType += GetEntityClassName(relation.ChildTable);
            }

            if (!isOneToOne) {
                propertyType += ">";
            }

            return propertyType;
        }

        private static string GetDataContextPropertyName(Relationship relation, bool isParentRelations) {
            return isParentRelations ? relation.ParentTable.PropertyName : relation.ChildTable.PropertyName;
        }
    }
}