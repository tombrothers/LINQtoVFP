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
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using IQToolkit;
using IQToolkit.Data;
using IQToolkit.Data.Common;
using VfpClient;

namespace LinqToVfp {
    public partial class VfpQueryProvider : DbEntityProvider, IDisposable {
        public bool AutoRightTrimStrings { get; set; }

        internal VfpQueryProvider(VfpConnection connection, QueryMapping mapping)
            : this(connection, VfpLanguage.Default, mapping, VfpQueryPolicy.Default) {
        }

        internal VfpQueryProvider(VfpConnection connection, QueryLanguage language, QueryMapping mapping, VfpQueryPolicy policy) :
            base(connection, language, mapping, policy) {
        }

        public void Pack<T>(IEntityTable<T> entityTable) {
            ExecuteNonQuery(string.Format("EXECSCRIPT([USE {0} IN SELECT (0) EXCLUSIVE] + CHR(13) + [PACK] + CHR(13) + [CLOSE TABLES ALL])", GetTableName(entityTable)));
        }
        public void Zap<T>(IEntityTable<T> entityTable) {
            ExecuteNonQuery(string.Format("EXECSCRIPT([USE {0} IN SELECT (0) EXCLUSIVE] + CHR(13) + [ZAP] + CHR(13) + [CLOSE TABLES ALL])", GetTableName(entityTable)));
        }

        private int ExecuteNonQuery(string commandText) {
            var result = 0;

            using (var command = Connection.CreateCommand()) {
                command.CommandText = commandText;
                DoConnected(() => result = command.ExecuteNonQuery());
            }

            return result;
        }

        private string GetTableName<T>(IEntityTable<T> entityTable) {
            var advancedMapping = Mapping as VfpAdvancedMapping;

            if (advancedMapping == null) {
                return entityTable.TableId;
            }

            return advancedMapping.GetTableName(advancedMapping.GetEntity(typeof(T), entityTable.TableId));
        }

        protected VfpQueryProvider New(DbConnection connection, QueryMapping mapping, VfpQueryPolicy policy) {
            if (policy == null) {
                throw new ArgumentNullException("VfpQueryPolicy required");
            }

            return new VfpQueryProvider(connection as VfpConnection, VfpLanguage.Default, mapping, policy);
        }

        public VfpQueryProvider New(VfpConnection connection) {
            var provider = New(connection, Mapping, Policy as VfpQueryPolicy);

            provider.Log = Log;

            return provider;
        }

        public VfpQueryProvider New(VfpBasicMapping mapping) {
            var provider = New(Connection, mapping, Policy as VfpQueryPolicy);

            provider.Log = Log;

            return provider;
        }

        public VfpQueryProvider New(VfpQueryPolicy policy) {
            var provider = New(Connection, Mapping, policy);

            provider.Log = Log;

            return provider;
        }

        public static new QueryMapping GetMapping(string mappingId) {
            if (mappingId != null) {
                var type = FindLoadedType(mappingId);

                if (type != null) {
                    return new VfpAttributeMapping(type);
                }

                if (File.Exists(mappingId)) {
                    return VfpXmlMapping.FromXml(File.ReadAllText(mappingId));
                }
            }

            return new VfpImplicitMapping();
        }

        private static Type FindLoadedType(string typeName) {
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .Select(assem => assem.GetType(typeName, false, true))
                                          .FirstOrDefault(type => type != null);
        }

        protected internal void LogMessage(string message) {
            if (Log != null) {
                Log.WriteLine(message);
            }
        }

        public void Dispose() {
            if (Connection != null && Connection.State != ConnectionState.Closed) {
                Connection.Close();
                Connection.Dispose();
            }
        }

        public override object Execute(Expression expression) {
            StartUsingConnection();

            try {
                return base.Execute(expression);
            }
            finally {
                StopUsingConnection();
            }
        }

        protected override QueryExecutor CreateExecutor() {
            return new Executor(this);
        }

        public static VfpQueryProvider Create(string connectionString, string mappingId) {
            var connection = new VfpConnection(GetConnectionString(connectionString));

            return new VfpQueryProvider(connection, VfpLanguage.Default, GetMapping(mappingId), VfpQueryPolicy.Default);
        }

        internal static string GetConnectionString(string databaseFile) {
            if (!string.IsNullOrEmpty(databaseFile)) {
                if (databaseFile.Contains("=")) {
                    // Assumes complete connection string if the string includes the equals sign.
                    return databaseFile;
                }

                if (IsValidDataPath(databaseFile)) {
                    return string.Format("Provider=VFPOLEDB;Data Source={0}", databaseFile);
                }
            }

            throw new ApplicationException("Invalid connection string.");
        }

        private static bool IsValidDataPath(string path) {
            if (string.IsNullOrEmpty(path)) {
                return false;
            }

            return (Path.GetExtension(path).ToLower() == ".dbc" && File.Exists(path)) || Directory.Exists(path);
        }
    }
}