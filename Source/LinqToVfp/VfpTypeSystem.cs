using System;
/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Data;
using System.Diagnostics;
using System.Text;
using IQToolkit.Data;
using IQToolkit.Data.Common;
using VfpClient;

namespace LinqToVfp {
    internal class VfpTypeSystem : DbTypeSystem {
        public override QueryType GetQueryType(string typeName, string[] args, bool isNotNull) {
            var queryType = (DbQueryType)base.GetQueryType(typeName, args, isNotNull);

            if (args != null && args.Length == 2) {
                return new VfpDbQueryType(queryType);
            }

            return queryType;
        }

        public override SqlDbType GetSqlType(string typeName) {
            if (string.Compare(typeName, "Memo", true) == 0) {
                return SqlDbType.Text;
            }

            if (string.Compare(typeName, "Currency", true) == 0) {
                return SqlDbType.Decimal;
            }

            if (string.Compare(typeName, "Logical", true) == 0) {
                return SqlDbType.Bit;
            }

            var vfpTypeName = typeName.ToVfpTypeName();

            if (string.IsNullOrEmpty(vfpTypeName)) {
                return base.GetSqlType(typeName);
            }

            var vfpType = vfpTypeName.ToVfpType();
            
            return GetSqlDbType(vfpType.ToDbType());
        }

        private static SqlDbType GetSqlDbType(DbType dbType) {
            switch (dbType) {
                case DbType.StringFixedLength:
                    return SqlDbType.Char;
                case DbType.Currency:
                    return SqlDbType.Money;
                case DbType.Date:
                    return SqlDbType.Date;
                case DbType.DateTime:
                    return SqlDbType.DateTime;
                case DbType.Single:
                case DbType.Double:
                    return SqlDbType.Float;
                case DbType.Int32:
                    return SqlDbType.Int;
                case DbType.Boolean:
                    return SqlDbType.Bit;
                case DbType.Decimal:
                    return SqlDbType.Decimal;
                case DbType.Binary:
                    return SqlDbType.Binary;
                case DbType.String:
                    return SqlDbType.VarChar;
                case DbType.Object:
                    return SqlDbType.Udt;
                default:
                    throw new ArgumentOutOfRangeException("dbType");
            }
        }

        public override
                    string GetVariableDeclaration(QueryType type, bool suppressSize) {
            StringBuilder sb = new StringBuilder();
            DbQueryType sqlType = (DbQueryType)type;
            SqlDbType sqlDbType = sqlType.SqlDbType;

            switch (sqlDbType) {
                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.DateTime:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.SmallDateTime:
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.Timestamp:
                case SqlDbType.TinyInt:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.Variant:
                case SqlDbType.Xml:
                    sb.Append(sqlDbType);
                    break;
                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                    sb.Append(sqlDbType);
                    if (type.Length > 0 && !suppressSize) {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }

                    break;
                case SqlDbType.Image:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                    sb.Append(sqlDbType);
                    if (type.Length > 0 && !suppressSize) {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }

                    break;
                case SqlDbType.Decimal:
                    sb.Append("Currency");
                    break;
                case SqlDbType.Float:
                case SqlDbType.Real:
                    sb.Append(sqlDbType);
                    if (type.Precision != 0) {
                        sb.Append("(");
                        sb.Append(type.Precision);
                        if (type.Scale != 0) {
                            sb.Append(",");
                            sb.Append(type.Scale);
                        }

                        sb.Append(")");
                    }

                    break;
            }

            return sb.ToString();
        }
    }
}
