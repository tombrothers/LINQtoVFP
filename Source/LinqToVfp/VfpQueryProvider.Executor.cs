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
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IQToolkit;
using IQToolkit.Data;
using IQToolkit.Data.Common;
using VfpClient;
using System.Data.OleDb;

namespace LinqToVfp {
    public partial class VfpQueryProvider {
        public new class Executor : DbEntityProvider.Executor {
            private VfpQueryProvider provider;
            private int rowsAffected;

            public Executor(VfpQueryProvider provider)
                : base(provider) {
                this.provider = provider;
            }

            protected override DbDataReader ExecuteReader(DbCommand command) {
                DbDataReader reader = new VfpDataReader(command.ExecuteReader(), this.provider.AutoRightTrimStrings);

                if (this.BufferResultRows) {
                    // use data table to buffer results
                    var ds = new DataSet();
                    ds.EnforceConstraints = false;
                    var table = new DataTable();
                    ds.Tables.Add(table);
                    ds.EnforceConstraints = false;
                    table.Load(reader);
                    reader = table.CreateDataReader();
                }
                return reader;
            }
            protected override IEnumerable<T> Project<T>(DbDataReader reader, Func<FieldReader, T> fnProjector, MappingEntity entity, bool closeReader) {
                VfpDataReader vfpDataReader = new VfpDataReader(reader, this.provider.AutoRightTrimStrings);
                return base.Project<T>(vfpDataReader, fnProjector, entity, closeReader);
            }

            public override object Convert(object value, Type type) {
                // special handling for vfp type "Character (binary)"
                if (value != null && type == typeof(string) && value.GetType() == typeof(byte[])) {
                    string result = Encoding.Default.GetString((byte[])value);

                    if (this.provider.AutoRightTrimStrings) {
                        return result.TrimEnd();
                    }

                    return result;
                }

                return base.Convert(value, type);
            }

            public override IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize, bool stream) {
                this.StartUsingConnection();
                try {
                    var result = this.ExecuteBatch(query, paramSets);
                    if (!stream || this.ActionOpenedConnection) {
                        return result.ToList();
                    }
                    else {
                        return new EnumerateOnce<int>(result);
                    }
                }
                finally {
                    this.StopUsingConnection();
                }
            }

            private IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets) {
                this.LogCommand(query, null);

                foreach (var paramValues in paramSets) {
                    this.LogParameters(query, paramValues);
                    this.LogMessage(string.Empty);

                    DbCommand cmd = this.GetCommand(query, paramValues);

                    this.rowsAffected = cmd.ExecuteNonQuery();
                    yield return this.rowsAffected;
                }
            }

            protected override void SetParameterValues(QueryCommand query, DbCommand command, object[] paramValues) {
                base.SetParameterValues(query, command, paramValues);

                if (paramValues == null) {
                    return;
                }

                foreach (DbParameter parameter in command.Parameters) {
                    if (parameter.ParameterName.StartsWith("@")) {
                        parameter.ParameterName = parameter.ParameterName.Substring(1);
                    }
                }

                //this.ReOrderParameters(query, command);
            }

            // The parameters added to the collection may not be added in the same order as they have been
            //      referenced.  These parameters need to be reordered in the order in which they are referenced
            //      because the Vfp OleDb driver does not use named parameters
            private void ReOrderParameters(QueryCommand query, DbCommand command) {
                if (command.Parameters.Count > 0) {
                    MatchCollection mc = Regex.Matches(command.CommandText, @"__Param__\d*__");
                    if (mc.Count > 0) {
                        List<DbParameter> parameterList = new List<DbParameter>();

                        for (int index = 0, total = mc.Count; index < total; index++) {
                            DbParameter p = command.Parameters[mc[index].Value];
                            command.CommandText = command.CommandText.Replace(mc[index].Value, "?");

                            // this tests to see if the parameters are in the correct order
                            if (p.ParameterName != "@Param_#" + index.ToString() + "@") {
                            }

                            parameterList.Add(p);
                        }

                        command.Parameters.Clear();

                        for (int index = 0, total = parameterList.Count; index < total; index++) {
                            DbParameter p = parameterList[index];

                            if (command.Parameters.Contains(p.ParameterName)) {
                                QueryParameter qp = query.Parameters.First(d => d.Name == p.ParameterName);
                                QueryParameter qp2 = new QueryParameter("p" + index.ToString(), qp.Type, qp.QueryType);
                                this.AddParameter(command, qp2, p.Value);
                            }
                            else {
                                command.Parameters.Add(p);
                            }
                        }
                    }
                }
            }

