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
using System.Collections.ObjectModel;
using System.Linq;
using LINQPad.Extensibility.DataContext.DbSchema;
using LinqToVfpLinqPadDriver.Schema;

namespace LinqToVfpLinqPadDriver.CodeGeneration {
    public class MappingCodeGen : CodeGenBase {
        private readonly ReadOnlyCollection<SchemaObject> _schemaObjects;

        public MappingCodeGen(bool singularize, ReadOnlyCollection<SchemaObject> schemaObjects)
            : base(singularize) {
            if (schemaObjects == null) {
                throw new ArgumentNullException("schemaObjects");
            }

            _schemaObjects = schemaObjects;
        }

        public override string ToString() {
            WriteLine("public class DataContextAttributes : DataContext {");

            foreach (var schemaObject in _schemaObjects) {
                WriteTab();
                Write("[Table(Name=\"");
                Write(schemaObject.SqlName);
                WriteLine("\")]");
                WriteColumns(schemaObject);
                WriteAssociations(schemaObject);
                WriteTab();
                Write("public override IEntityTable<");
                Write(GetEntityClassName(schemaObject));
                Write("> ");
                Write(schemaObject.PropertyName);
                WriteLine(" {");
                WriteTab(2);
                WriteLine("get { ");
                WriteTab(3);
                Write("return base.");
                Write(schemaObject.PropertyName);
                WriteLine(";");
                WriteTab(2);
                WriteLine("}");
                WriteTab();
                WriteLine("}");
            }

            WriteTab();
            WriteLine("public DataContextAttributes(string connectionString) : base(connectionString) {}");
            WriteLine("}");

            return base.ToString();
        }

        private void WriteColumns(SchemaObject schemaObject) {
            var columns = GetColumns(schemaObject);

            if (columns == null) {
                return;
            }

            foreach (var column in columns.Values.Cast<VfpColumn>()) {
                WriteTab();
                Write("[Column(Member=\"");
                Write(column.PropertyName);
                Write("\"");

                if (!column.ColumnName.Equals(column.PropertyName, StringComparison.InvariantCultureIgnoreCase)) {
                    Write(", Name=\"");
                    Write(column.ColumnName);
                    Write("\"");
                }

                if (column.IsKey) {
                    Write(", IsPrimaryKey=true");

                    if (column.IsAutoGen) {
                        Write(", IsGenerated=true");
                    }
                }

                if (!string.IsNullOrWhiteSpace(column.FieldType)) {
                    Write(", DbType=\"");
                    Write(column.FieldType);
                    Write("\"");
                }

                WriteLine(")]");
            }
        }

        private static IDictionary<int, Column> GetColumns(SchemaObject schemaObject) {
            var table = schemaObject as Table;

            if (table != null) {
                return table.Columns;
            }

            var view = schemaObject as View;

            if (view != null) {
                return view.Columns;
            }

            return null;
        }

        private void WriteAssociations(SchemaObject schemaObject) {
            var table = schemaObject as Table;

            if (table == null) {
                return;
            }

            WriteAssociations(table.ParentRelations, true);
            WriteAssociations(table.ChildRelations, false);
        }

        private void WriteAssociations(List<Relationship> relations, bool isParentRelations) {
            foreach (var relation in relations) {
                WriteTab();
                Write("[Association(Member=\"");
                WriteMemberName(relation, isParentRelations);
                Write("\", KeyMembers=\"");
                WriteKeyMembers(relation, isParentRelations);

                Write("\", RelatedEntityID=\"");
                WriteRelatedEntityId(relation, isParentRelations);

                Write("\", RelatedKeyMembers=\"");
                WriteKeyMembers(relation, !isParentRelations);
                WriteLine("\")]");
            }
        }

        private void WriteKeyMembers(Relationship relation, bool isParentRelations) {
            List<Column> columns;

            if (isParentRelations) {
                columns = relation.ChildCols;
            }
            else {
                columns = relation.ParentCols;
            }

            Write(string.Join(",", columns.Select(x => x.PropertyName).ToArray()));
        }

        private void WriteMemberName(Relationship relation, bool isParentRelations) {
            if (isParentRelations) {
                Write(relation.PropNameForChild);
            }
            else {
                Write(relation.PropNameForParent);
            }
        }

        private void WriteRelatedEntityId(Relationship relation, bool isParentRelations) {
            if (isParentRelations) {
                Write(relation.ParentTable.PropertyName);
            }
            else {
                Write(relation.ChildTable.PropertyName);
            }
        }
    }
}