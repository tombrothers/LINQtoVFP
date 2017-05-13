/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using LINQPad.Extensibility.DataContext.DbSchema;

namespace LinqToVfpLinqPadDriver.Schema {
    internal class VfpColumn : Column {
        public string FieldType { get; set; }
        public new string PropertyName { get; set; }

        public override string GetFullSqlTypeDeclaration() {
            return FieldType;
        }
    }
}