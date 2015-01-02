/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
namespace LinqToVfpLinqPadDriver.Schema {
    public class PrimaryKey {
        public string TableName { get; private set; }
        public string ColumnName { get; private set; }
        public bool AutoIncrement { get; private set; }

        public PrimaryKey(string tableName, string columnName, bool autoIncrement) {
            TableName = tableName;
            ColumnName = columnName;
            AutoIncrement = autoIncrement;
        }
    }
}