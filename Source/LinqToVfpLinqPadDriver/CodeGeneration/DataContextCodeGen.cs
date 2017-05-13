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
using System.Collections.ObjectModel;
using LINQPad.Extensibility.DataContext.DbSchema;

namespace LinqToVfpLinqPadDriver.CodeGeneration {
    public class DataContextCodeGen : CodeGenBase {
        private readonly ReadOnlyCollection<SchemaObject> _schemaObjects;

        public DataContextCodeGen(bool singularize, ReadOnlyCollection<SchemaObject> schemaObjects)
            : base(singularize) {
            if (schemaObjects == null) {
                throw new ArgumentNullException("schemaObjects");
            }

            _schemaObjects = schemaObjects;
        }

        public override string ToString() {
            WriteLine("public class DataContext : IQToolkitContrib.DbEntityContextBase {");
            WriteTab();
            WriteLine("internal static DataContext Instance;");

            WriteLine();
            WriteTab();
            WriteLine("public new VfpQueryProvider Provider {");
            WriteTab(2);
            WriteLine("get {");
            WriteTab(3);
            WriteLine("return (VfpQueryProvider) base.Provider;");
            WriteTab(2);
            WriteLine("}");
            WriteTab();
            WriteLine("}");

            foreach (var schemaObject in _schemaObjects) {
                WriteLine();
                WriteTab();
                Write("public virtual IEntityTable<");
                Write(GetEntityClassName(schemaObject));
                Write("> ");
                Write(schemaObject.PropertyName);
                WriteLine(" {");
                WriteTab(2);
                WriteLine("get {");
                WriteTab(3);
                Write("return this.GetTable<");
                Write(GetEntityClassName(schemaObject));
                WriteLine(">();");
                WriteTab(2);
                WriteLine("}");
                WriteTab();
                WriteLine("}");
            }

            WriteLine();
            WriteTab();
            WriteLine("public DataContext(string connectionString) : base(VfpQueryProvider.Create(connectionString, typeof(DataContextAttributes).FullName)) {");
            WriteTab(2);
            WriteLine("Instance = this;");
            WriteTab();
            WriteLine("}");
            WriteLine("}");

            return base.ToString();
        }
    }
}