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
using System.Data;
using System.Data.OleDb;

namespace LinqToVfp {
    public partial class VfpQueryProvider {
        protected static OleDbType ToOleDbType(SqlDbType type) {
            switch (type) {
                case SqlDbType.BigInt:
                    return OleDbType.BigInt;
                case SqlDbType.Binary:
                    return OleDbType.Binary;
                case SqlDbType.Bit:
                    return OleDbType.Boolean;
                case SqlDbType.Char:
                    return OleDbType.Char;
                case SqlDbType.Date:
                    return OleDbType.Date;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    return OleDbType.DBTimeStamp;
                case SqlDbType.Decimal:
                    return OleDbType.Decimal;
                case SqlDbType.Float:
                case SqlDbType.Real:
                    return OleDbType.Double;
                case SqlDbType.Image:
                    return OleDbType.LongVarBinary;
                case SqlDbType.Int:
                    return OleDbType.Integer;
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return OleDbType.Currency;
                case SqlDbType.NChar:
                    return OleDbType.WChar;
                case SqlDbType.NText:
                    return OleDbType.LongVarChar;
                case SqlDbType.NVarChar:
                    return OleDbType.VarWChar;
                case SqlDbType.SmallInt:
                    return OleDbType.SmallInt;
                case SqlDbType.Text:
                    return OleDbType.LongVarChar;
                case SqlDbType.Time: // TimeSpan will be converted to seconds
                    return OleDbType.Integer;
                case SqlDbType.Timestamp:
                    return OleDbType.Binary;
                case SqlDbType.TinyInt:
                    return OleDbType.TinyInt;
                case SqlDbType.Udt:
                    return OleDbType.Variant;
                case SqlDbType.UniqueIdentifier:
                    return OleDbType.Guid;
                case SqlDbType.VarBinary:
                    return OleDbType.VarBinary;
                case SqlDbType.VarChar:
                    return OleDbType.VarChar;
                case SqlDbType.Variant:
                    return OleDbType.Variant;
                case SqlDbType.Xml:
                    return OleDbType.VarWChar;
                default:
                    throw new InvalidOperationException(string.Format("Unhandled sql type: {0}", type));
            }
        }

        protected static OleDbType ToOleDbType(DbType type) {
            switch (type) {
                case DbType.AnsiString:
                    return OleDbType.VarChar;
                case DbType.AnsiStringFixedLength:
                    return OleDbType.Char;
                case DbType.Binary:
                    return OleDbType.Binary;
                case DbType.Boolean:
                    return OleDbType.Boolean;
                case DbType.Byte:
                    return OleDbType.UnsignedTinyInt;
                case DbType.Currency:
                    return OleDbType.Currency;
                case DbType.Date:
                    return OleDbType.Date;
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return OleDbType.DBTimeStamp;
                case DbType.Decimal:
                    return OleDbType.Decimal;
                case DbType.Double:
                    return OleDbType.Double;
                case DbType.Guid:
                    return OleDbType.Guid;
                case DbType.Int16:
                    return OleDbType.SmallInt;
                case DbType.Int32:
                    return OleDbType.Integer;
                case DbType.Int64:
                    return OleDbType.BigInt;
                case DbType.Object:
                    return OleDbType.Variant;
                case DbType.SByte:
                    return OleDbType.TinyInt;
                case DbType.Single:
                    return OleDbType.Single;
                case DbType.String:
                    return OleDbType.VarWChar;
                case DbType.StringFixedLength:
                    return OleDbType.WChar;
                case DbType.Time:
                    return OleDbType.DBTime;
                case DbType.UInt16:
                    return OleDbType.UnsignedSmallInt;
                case DbType.UInt32:
                    return OleDbType.UnsignedInt;
                case DbType.UInt64:
                    return OleDbType.UnsignedBigInt;
                case DbType.VarNumeric:
                    return OleDbType.Numeric;
                case DbType.Xml:
                    return OleDbType.VarWChar;
                default:
                    throw new InvalidOperationException(string.Format("Unhandled db type '{0}'.", type));
            }
        }
    }
}
