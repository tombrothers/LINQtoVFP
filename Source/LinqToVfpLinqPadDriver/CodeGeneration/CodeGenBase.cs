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
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Text;
using LINQPad.Extensibility.DataContext.DbSchema;

namespace LinqToVfpLinqPadDriver.CodeGeneration {
    public abstract class CodeGenBase {
        private readonly StringBuilder _output = new StringBuilder();
        private readonly bool _singularize;
        private readonly PluralizationService _pluralizationService = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

        protected CodeGenBase(bool singularize) {
            _singularize = singularize;
        }

        protected string GetEntityClassName(SchemaObject schemaObject) {
            return _singularize ? _pluralizationService.Singularize(schemaObject.DotNetName) : schemaObject.DotNetName;
        }

        protected void WriteTab(int count = 1) {
            Write(new string('\t', count));
        }

        protected void WriteLine(object value = null) {
            value = (value ?? string.Empty) + Environment.NewLine;
            Write(value);
        }

        protected void Write(object value) {
            _output.Append(value);
        }

        public override string ToString() {
            return _output.ToString();
        }
    }
}