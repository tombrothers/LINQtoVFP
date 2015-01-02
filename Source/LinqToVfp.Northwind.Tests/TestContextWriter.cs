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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToVfp.Northwind.Tests {
    public class TestContextWriter : TextWriter {
        TestContext context;

        public TestContextWriter(TestContext context) {
            this.context = context;
        }

        public override Encoding Encoding {
            get { return Encoding.Unicode; }
        }

        public override void WriteLine(string format, object arg0) {
            this.context.WriteLine(format, arg0);
        }

        public override void WriteLine() {
            this.context.WriteLine("");
        }

        public override void WriteLine(bool value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(char value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(char[] buffer) {
            throw new NotImplementedException();
        }

        public override void WriteLine(char[] buffer, int index, int count) {
            throw new NotImplementedException();
        }

        public override void WriteLine(decimal value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(double value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(float value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(int value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(long value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(object value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(string format, object arg0, object arg1) {
            this.context.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2) {
            this.context.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg) {
            this.context.WriteLine(format, arg);
        }

        public override void WriteLine(string value) {
            this.context.WriteLine(value);
        }

        public override void WriteLine(uint value) {
            this.context.WriteLine("{0}", value);
        }

        public override void WriteLine(ulong value) {
            this.context.WriteLine("{0}", value);
        }
    }

}
