/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using IQToolkit;
using IQToolkit.Data.Common;

namespace LinqToVfp {
    public class VfpBasicMapper : BasicMapper {
        public VfpBasicMapper(BasicMapping mapping, QueryTranslator translator)
            : base(mapping, translator) {
        }

        protected override DeclarationCommand GetGeneratedIdCommand(MappingEntity entity, List<MemberInfo> members, Dictionary<MemberInfo, Expression> map) {
            var columns = new List<ColumnDeclaration>();
            var decls = new List<VariableDeclaration>();
            var alias = new TableAlias();

            var orderBy = new List<OrderExpression> {
                new OrderExpression(OrderType.Ascending, Expression.Constant(1))
            };

            foreach(var member in members) {
                var genId = Translator.Linguist.Language.GetGeneratedIdExpression(member);

                var colType = GetColumnType(entity, member);
                columns.Add(new ColumnDeclaration(member.Name, genId, colType));
                decls.Add(new VariableDeclaration(member.Name, colType, new ColumnExpression(genId.Type, colType, alias, member.Name)));
                if(map != null) {
                    var vex = new VariableExpression(member.Name, TypeHelper.GetMemberType(member), colType);
                    map.Add(member, vex);
                }
            }

            var attributeMapping = entity as VfpAttributeMapping.AttributeMappingEntity;
            var tableId = entity.TableId;

            if(attributeMapping != null) {
                tableId = attributeMapping.TableName;
            }

            var from = new TableExpression(alias, entity, tableId);
            Expression take = Expression.Constant(1);

            var select = new SelectExpression(alias, columns, from, null, new ReadOnlyCollection<OrderExpression>(orderBy), null, false, null, take, false);

            return new DeclarationCommand(decls, select);
        }
    }
}