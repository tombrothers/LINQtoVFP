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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using IQToolkit.Data.Common;
using IQToolkit.Data;

namespace LinqToVfp.Northwind.Tests.EntityProviderTests {
    public abstract class ATranslationTests : AEntityProviderTests {
        Dictionary<string, string> baselines;

        protected void TestQuery(IQueryable query) {
            TestQuery((DbEntityProvider)query.Provider, query.Expression, false);
        }

        protected void TestQuery(IQueryable query, string baselineKey) {
            TestQuery((DbEntityProvider)query.Provider, query.Expression, false);
        }

        protected void TestQuery(Expression<Func<object>> query) {
            TestQuery(this.Northwind.Provider, query.Body, false);
        }

        protected void TestQueryFails(IQueryable query) {
            TestQuery((DbEntityProvider)query.Provider, query.Expression, true);
        }

        protected void TestQueryFails(Expression<Func<object>> query) {
            TestQuery(this.Northwind.Provider, query.Body, true);
        }

        protected void TestQuery(DbEntityProvider pro, Expression query, bool expectedToFail) {
            if (this.baselines == null) {
                XDocument doc = XDocument.Parse(Properties.Resources.NorthwindTranslationXml);
                this.baselines = doc.Root.Elements("baseline").ToDictionary(e => (string)e.Attribute("key"), e => e.Value);
            }

            if (query.NodeType == ExpressionType.Convert && query.Type == typeof(object)) {
                query = ((UnaryExpression)query).Operand; // remove box
            }

            string queryText = null;

            queryText = pro.GetQueryText(query);

            Exception caught = null;
            try {
                object result = pro.Execute(query);
                IEnumerable seq = result as IEnumerable;
                if (seq != null) {
                    // iterate results
                    foreach (var item in seq) {
                    }
                }
                else {
                    IDisposable disposable = result as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
            }
            catch (Exception e) {
                caught = e;
                if (!expectedToFail) {
                    throw;
                }
            }

            if (caught == null && expectedToFail) {
                throw new Exception("Table succeeded when expected to fail");
            }


            string baseline = null;
            if (this.baselines != null && this.baselines.TryGetValue(this.TestContext.TestName, out baseline)) {
                string trimAct = TrimExtraWhiteSpace(queryText).Trim();
                string trimBase = TrimExtraWhiteSpace(baseline).Trim();

                if (trimAct != trimBase) {
                    this.TestContext.WriteLine("Table translation does not match baseline:");
                    this.TestContext.WriteLine(queryText);
                    this.TestContext.WriteLine("---- current ----");
                    this.TestContext.WriteLine(trimAct);
                    this.TestContext.WriteLine("---- baseline ----");
                    this.TestContext.WriteLine(trimBase);
                    throw new Exception("Translation differed from baseline.");
                }
            }

            if (baseline == null && this.baselines != null) {
                throw new Exception("No baseline");
            }
        }

        private string TrimExtraWhiteSpace(string s) {
            StringBuilder sb = new StringBuilder();
            bool lastWasWhiteSpace = false;
            foreach (char c in s) {
                bool isWS = char.IsWhiteSpace(c);
                if (!isWS || !lastWasWhiteSpace) {
                    if (isWS)
                        sb.Append(' ');
                    else
                        sb.Append(c);
                    lastWasWhiteSpace = isWS;
                }
            }
            return sb.ToString();
        }
    }
}
