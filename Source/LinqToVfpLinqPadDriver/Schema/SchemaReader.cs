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
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using LINQPad.Extensibility.DataContext;
using LINQPad.Extensibility.DataContext.DbSchema;
using VfpClient;
using SchemaColumnNames = VfpClient.VfpConnection.SchemaColumnNames;
using SchemaNames = VfpClient.VfpConnection.SchemaNames;

namespace LinqToVfpLinqPadDriver.Schema {
    public class SchemaReader {
        private readonly VfpConnection _connection;
        private readonly List<string> _tableNames;
        private readonly List<PrimaryKey> _primaryKeys;
        private readonly IConnectionInfo _connectionInfo;
        private readonly bool _singularize;
        private readonly PluralizationService _pluralizationService = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

        public SchemaReader(VfpConnection connection, IConnectionInfo connectionInfo) {
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }

            if (connectionInfo == null) {
                throw new ArgumentNullException("connectionInfo");
            }

            _connection = connection;
            _connectionInfo = connectionInfo;

            new VfpConnectionStringBuilder(_connection.ConnectionString);

            _singularize = GetSingularizeSetting();
            _tableNames = GetTableNames();
            _primaryKeys = GetPrimaryKeys();
        }

        private bool GetSingularizeSetting() {
            var singularizeElement = _connectionInfo.DriverData.Element("Singularize");

            return singularizeElement != null && Convert.ToBoolean(singularizeElement.Value);
        }

        public Database GetDatabase() {
            var columns = GetColumns();
            var associations = GetAssociations();
            var parameters = new List<Parameter>();

            return new Database(_connectionInfo.DynamicSchemaOptions, columns, associations, parameters);
        }

        private IEnumerable<ColumnAssociation> GetAssociations() {
            var associations = new List<ColumnAssociation>();

            if (!_connection.IsDbc) {
                return associations;
            }

            DataTable foreignKeys = null;

            DoConnected(() => {
                foreignKeys = _connection.GetSchema(SchemaNames.ForeignKeys);
            });

            associations.AddRange((from row in foreignKeys.AsEnumerable()
                                   select new ColumnAssociation {
                                       RelationshipName = row.Field<string>(SchemaColumnNames.ForeignKey.ForeignKeyName),
                                       ChildTable = row.Field<string>(SchemaColumnNames.ForeignKey.ForeignKeyTableName),
                                       ChildColumn = row.Field<string>(SchemaColumnNames.ForeignKey.ForeignKeyFieldName),
                                       ParentTable = row.Field<string>(SchemaColumnNames.ForeignKey.PrimaryKeyTableName),
                                       ParentColumn = row.Field<string>(SchemaColumnNames.ForeignKey.PrimaryKeyFieldName)
                                   }).ToList());

            return associations;
        }

        private IEnumerable<Column> GetColumns() {
            var list = new List<Column>();
            var tables = GetTableOrViewColumns("Table").ToList();

            if (tables.Any()) {
                list.AddRange(tables);
            }

            var views = GetTableOrViewColumns("View").ToList();

            if (views.Any()) {
                list.AddRange(views);
            }

            return list;
        }

        private IEnumerable<Column> GetTableOrViewColumns(string prefix) {
            var list = new List<Column>();

            DoConnected(() => {
                var columns = _connection.GetSchema(prefix + "Fields");

                if (columns.Rows.Count > 0) {
                    list.AddRange(GetVfpColumns(columns, prefix + "Name"));
                }
            });

            return list;
        }

