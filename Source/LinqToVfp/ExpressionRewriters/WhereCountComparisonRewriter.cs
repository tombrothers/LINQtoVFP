/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Collections.Generic;
using System.Linq.Expressions;
using IQToolkit.Data.Common;

namespace LinqToVfp.ExpressionRewriters {
    /*
     * This class was created to fix a problem the following linq statement.
     * 
     * var list = this.Northwind.Customers.Where(d => d.Orders.Count > 0).ToList();
     * 
     * 
     * The linq statement originally generated the following invalid select statement.
     
SELECT t0.City, t0.CompanyName, t0.ContactName, t0.Country, t0.CustomerID, t0.Phone ;
FROM Customers AS t0 ;
WHERE (( ;
  SELECT COUNT(*) ;
  FROM Orders AS t1 ;
  WHERE (t1.CustomerID = t0.CustomerID) ;
  ) > 0)

     
     * This class changed the expression tree so that is would result in the following sql statement.
     * 
      
     SELECT t0.City, t0.CompanyName, t0.ContactName, t0.Country, t0.CustomerID, t0.Phone ;
FROM Customers AS t0 ;
INNER JOIN ( ;
  SELECT ( ;
    SELECT COUNT(*) ;
    FROM Orders AS t2 ;
    WHERE (t2.CustomerID = t1.CustomerID) ;
    ) AS CountValue, t1.CustomerID ;
  FROM Customers AS t1 ;
  ) AS t1 ;
  ON (t1.CustomerID = t0.CustomerID) ;
WHERE (t1.CountValue > 0) 
      
     
     */
    internal class WhereCountComparisonRewriter : VfpExpressionVisitor {
        internal static Expression Rewrite(Expression expression) {
            return new WhereCountComparisonRewriter().Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select) {
            return base.VisitSelect(this.GetCountSelectExpression(select) ?? select);
        }

        private SelectExpression GetCountSelectExpression(SelectExpression select) {
            BinaryExpression binaryExpression = select.Where as BinaryExpression;

            if (binaryExpression != null) {
                ScalarExpression scalarExpression = binaryExpression.Left as ScalarExpression;

                if (scalarExpression != null) {
                    SelectExpression selectCount = scalarExpression.Select as SelectExpression;

                    if (selectCount != null && selectCount.Columns.Count == 1) {
                        AggregateExpression aggregateExpression = (AggregateExpression)selectCount.Columns[0].Expression;

                        if (aggregateExpression != null && aggregateExpression.AggregateName == "Count") {
                            BinaryExpression where = selectCount.Where as BinaryExpression;

                            if (where != null) {
                                ColumnExpression columnExpression = where.Left as ColumnExpression;

                                if (columnExpression != null) {
                                    TableAlias tableAlias = new TableAlias();
                                    TableExpression tableExpression = (TableExpression)select.From;
                                    tableExpression = new TableExpression(tableAlias, tableExpression.Entity, tableExpression.Name);

                                    columnExpression = new ColumnExpression(columnExpression.Type, columnExpression.QueryType, tableAlias, columnExpression.Name);
                                    ColumnDeclaration columnDeclaration = new ColumnDeclaration(string.Empty, columnExpression, columnExpression.QueryType);

                                    BinaryExpression where2 = Expression.MakeBinary(where.NodeType, where.Left, columnExpression);
                                    selectCount = new SelectExpression(selectCount.Alias, selectCount.Columns, selectCount.From, where2);

                                    List<ColumnDeclaration> columns = new List<ColumnDeclaration> {
                                        new ColumnDeclaration("CountValue", new ScalarExpression(selectCount.Columns[0].Expression.Type, selectCount), selectCount.Columns[0].QueryType),
                                        columnDeclaration
                                    };
                                    
                                    selectCount = new SelectExpression(tableAlias,
                                                                        columns.ToReadOnly(),
                                                                        tableExpression,
                                                                        null,
                                                                        selectCount.OrderBy,
                                                                        null,
                                                                        selectCount.IsDistinct,
                                                                        selectCount.Skip,
                                                                        selectCount.Take,
                                                                        selectCount.IsReverse);

                                    ColumnExpression countValueColumnExpression = new ColumnExpression(selectCount.Columns[0].Expression.Type,
                                                                                                        selectCount.Columns[0].QueryType,
                                                                                                        tableAlias,
                                                                                                        "CountValue");

                                    SelectExpression newSelect = new SelectExpression(select.Alias, 
                                                                                        select.Columns, 
                                                                                        select.From, 
                                                                                        null, 
                                                                                        select.OrderBy, 
                                                                                        select.GroupBy, 
                                                                                        select.IsDistinct, 
                                                                                        select.Skip, 
                                                                                        select.Take, 
                                                                                        select.IsReverse);

                                    JoinExpression joinExpression = new JoinExpression(JoinType.InnerJoin, 
                                                                                        newSelect, selectCount, 
                                                                                        Expression.MakeBinary(ExpressionType.Equal, columnExpression, where.Right));

                                    select = new SelectExpression(newSelect.Alias, 
                                                                    newSelect.Columns, 
                                                                    joinExpression, 
                                                                    Expression.MakeBinary(binaryExpression.NodeType, countValueColumnExpression, binaryExpression.Right), 
                                                                    newSelect.OrderBy, 
                                                                    newSelect.GroupBy, 
                                                                    newSelect.IsDistinct, 
                                                                    newSelect.Skip, 
                                                                    newSelect.Take, 
                                                                    newSelect.IsReverse);
                                    
                                    return select;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
