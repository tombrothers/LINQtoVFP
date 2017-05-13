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
using System.Collections.ObjectModel;
using System.Text;
using LINQPad.Extensibility.DataContext;
using LINQPad.Extensibility.DataContext.DbSchema;

namespace LinqToVfpLinqPadDriver.CodeGeneration {
    public class CodeGenWriter {
        public string GetCode(IConnectionInfo connectionInfo, ReadOnlyCollection<SchemaObject> schemaObjects) {
            var sb = new StringBuilder();

            sb.AppendLine(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IQToolkit;
using IQToolkit.Data.Mapping;
using LinqToVfp;

namespace LinqToVfpLinqPad {
    public class LazyLoadedAttribute : Attribute { }
	public class EntityAttribute : Attribute { }


");
            sb.AppendLine();

            var singularize = false;
            var singularizeElement = connectionInfo.DriverData.Element("Singularize");

            if (singularizeElement != null) {
                singularize = Convert.ToBoolean(singularizeElement.Value);
            }

            var dataContextTemplate = new DataContextCodeGen(singularize, schemaObjects);
            sb.Append(dataContextTemplate);
            sb.AppendLine();

            var mappingTemplate = new MappingCodeGen(singularize, schemaObjects);

            sb.Append(mappingTemplate);
            sb.AppendLine();

            for (int index = 0, total = schemaObjects.Count; index < total; index++) {
                var template = new EntityCodeGen(singularize, schemaObjects[index]);

                sb.AppendLine();
                sb.Append(template);
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}