            protected override void AddParameter(DbCommand command, QueryParameter parameter, object value) {
                QueryType qt = parameter.QueryType;

                if (qt == null) {
                    qt = this.provider.Language.TypeSystem.GetColumnType(parameter.Type);
                }

                var p = new VfpParameter(parameter.Name, this.GetOleDbType(qt).ToVfpType());
                p.Size = qt.Length;
                ((VfpCommand)command).Parameters.Add(p);

                //if (qt.Precision != 0) {
                //    p.Precision = (byte)qt.Precision;
                //}

                //if (qt.Scale != 0) {
                //    p.Scale = (byte)qt.Scale;
                //}

                if (value != null && value is TimeSpan) {
                    p.Value = ((TimeSpan)value).TotalSeconds;
                }
                else {
                    p.Value = value ?? DBNull.Value;
                }
            }

            protected OleDbType GetOleDbType(QueryType type) {
                DbQueryType sqlType = type as DbQueryType;

                if (sqlType != null) {
                    return ToOleDbType(sqlType.SqlDbType);
                }

                return ToOleDbType(((DbQueryType)type).SqlDbType);
            }

            protected override void LogCommand(QueryCommand command, object[] paramValues) {
                if (this.provider.Log != null) {
                    if (paramValues != null) {
                        this.LogParameters(command, paramValues);
                    }

                    this.provider.Log.WriteLine(command.CommandText.Replace("\r\n", " ;\r\n"));
                    this.provider.Log.WriteLine();
                }
            }

            protected override void LogParameters(QueryCommand command, object[] paramValues) {
                if (this.provider.Log != null && paramValues != null) {
                    for (int i = 0, n = command.Parameters.Count; i < n; i++) {
                        var p = command.Parameters[i];
                        var v = paramValues[i];

                        if (v == null || v == DBNull.Value) {
                            this.provider.Log.WriteLine("{0} = NULL", p.Name);
                        }
                        else {
                            string typeName = p.Type.Name;

                            if (typeName == "Nullable`1") {
                                typeName = p.Type.GetGenericArguments()[0].Name;
                            }

                            switch (typeName) {
                                case "Byte":
                                case "SByte":
                                case "Int32":
                                case "UInt32":
                                case "Int16":
                                case "UInt16":
                                case "Int64":
                                case "UInt64":
                                case "Single":
                                case "Double":
                                case "Decimal":
                                    this.provider.Log.WriteLine("{0} = {1}", p.Name, v);
                                    break;
                                case "Boolean":
                                    bool boolValue = (bool)v;
                                    this.provider.Log.WriteLine("{0} = {1}", p.Name, (boolValue ? ".t." : ".f."));
                                    break;
                                case "String":
                                    if (v.ToString().Length > 254 || v.ToString().IndexOf(Environment.NewLine) >= 0) {
                                        this.provider.Log.WriteLine(string.Format(@"
TEXT TO {0} NOSHOW
{1}
ENDTEXT"
                                            , p.Name, v));
                                    }
                                    else {
                                        this.provider.Log.WriteLine("{0} = [{1}]", p.Name, v);
                                    }
                                    break;
                                default:
                                    this.provider.Log.WriteLine("{0} = [{1}]", p.Name, v);
                                    break;
                            }

                        }
                    }
                }
            }
        }
    }
}
