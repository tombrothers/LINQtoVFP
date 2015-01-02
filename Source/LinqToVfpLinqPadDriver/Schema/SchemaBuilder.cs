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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using LINQPad.Extensibility.DataContext;
using LINQPad.Extensibility.DataContext.DbSchema;
using LinqToVfpLinqPadDriver.CodeGeneration;
using Microsoft.CSharp;
using VfpClient;

namespace LinqToVfpLinqPadDriver.Schema {
    public static class SchemaBuilder {
        public static List<ExplorerItem> GetSchema(VfpConnection connection, IConnectionInfo connectionInfo) {
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }

            if (connectionInfo == null) {
                throw new ArgumentNullException("connectionInfo");
            }

            var reader = new SchemaReader(connection, connectionInfo);
            var database = reader.GetDatabase();

            return database.GetExplorerSchema();
        }

        public static List<ExplorerItem> GetSchemaAndBuildAssembly(IConnectionInfo connectionInfo,
                                                                   AssemblyName name,
                                                                   ref string nameSpace,
                                                                   ref string typeName) {
            if (connectionInfo == null) {
                throw new ArgumentNullException("connectionInfo");
            }

            var connection = (VfpConnection)connectionInfo.DatabaseInfo.GetConnection();
            var schema = GetSchema(connection, connectionInfo);
            var writer = new CodeGenWriter();
            var code = writer.GetCode(connectionInfo, GetTableAndViews(schema));

            // Compile the code into the assembly, using the assembly name provided:
            BuildAssembly(code, name);

            nameSpace = "LinqToVfpLinqPad";
            typeName = "DataContext";

            return schema;
        }

        public static ReadOnlyCollection<SchemaObject> GetTableAndViews(List<ExplorerItem> schema) {
            var tables = schema.Where(x => x.Tag is Table)
                               .Select(x => x.Tag as SchemaObject)
                               .ToList();

            var categories = schema.Where(x => x.Kind == ExplorerItemKind.Category);

            foreach (var category in categories) {
                tables.AddRange(category.Children.Where(x => x.Tag is View).Select(x => x.Tag as SchemaObject).ToList());
            }

            return tables.AsReadOnly();
        }

        public static void BuildAssembly(string code, AssemblyName name) {
            // Use the CSharpCodeProvider to compile the generated code:
            CompilerResults results;

            var path = Path.GetDirectoryName(typeof(VfpDynamicDataContextDriver).Assembly.Location);
            var references = new[] { 
                "System.dll", 
                "System.Data.dll", 
                "System.Core.dll", 
                Path.Combine(path, "IQToolkit.dll"), 
                Path.Combine(path, "IQToolkit.Data.dll"), 
                Path.Combine(path, "IQToolkitContrib.dll"), 
                Path.Combine(path, "LINQtoVFP.dll"),
                Path.Combine(path, "VfpClient3.5.dll") 
            };

            using (var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } })) {
                var options = new CompilerParameters(references, name.CodeBase, true);

                results = codeProvider.CompileAssemblyFromSource(options, code);
            }

            if (results.Errors.Count > 0) {
                throw new Exception("Cannot compile typed context: " + results.Errors[0].ErrorText + " (line " + results.Errors[0].Line + ")");
            }
        }
    }
}