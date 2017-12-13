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
using LinqToVfp.ExpressionRewriters;

namespace LinqToVfp {
    public partial class VfpLanguage {
        private class VfpLinguist : IQToolkit.Data.Common.QueryLinguist {
            private VfpLanguage language;

            public VfpLinguist(VfpLanguage language, IQToolkit.Data.Common.QueryTranslator translator)
                : base(language, translator) {
                this.language = language;
            }

            public override Expression Translate(Expression expression) {
                bool hasRowNumberExpression;
                
                // fix up any order-by's
                expression = OrderByRewriter.Rewrite(this.Language, expression);

                expression = XmlToCursorExpressionRewriter.Rewrite(expression);

                // remove redundant layers again before cross apply rewrite
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);

                expression = OneBasedIndexRewriter.Rewrite(expression);

                // convert cross-apply and outer-apply joins into inner & left-outer-joins if possible
                var rewritten = CrossApplyRewriter.Rewrite(this.language, expression);

                // convert cross joins into inner joins
                rewritten = CrossJoinRewriter.Rewrite(rewritten);

                if (rewritten != expression) {
                    expression = rewritten;
                    // do final reduction
                    expression = UnusedColumnRemover.Remove(expression);
                    expression = RedundantSubqueryRemover.Remove(expression);
                    expression = RedundantJoinRemover.Remove(expression);
                    expression = RedundantColumnRemover.Remove(expression);
                }
                                
                // convert skip/take info into RowNumber pattern
                expression = SkipToRowNumberRewriter.Rewrite(expression, out hasRowNumberExpression);
                
                expression = SkipToNestedOrderByRewriter.Rewrite(this.Language, expression);

                expression = UnusedColumnRemover.Remove(expression);

                expression = WhereCountComparisonRewriter.Rewrite(expression);

                if (!hasRowNumberExpression) {
                    expression = OrderByRewriter.Rewrite(this.Language, expression);
                    expression = RedundantSubqueryRemover.Remove(expression);
                }
                                
                expression = VfpCrossJoinIsolator.Isolate(expression);
                expression = ConditionalImmediateIfNullRemover.Remove(expression);
                expression = XmlToCursorJoinRewriter.Rewrite(expression);

                return expression;
            }

            public override string Format(Expression expression) {
                return VfpFormatter.Format(expression);
            }
            
            public override Expression Parameterize(Expression expression) {
                return VfpParameterizer.Parameterize(this.Language, expression);
            }
        }
    }
}
