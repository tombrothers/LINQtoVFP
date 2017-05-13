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
using System.Data.OleDb;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VfpClient;

namespace LinqToVfp.Northwind.Tests.CharByteArray {
    [TestClass]
    public class CharByteArrayTests : TestBase {
        [TestMethod]
        public void CharByteArrayStringMappingTest() {
            File.WriteAllBytes("CharByteArrayData.zip", Properties.Resources.CharByteArrayDataZip);

            FastZip zip = new FastZip();
            zip.ExtractZip("CharByteArrayData.zip", this.TestContext.TestDeploymentDir, string.Empty);

            string connectionString = @"Provider=VFPOLEDB;Data Source=" + this.TestContext.TestDeploymentDir;
            using (var connection = new VfpConnection(connectionString)) {
                var provider = new VfpQueryProvider(connection, VfpLanguage.Default, new VfpImplicitMapping(), VfpQueryPolicy.Default);
                provider.AutoRightTrimStrings = true;
                provider.Log = Console.Out;
                var table = provider.GetTable<MenuList>("MenuList");
                                
                var newItem = new MenuList { Menu_Name = "blah", Alt_Name = "blahAlt", MenuActive = "N", MenuListId = 99 };
                table.Insert(newItem);
                var existingItem = table.Where(x => x.MenuListId == newItem.MenuListId).OrderBy(x => x.MenuListId).Single();

                Assert.AreEqual(newItem.Alt_Name, existingItem.Alt_Name);
                Assert.AreEqual(newItem.Menu_Name, existingItem.Menu_Name);
                Assert.AreEqual(newItem.MenuActive, existingItem.MenuActive);
                Assert.AreEqual(newItem.MenuListId, existingItem.MenuListId);
            }
        }
    }
}
