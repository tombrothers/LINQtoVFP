/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Linq.Expressions;
using System.Reflection;
using IQToolkit;
using IQToolkit.Data.Common;

namespace LinqToVfp {
    public partial class VfpLanguage : QueryLanguage {
        private VfpTypeSystem typeSystem = new VfpTypeSystem();

        public override QueryTypeSystem TypeSystem {
            get { return this.typeSystem; }
        }

        public override string Quote(string name) {
            return name;
        }
        
        public override Expression GetGeneratedIdExpression(MemberInfo member) {
            return new FunctionExpression(TypeHelper.GetMemberType(member), "GETAUTOINCVALUE()", null);
        }

        public override QueryLinguist CreateLinguist(QueryTranslator translator) {
            return new VfpLinguist(this, translator);
        }
        
        private static VfpLanguage defaultInstance;

        public static VfpLanguage Default {
            get {
                if (defaultInstance == null) {
                    System.Threading.Interlocked.CompareExchange(ref defaultInstance, new VfpLanguage(), null);
                }

                return defaultInstance;
            }
        }
    }
}
