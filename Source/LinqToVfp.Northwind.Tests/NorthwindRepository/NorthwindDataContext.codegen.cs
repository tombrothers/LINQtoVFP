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
using System.IO;
using IQToolkitContrib;
using LinqToVfp;

namespace LinqToVfp.Northwind.Tests.NorthwindRepository {
    public partial class NorthwindDataContext : IQToolkitContrib.DataContext {
        public VfpQueryProvider Provider { get; private set; }
            
        public NorthwindDataContext(string connectionString)
            : this(CreateRepository(connectionString)) {
        }

        public NorthwindDataContext(IRepository repository)
            : base(repository) {
            
            if (repository is DbEntityRepository) {
                this.Provider = ((DbEntityRepository)repository).Provider as VfpQueryProvider;
            }            
        }

        private static DbEntityRepository CreateRepository(string connectionString) {
            VfpQueryProvider provider = VfpQueryProvider.Create(connectionString, null);

            Type type = typeof(NorthwindDataContext);

            // path of the xml file in the dll
            string xmlPath = Path.GetFileNameWithoutExtension(type.FullName) + ".Mapping.xml";

            using (StreamReader streamReader = new StreamReader(type.Assembly.GetManifestResourceStream(xmlPath))) {
                provider = provider.New(VfpXmlMapping.FromXml(streamReader.ReadToEnd()));
            }

            return new DbEntityRepository(provider);
        }
    }
}
