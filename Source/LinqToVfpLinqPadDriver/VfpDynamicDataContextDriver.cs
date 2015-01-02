/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using LINQPad;
using LINQPad.Extensibility.DataContext;
using LinqToVfpLinqPadDriver.Schema;
using VfpClient;

namespace LinqToVfpLinqPadDriver {
    public class VfpDynamicDataContextDriver : DynamicDataContextDriver {
        public override string Name {
            get {
                return "LINQ to VFP";
            }
        }

        public override string Author {
            get { return "Tom Brothers"; }
        }

        public override string GetConnectionDescription(IConnectionInfo connectionInfo) {
            var builder = new VfpConnectionStringBuilder(connectionInfo.DatabaseInfo.CustomCxString);
            return builder.DataSource;
        }

        public override ParameterDescriptor[] GetContextConstructorParameters(IConnectionInfo connectionInfo) {
            return new[] { new ParameterDescriptor("connectionString", "System.String") };
        }

        public override object[] GetContextConstructorArguments(IConnectionInfo connectionInfo) {
            return new object[] { connectionInfo.DatabaseInfo.CustomCxString };
        }

        public override IDbConnection GetIDbConnection(IConnectionInfo connectionInfo) {
            var connection = new VfpConnection(connectionInfo.DatabaseInfo.CustomCxString);

            return connection;
        }

        public override void InitializeContext(IConnectionInfo connectionInfo, object context, QueryExecutionManager executionManager) {
            object provider = context.GetType().GetProperties().First(x => x.Name == "Provider").GetValue(context, null);
            provider.GetType().GetProperty("Log").SetValue(provider, executionManager.SqlTranslationWriter, null);

            base.InitializeContext(connectionInfo, context, executionManager);
        }

        public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo) {
            yield return "IQToolkit.dll";
            yield return "IQToolkit.Data.dll";
            yield return "IQToolkitContrib.dll";
            yield return "LINQtoVFP.dll";
            yield return "VfpClient3.5.dll";
        }

        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo) {
            yield return "IQToolkit";
            yield return "IQToolkit.Data.Mapping";
            yield return "LinqToVfp";
            yield return "VfpClient";
        }

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(IConnectionInfo connectionInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName) {
            return SchemaBuilder.GetSchemaAndBuildAssembly(connectionInfo, assemblyToBuild, ref nameSpace, ref typeName);
        }

        public override bool ShowConnectionDialog(IConnectionInfo connectionInfo, bool isNewConnection) {
            var dialog = new OptionsDialog(connectionInfo);
            connectionInfo.DatabaseInfo.Provider = "VfpClient";

            return dialog.ShowDialog() == true;
        }

        public override ICustomMemberProvider GetCustomDisplayMemberProvider(object objectToWrite) {
            if (objectToWrite != null) {
                object attribute = objectToWrite.GetType().GetCustomAttributes(false).Where(a => a.GetType().FullName == "LinqToVfpLinqPad.EntityAttribute").FirstOrDefault();

                if (attribute != null) {
                    return new EntityMemberProvider(objectToWrite);
                }
            }

            return null;
        }
    }
}