        private IEnumerable<Column> GetVfpColumns(DataTable dataTable, string objectColumnName) {
            return dataTable.AsEnumerable()
                            .Select((row, index) => new VfpColumn {
                                ObjectName = row.Field<string>(objectColumnName),
                                ObjectKind = GetDbObjectKind(row.Field<string>(objectColumnName)),
                                ColumnID = index,
                                ColumnOrdinal = row.Field<int>(SchemaColumnNames.TableField.Ordinal),
                                ColumnName = row.Field<string>(SchemaColumnNames.TableField.FieldName),
                                IsNullable = row.Field<bool>(SchemaColumnNames.TableField.Nullable),
                                Length = GetWidth(row),
                                Precision = GetWidth(row),
                                Scale = GetScale(row),
                                IsKey = IsKey(row.Field<string>(objectColumnName), row.Field<string>(SchemaColumnNames.TableField.FieldName)),
                                IsAutoGen = IsAutoGen(row.Field<string>(objectColumnName), row.Field<string>(SchemaColumnNames.TableField.FieldName)),
                                IsComputed = IsAutoGen(row.Field<string>(objectColumnName), row.Field<string>(SchemaColumnNames.TableField.FieldName)),
                                ClrType = GetClrType((VfpType)row.Field<int>(SchemaColumnNames.TableField.VfpType), row.Field<bool>(SchemaColumnNames.TableField.Nullable)),
                                FieldType = GetFieldType(row),
                                PropertyName = GetPropertyName(row, objectColumnName)
                            });
        }

        private static string GetFieldType(DataRow row) {
            var vfpType = (VfpType) row.Field<int>(SchemaColumnNames.TableField.VfpType);
            var fieldType = vfpType.ToFieldType();

            return string.Format(fieldType, GetWidth(row), GetScale(row));
        }

        private string GetPropertyName(DataRow row, string objectColumnName) {
            var columnName = row.Field<string>(SchemaColumnNames.TableField.FieldName);
            var parentName = row.Field<string>(objectColumnName);

            if (_singularize) {
                parentName = _pluralizationService.Singularize(parentName);
            }

            if (columnName.Equals(parentName, StringComparison.InvariantCultureIgnoreCase)) {
                return columnName + "_";
            }

            return columnName;
        }

        private static int GetScale(DataRow row) {
            return row.IsNull(SchemaColumnNames.TableField.Decimal) ? 0 : row.Field<int>(SchemaColumnNames.TableField.Decimal);
        }

        private static int GetWidth(DataRow row) {
            return row.IsNull(SchemaColumnNames.TableField.Width) ? 0 : row.Field<int>(SchemaColumnNames.TableField.Width);
        }

        private static Type GetClrType(VfpType vfpType, bool isNullable) {
            var type = vfpType.ToType();

            if (isNullable && type.IsValueType) {
                type = typeof(Nullable<>).MakeGenericType(new[] { type });
            }

            return type;
        }

        private bool IsAutoGen(string tableName, string columnName) {
            var pk = GetPrimaryKey(tableName, columnName);

            return pk != null && pk.AutoIncrement;
        }

        private bool IsKey(string tableName, string columnName) {
            return GetPrimaryKey(tableName, columnName) != null;
        }

        private PrimaryKey GetPrimaryKey(string tableName, string columnName) {
            return _primaryKeys.Find(x => x.TableName == tableName && x.ColumnName == columnName);
        }

        private DbObjectKind GetDbObjectKind(string name) {
            return _tableNames.Contains(name) ? DbObjectKind.Table : DbObjectKind.View;
        }

        private List<string> GetTableNames() {
            DataTable dataTable = null;

            DoConnected(() => {
                dataTable = _connection.GetSchema(SchemaNames.Tables);
            });

            return dataTable.AsEnumerable()
                            .Select(row => row.Field<string>(SchemaColumnNames.Table.TableName))
                            .ToList();
        }

        private List<PrimaryKey> GetPrimaryKeys() {
            DataTable schema = null;

            DoConnected(() => {
                schema = _connection.GetSchema(SchemaNames.PrimaryKeys);
            });

            return schema.AsEnumerable()
                         .Select(row => new PrimaryKey(row.Field<string>(SchemaColumnNames.PrimaryKey.TableName),
                                                       row.Field<string>(SchemaColumnNames.PrimaryKey.FieldName),
                                                       row.Field<bool>(SchemaColumnNames.PrimaryKey.AutoInc)))
                         .ToList();
        }

        private void DoConnected(Action action) {
            if (action == null) {
                return;
            }

            var closeConnection = false;

            if (_connection.State == ConnectionState.Closed) {
                _connection.Open();
                closeConnection = true;
            }

            try {
                action();
            }
            finally {
                if (closeConnection) {
                    _connection.Close();
                }
            }
        }
    }
}