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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using IQToolkit.Data.Common;
using IQToolkit.Data;

namespace LinqToVfp {
    internal class VfpFormatter : SqlFormatter {
        protected VfpFormatter(QueryLanguage language)
            : base(language) {
        }

        public static new string Format(Expression expression) {
            return Format(expression, new VfpLanguage());
        }

        public static string Format(Expression expression, QueryLanguage language) {
            VfpFormatter formatter = new VfpFormatter(language);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        protected override void WriteParameterName(string name) {
            this.Write(name);
        }

        protected override Expression VisitJoin(JoinExpression join) {
            this.VisitJoinLeft(join.Left);
            this.WriteLine(Indentation.Same);
            switch (join.Join) {
                case JoinType.CrossJoin:
                    this.Write(", ");
                    break;
                case JoinType.InnerJoin:
                    this.Write("INNER JOIN ");
                    break;
                case JoinType.CrossApply:
                    this.Write("CROSS APPLY ");
                    break;
                case JoinType.OuterApply:
                    this.Write("OUTER APPLY ");
                    break;
                case JoinType.LeftOuter:
                case JoinType.SingletonLeftOuter:
                    this.Write("LEFT OUTER JOIN ");
                    break;
            }
            this.VisitJoinRight(join.Right);
            if (join.Condition != null) {
                this.WriteLine(Indentation.Inner);
                this.Write("ON ");
                this.VisitPredicate(join.Condition);
                this.Indent(Indentation.Outer);
            }
            return join;
        }

        protected override Expression VisitColumn(ColumnExpression column) {
            if (column.Name.Equals("recno()", StringComparison.InvariantCultureIgnoreCase)) {
                column = new ColumnExpression(column.Type, column.QueryType, null, column.Name);
            }

            return base.VisitColumn(column);
        }
        protected override void WriteColumns(System.Collections.ObjectModel.ReadOnlyCollection<ColumnDeclaration> columns) {
            if (columns.Count == 0) {
                this.Write("0");
            }
            else {
                for (int i = 0, n = columns.Count; i < n; i++) {
                    ColumnDeclaration column = columns[i];
                    if (i > 0) {
                        this.Write(", ");
                    }

                    var dbQueryType = column.QueryType as DbQueryType;
                    var vfpDbQueryType = dbQueryType as VfpDbQueryType;
                    var isDecimal = dbQueryType != null && dbQueryType.DbType == System.Data.DbType.Decimal;

                    if (vfpDbQueryType != null && isDecimal) {
                        this.Write(" CAST(");
                        this.VisitValue(column.Expression);
                        this.Write(" AS N(20, ");
                        this.Write(dbQueryType.Scale);
                        this.Write(")) ");

                        if (!string.IsNullOrEmpty(column.Name)) {
                            this.WriteAsColumnName(column.Name);
                        }
                    }
                    else {
                        ColumnExpression c = this.VisitValue(column.Expression) as ColumnExpression;
                        if (!string.IsNullOrEmpty(column.Name) && (c == null || c.Name != column.Name)) {
                            this.Write(" ");
                            this.WriteAsColumnName(column.Name);
                        }
                    }
                }
            }
        }

        // Override of base.VisitUpdate to provide for aliasing the table name to deal with some poorly named fields
        protected override Expression VisitUpdate(UpdateCommand update) {
            this.Write("UPDATE t0 ");

            this.WriteLine(Indentation.Same);
            bool saveHide = this.HideColumnAliases;
            this.HideColumnAliases = true;

            this.Write("SET ");
            for (int i = 0, n = update.Assignments.Count; i < n; i++) {
                ColumnAssignment ca = update.Assignments[i];

                if (i > 0) {
                    this.Write(", ");
                }

                this.Write("t0.");
                this.Visit(ca.Column);
                this.Write(" = ");
                this.Visit(ca.Expression);
            }

            this.WriteLine(Indentation.Same);
            this.Write(" FROM ");
            this.WriteTableName(update.Table.Name);
            this.Write(" t0 ");

            if (update.Where != null) {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.Visit(update.Where);
            }

            this.HideColumnAliases = saveHide;
            return update;
        }

        protected void WriteValue(ConstantExpression c) {
            if (c != null) {
                this.WriteValue(c.Value);
            }
        }

        protected override void WriteValue(object value) {
            if (value != null && value.GetType() == typeof(char[])) {
                char[] valueArray = (char[])value;

                for (int index = 0, total = valueArray.Length; index < total; index++) {
                    this.Write(",'");
                    this.Write(valueArray[index]);
                    this.Write("'");
                }
            }
            else {
                if (value == null) {
                    this.Write("NULL");
                }
                else if (value.GetType().IsEnum) {
                    this.Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
                }
                else {
                    switch (Type.GetTypeCode(value.GetType())) {
                        case TypeCode.Boolean:
                            this.Write(((bool)value) ? ".t." : ".f.");
                            break;
                        case TypeCode.String:
                            this.Write("'");
                            this.Write(value);
                            this.Write("'");
                            break;
                        case TypeCode.Object:
                            throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            string str = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                            if (!str.Contains('.')) {
                                str += ".0";
                            }
                            this.Write(str);
                            break;
                        case TypeCode.DateTime:
                            this.Write(string.Format("CTOT('{0:yyyy-MM-dd}T{0:HH:mm:ss}')", value));
                            break;
                        default:
                            this.Write(value);
                            break;
                    }
                }
            }
        }

        protected override Expression VisitSelect(SelectExpression select) {
            if (select.Take != null && select.OrderBy == null) {
                throw new NotSupportedException("LinqToVfp cannot support the 'take' operation without explicit ordering");
            }

            if (select.Skip != null) {
                if (select.OrderBy == null && select.OrderBy.Count == 0) {
                    throw new NotSupportedException("LinqToVfp cannot support the 'skip' operation without explicit ordering");
                }
                else if (select.Take == null) {
                    throw new NotSupportedException("LinqToVfp cannot support the 'skip' operation without the 'take' operation");
                }
                else {
                    throw new NotSupportedException("LinqToVfp cannot support the 'skip' operation in this query");
                }
            }

            bool hideColumnAliases = this.HideColumnAliases;

            if (select.From is ConditionalExpression) {
                this.HideColumnAliases = true;
            }

            Expression result = base.VisitSelect(select);
            this.HideColumnAliases = hideColumnAliases;

            return result;
        }

        protected override Expression VisitRowNumber(RowNumberExpression rowNumber) {
            this.Write("RecNo()");
            return rowNumber;
        }

        protected override Expression VisitMemberAccess(MemberExpression m) {
            if (m.Member.DeclaringType == typeof(string)) {
                switch (m.Member.Name) {
                    case "Length":
                        this.Write("LEN(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                }
            }
            else if (m.Member.DeclaringType == typeof(DateTime) || m.Member.DeclaringType == typeof(DateTimeOffset)) {
                switch (m.Member.Name) {
                    case "Day":
                        this.Write("DAY(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Month":
                        this.Write("MONTH(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Year":
                        this.Write("YEAR(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Hour":
                        this.Write("HOUR(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Minute":
                        this.Write("MINUTE(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Second":
                        this.Write("SEC(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "DayOfWeek":
                        this.Write("DOW(");
                        this.Visit(m.Expression);
                        this.Write(")-1");
                        return m;
                    case "DayOfYear":
                        this.Write("INT(VAL(SYS(11, ");
                        this.Visit(m.Expression);
                        this.Write(")) - VAL(SYS(11, DATE(YEAR(");
                        this.Visit(m.Expression);
                        this.Write("), 1, 1))) + 1)");
                        return m;
                }
            }

            // for nullable types
            if (m.Member.Name == "Value") {
                this.Visit(m.Expression);
                return m;
            }

            return base.VisitMemberAccess(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m) {
            if (m.Method.DeclaringType == typeof(string)) {
                switch (m.Method.Name) {
                    case "StartsWith":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + '%')");
                        return m;
                    case "EndsWith":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Contains":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + '%')");
                        return m;
                    case "Concat":
                        IList<Expression> args = m.Arguments;
                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit) {
                            args = ((NewArrayExpression)args[0]).Expressions;
                        }

                        for (int i = 0, n = args.Count; i < n; i++) {
                            if (i > 0) {
                                this.Write(" + ");
                            }

                            this.Visit(args[i]);
                        }

                        return m;
                    case "IsNullOrEmpty":
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(" IS NULL OR ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" = '')");
                        return m;
                    case "ToUpper":
                        this.Write("UPPER(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "ToLower":
                        this.Write("LOWER(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "Replace":
                        this.Write("STRTRAN(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Substring":
                        this.Write("SUBSTR(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);

                        if (m.Arguments.Count == 2) {
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                        }

                        this.Write(")");
                        return m;
                    case "Remove":
                        this.Write("STUFF(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        if (m.Arguments.Count == 2) {
                            this.Visit(m.Arguments[1]);
                        }
                        else {
                            this.Write("8000");
                        }

                        this.Write(", '')");
                        return m;
                    case "IndexOf":
                        if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int)) {
                            ////IIF(EMPTY(ATC(x, SUBSTR(y, z + 1))), 0, ATC(x, SUBSTR(y, z + 1))) + z - 1
                            this.Write("IIF(EMPTY(ATC(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(" + 1))), 0, ATC(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(" + 1))) + ");
                            this.Visit(m.Arguments[1]);
                            this.Write(" - 1");
                        }
                        else {
                            this.Write("(ATC(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Object);
                            this.Write(") - 1)");
                        }

                        return m;
                    case "Trim":
                        this.Write("ALLTRIM(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "TrimEnd":
                        this.Write("RTRIM(");
                        this.Visit(m.Object);

                        if (m.Arguments.Count > 0) {
                            this.WriteValue(m.Arguments[0] as ConstantExpression);
                        }
                        this.Write(")");
                        return m;
                    case "TrimStart":
                        this.Write("LTRIM(");
                        this.Visit(m.Object);

                        if (m.Arguments.Count > 0) {
                            this.WriteValue(m.Arguments[0] as ConstantExpression);
                        }
                        this.Write(")");
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(DateTime)) {
                switch (m.Method.Name) {
                    case "op_Subtract":
                        if (m.Arguments[1].Type == typeof(DateTime)) {
                            this.Write("DATEDIFF(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }

                        break;
                    case "AddYears":
                        this.Write("CTOT(DTOC(GOMONTH(");
                        this.Visit(m.Object);
                        this.Write(",12*");
                        this.Visit(m.Arguments[0]);
                        this.Write(")) + ' ' + TTOC(");
                        this.Visit(m.Object);
                        this.Write(", 2))");
                        return m;
                    case "AddMonths":
                        this.Write("CTOT(DTOC(GOMONTH(");
                        this.Visit(m.Object);
                        this.Write(",");
                        this.Visit(m.Arguments[0]);
                        this.Write(")) + ' ' + TTOC(");
                        this.Visit(m.Object);
                        this.Write(", 2))");
                        return m;
                    case "AddDays":
                        // The field could be a Date or a DateTime field.  The "CTOT(TTOC(<<field>>))" will ensure that the adding is against a DateTime.
                        this.Write("CTOT(TTOC(");
                        this.Visit(m.Object);
                        this.Write("))");
                        this.Write("+");
                        this.Visit(m.Arguments[0]);
                        this.Write("*60*60*24");
                        return m;
                    case "AddHours":
                        // The field could be a Date or a DateTime field.  The "CTOT(TTOC(<<field>>))" will ensure that the adding is against a DateTime.
                        this.Write("CTOT(TTOC(");
                        this.Visit(m.Object);
                        this.Write("))");
                        this.Write("+");
                        this.Visit(m.Arguments[0]);
                        this.Write("*60*60");
                        return m;
                    case "AddMinutes":
                        // The field could be a Date or a DateTime field.  The "CTOT(TTOC(<<field>>))" will ensure that the adding is against a DateTime.
                        this.Write("CTOT(TTOC(");
                        this.Visit(m.Object);
                        this.Write("))");
                        this.Write("+");
                        this.Visit(m.Arguments[0]);
                        this.Write("*60");
                        return m;
                    case "AddSeconds":
                        // The field could be a Date or a DateTime field.  The "CTOT(TTOC(<<field>>))" will ensure that the adding is against a DateTime.
                        this.Write("CTOT(TTOC(");
                        this.Visit(m.Object);
                        this.Write("))");
                        this.Write("+");
                        this.Visit(m.Arguments[0]);
                        return m;
                    case "Add":
                        // The field could be a Date or a DateTime field.  The "CTOT(TTOC(<<field>>))" will ensure that the adding is against a DateTime.
                        this.Write("CTOT(TTOC(");
                        this.Visit(m.Object);
                        this.Write("))");
                        this.Write("+");
                        this.Visit(m.Arguments[0]);
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(decimal)) {
                switch (m.Method.Name) {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Remainder":
                        this.Write("(");
                        this.VisitValue(m.Arguments[0]);
                        this.Write(" ");
                        this.Write(GetOperator(m.Method.Name));
                        this.Write(" ");
                        this.VisitValue(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Negate":
                        this.Write("-");
                        this.Visit(m.Arguments[0]);
                        this.Write(string.Empty);
                        return m;
                    case "Ceiling":
                    case "Floor":
                        this.Write(m.Method.Name.ToUpper());
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1) {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", 0)");
                            return m;
                        }
                        else if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int)) {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }

                        break;
                    case "Truncate":
                        this.Write("INT(FLOOR(");
                        this.Visit(m.Arguments[0]);
                        this.Write("))");
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(Math)) {
                switch (m.Method.Name) {
                    case "Abs":
                    case "Acos":
                    case "Asin":
                    case "Atan":
                    case "Cos":
                    case "Exp":
                    case "Sin":
                    case "Tan":
                    case "Sqrt":
                    case "Sign":
                    case "Ceiling":
                    case "Floor":
                        this.Write(m.Method.Name.ToUpper());
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Atan2":
                        this.Write("ATN2(");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Log10":
                        if (m.Arguments.Count == 1) {
                            this.Write(m.Method.Name.ToUpper());
                            this.Write("(EVL(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", 1))");
                            return m;
                        }

                        break;
                    case "Log":
                        this.Write(m.Method.Name.ToUpper());
                        this.Write("(EVL(");
                        this.Visit(m.Arguments[0]);
                        this.Write(", 1))");
                        return m;
                    case "Pow":
                        this.Visit(m.Arguments[0]);
                        this.Write("^");
                        this.Visit(m.Arguments[1]);
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1) {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", 0)");
                            return m;
                        }
                        else if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int)) {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }

                        break;
                    case "Truncate":
                        this.Write("FLOOR(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                }
            }

            if (m.Method.Name == "ToString") {
                if (m.Object.Type != typeof(string)) {
                    this.Write("TRANSFORM(");
                    this.Visit(m.Object);
                    this.Write(")");
                }
                else {
                    this.Visit(m.Object);
                }

                return m;
            }
            else if (!m.Method.IsStatic && m.Method.Name == "CompareTo" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 1) {
                this.Write("ICASE(");
                this.Visit(m.Object);
                this.Write(" = ");
                this.Visit(m.Arguments[0]);
                this.Write(", 0, ");
                this.Visit(m.Object);
                this.Write(" < ");
                this.Visit(m.Arguments[0]);
                this.Write(", -1, 1)");
                return m;
            }
            else if (m.Method.IsStatic && m.Method.Name == "Compare" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 2) {
                this.Write("ICASE(");
                this.Visit(m.Arguments[0]);
                this.Write(" = ");
                this.Visit(m.Arguments[1]);
                this.Write(", 0, ");
                this.Visit(m.Arguments[0]);
                this.Write(" < ");
                this.Visit(m.Arguments[1]);
                this.Write(", -1, 1)");
                return m;
            }

            return base.VisitMethodCall(m);
        }

        protected override NewExpression VisitNew(NewExpression nex) {
            if (nex.Constructor.DeclaringType == typeof(DateTime)) {
                if (nex.Arguments.Count == 3) {
                    this.Write("DATE(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(",");
                    this.Visit(nex.Arguments[1]);
                    this.Write(",");
                    this.Visit(nex.Arguments[2]);
                    this.Write(")");
                    return nex;
                }
                else if (nex.Arguments.Count == 6) {
                    this.Write("DATETIME(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(",");
                    this.Visit(nex.Arguments[1]);
                    this.Write(",");
                    this.Visit(nex.Arguments[2]);
                    this.Write(",");
                    this.Visit(nex.Arguments[3]);
                    this.Write(",");
                    this.Visit(nex.Arguments[4]);
                    this.Write(",");
                    this.Visit(nex.Arguments[5]);
                    this.Write(")");
                    return nex;
                }
            }

            return base.VisitNew(nex);
        }

        protected override Expression VisitBinary(BinaryExpression b) {
            if (b.NodeType == ExpressionType.Power) {
                this.VisitValue(b.Left);
                this.Write("^");
                this.VisitValue(b.Right);
                return b;
            }
            else if (b.NodeType == ExpressionType.Coalesce) {
                int count = 1;
                this.Write("NVL(");
                this.VisitValue(b.Left);
                this.Write(", ");
                Expression right = b.Right;
                while (right.NodeType == ExpressionType.Coalesce) {
                    this.Write("NVL(");
                    BinaryExpression rb = (BinaryExpression)right;
                    this.VisitValue(rb.Left);
                    this.Write(", ");
                    right = rb.Right;
                    count++;
                }

                this.VisitValue(right);
                this.Write(string.Empty.PadRight(count, ')'));
                return b;
            }
            else if (b.NodeType == ExpressionType.LeftShift) {
                this.Write("BITLSHIFT(");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.RightShift) {
                this.Write("BITRSHIFT(");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.And && !this.IsBoolean(b.Left.Type)) {
                this.Write("BITAND(");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.Or && !this.IsBoolean(b.Left.Type)) {
                this.Write("BITOR(");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.ExclusiveOr) {
                this.Write("BITXOR(");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }

            return base.VisitBinary(b);
        }

        protected override Expression VisitUnary(UnaryExpression u) {
            string op = this.GetOperator(u);
            switch (u.NodeType) {
                case ExpressionType.Not:
                    if (IsBoolean(u.Operand.Type) || op.Length > 1) {
                        this.Write(op);
                        this.Write(" ");
                        this.VisitPredicate(u.Operand);
                    }
                    else {
                        this.Write("BITNOT(");
                        this.VisitValue(u.Operand);
                        this.Write(")");
                    }

                    break;
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    this.Write(op);
                    this.VisitValue(u.Operand);
                    break;
                case ExpressionType.UnaryPlus:
                    this.VisitValue(u.Operand);
                    break;
                case ExpressionType.Convert:
                    // ignore conversions for now
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }

        protected override Expression VisitPredicate(Expression expr) {
            this.Visit(expr);
            return expr;
        }

        protected override Expression VisitConditional(ConditionalExpression c) {
            if (this.IsPredicate(c.Test)) {
                this.Write("ICASE(");
                this.VisitPredicate(c.Test);
                this.Write(", ");
                this.VisitValue(c.IfTrue);
                Expression ifFalse = c.IfFalse;
                while (ifFalse != null && ifFalse.NodeType == ExpressionType.Conditional) {
                    ConditionalExpression fc = (ConditionalExpression)ifFalse;
                    this.Write(", ");
                    this.VisitPredicate(fc.Test);
                    this.Write(", ");
                    this.VisitValue(fc.IfTrue);
                    ifFalse = fc.IfFalse;
                }

                if (ifFalse != null) {
                    this.Write(", ");
                    this.VisitValue(ifFalse);
                }

                this.Write(")");
            }
            else {
                this.Write("iif(");
                this.VisitValue(c.Test);
                this.Write(", ");
                this.VisitValue(c.IfFalse);
                this.Write(", ");
                this.VisitValue(c.IfTrue);
                this.Write(")");
            }

            return c;
        }

        protected override Expression VisitSource(Expression source) {
            if (source.NodeType == ExpressionType.Conditional) {
                return this.VisitConditional((ConditionalExpression)source);
            }

            return base.VisitSource(source);
        }


        protected override Expression Visit(Expression expression) {
            if (expression.NodeType.IsVfpExpression()) {
                switch ((VfpExpressionType)expression.NodeType) {
                    case VfpExpressionType.XmlToCursor:
                        return this.VisitXmlToCursor((XmlToCursorExpression)expression);
                }
            }

            return base.Visit(expression);
        }

        protected Expression VisitXmlToCursor(XmlToCursorExpression expression) {
            this.Write("XmlToCursor(");
            this.Visit(expression.Xml);
            this.Write(", ");
            this.Visit(expression.CursorName);
            this.Write(")");

            return expression;
        }
    }
}
