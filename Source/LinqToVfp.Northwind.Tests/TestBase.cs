using System.Diagnostics;
/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VfpClient;

namespace LinqToVfp.Northwind.Tests {
    [TestClass]
    public abstract class TestBase {
        public TestContext TestContext { get; set; }
        protected static string DbcPath {
            get {
                return Path.GetDirectoryName(Path.GetFullPath("northwind.dbc"));
            }
        }

        protected static string BackupPath {
            get {
                return Path.Combine(DbcPath, "Backup");
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context) {
            File.WriteAllText("NorthwindTranslation_VfpQueryProvider.base", Properties.Resources.NorthwindTranslationXml);
            File.WriteAllBytes("NorthwindVfp.zip", Properties.Resources.NorthwindVfpZip);
            File.WriteAllBytes("DecimalTable.zip", Properties.Resources.DecimalTableZip);

            var zip = new FastZip();
            zip.ExtractZip("NorthwindVfp.zip", context.TestDeploymentDir, string.Empty);
            zip.ExtractZip("DecimalTable.zip", Path.Combine(context.TestDeploymentDir, "Decimal"), string.Empty);

            Directory.CreateDirectory(BackupPath);
            CopyData(DbcPath, BackupPath);

            VfpClientTracing.Tracer = new TraceSource("VfpClient", SourceLevels.Information);
            VfpClientTracing.Tracer.Listeners.Add(new TestContextTraceListener(context));
        }

        protected static void CopyData(string sourcePath, string destinationPath) {
            var tables = new[] { "customers", "orders" };
            var extensions = new[] { ".dbf", ".cdx" };

            for (int tableIndex = 0, tableTotal = tables.Length; tableIndex < tableTotal; tableIndex++) {
                for (int extensionIndex = 0, extensionTotal = extensions.Length; extensionIndex < extensionTotal; extensionIndex++) {
                    var file = tables[tableIndex] + extensions[extensionIndex];
                    File.Copy(Path.Combine(sourcePath, file), Path.Combine(destinationPath, file), true);
                }
            }
        }
        private class TestContextTraceListener : TraceListener {
            private readonly TestContext _context;

            public TestContextTraceListener(TestContext context) {
                _context = context;
            }

            public override void Write(string message) {
                _context.WriteLine(message);
            }

            public override void WriteLine(string message) {
                _context.WriteLine(message.Replace("{", "{{").Replace("}", "}}"));
            }
        }
    }
}