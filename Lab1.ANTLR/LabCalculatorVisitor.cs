using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabCalculator
{
    public class LabCalculatorVisitor : LabCalculatorBaseVisitor<double>
    {
        Dictionary<string, double> tableIdentifier = new Dictionary<string, double>();
        private Dictionary<string, string> cellExpressions;

        public LabCalculatorVisitor(Dictionary<string, string> cellExpressions)
        {
            this.cellExpressions = cellExpressions;
        }

        public override double VisitCompileUnit(LabCalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(LabCalculatorParser.NumberExprContext context)
        {
            if (double.TryParse(context.GetText(), out var result))
            {
                Debug.WriteLine(result);
                return result;
            }
            else
            {
               
                return double.NaN;
            }
        }

        public override double VisitIdentifierExpr(LabCalculatorParser.IdentifierExprContext context)
        {
            var result = context.GetText();
            double value;
            if (tableIdentifier.TryGetValue(result.ToString(), out value))
            {
                return value;
            }
            else
            {
                return 0.0;
            }
        }

        public override double VisitParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }
        // 8,9 
        public override double VisitRelationalExpr(LabCalculatorParser.RelationalExprContext context)
        {
            double left = WalkLeft(context);
            double right = WalkRight(context);

            switch (context.operatorToken.Type)
            {
                case LabCalculatorLexer.LT:
                    return left < right ? 1.0 : 0.0;
                case LabCalculatorLexer.LEQ:
                    return left <= right ? 1.0 : 0.0;
                case LabCalculatorLexer.GT:
                    return left > right ? 1.0 : 0.0;
                case LabCalculatorLexer.GEQ:
                    return left >= right ? 1.0 : 0.0;
                case LabCalculatorLexer.EQ:
                    return left == right ? 1.0 : 0.0;
                case LabCalculatorLexer.NEQ:
                    return left != right ? 1.0 : 0.0;
                default:
                    throw new NotSupportedException("Not supported operation: " + context.operatorToken.Text);
            }
        }
        //10
        public override double VisitNotExpr(LabCalculatorParser.NotExprContext context)
        {
            double value = Visit(context.expression());
            return value == 0.0 ? 1.0 : 0.0;
        }
        //11
        public override double VisitLogicalExpr(LabCalculatorParser.LogicalExprContext context)
        {
            double left = WalkLeft(context);
            double right = WalkRight(context);

            switch (context.operatorToken.Type)
            {
                case LabCalculatorLexer.OR:
                    return (left != 0.0 || right != 0.0) ? 1.0 : 0.0;
                case LabCalculatorLexer.AND:
                    return (left != 0.0 && right != 0.0) ? 1.0 : 0.0;
                default:
                    throw new NotSupportedException("Not supported operation: " + context.operatorToken.Text);
            }
        }

        //4
        public override double VisitExponentialExpr(LabCalculatorParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            Debug.WriteLine("{0} ^ {1}", left, right);
            return Math.Pow(left, right);
        }
        //1
        public override double VisitAdditiveExpr(LabCalculatorParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == LabCalculatorLexer.ADD)
            {
                Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else
            {
                Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }
        //1
        public override double VisitMultiplicativeExpr(LabCalculatorParser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == LabCalculatorLexer.MULTIPLY)
            {
                Debug.WriteLine("{0} * {1}", left, right);
                return left * right;
            }
            else
            {
                Debug.WriteLine("{0} / {1}", left, right);
                return left / right;
            }
        }


        //2
        public override double VisitModDivExpr(LabCalculatorParser.ModDivExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.MOD)
            {
                return left % right;
            }
            else if (context.operatorToken.Type == LabCalculatorLexer.DIV)
            {
                if (right != 0)
                {
                    return (int)(left / right);
                }
                else
                {
                    throw new DivideByZeroException("Division by zero is not allowed.");
                }
            }
            else
            {
                return left / right;
            }
        }
        //3
        public override double VisitUnaryMinusExpr(LabCalculatorParser.UnaryMinusExprContext context)
        {
            return -Visit(context.expression());
        }
        //5
        public override double VisitIncrementExpr(LabCalculatorParser.IncrementExprContext context)
        {
            return Visit(context.expression()) + 1;
        }
        //5
        public override double VisitDecrementExpr(LabCalculatorParser.DecrementExprContext context)
        {
            return Visit(context.expression()) - 1;
        }
        //6
        public override double VisitMaxExpr(LabCalculatorParser.MaxExprContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            return Math.Max(left, right);
        }
        //6
        public override double VisitMinExpr(LabCalculatorParser.MinExprContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            return Math.Min(left, right);
        }
        //7
        public override double VisitMultiMaxExpr(LabCalculatorParser.MultiMaxExprContext context)
        {
            var values = context.expression().Select(expr => Visit(expr));
            return values.Max();
        }
        //7
        public override double VisitMultiMinExpr(LabCalculatorParser.MultiMinExprContext context)
        {
            var values = context.expression().Select(expr => Visit(expr));
            return values.Min();
        }
        //12
        public override double VisitEqvExpr(LabCalculatorParser.EqvExprContext context)
        {
            double left = Visit(context.expression(0));
            double right = Visit(context.expression(1));
            return (left == right) ? 1.0 : 0.0;
        }

        private double WalkLeft(LabCalculatorParser.ExpressionContext context)
        {
            var leftContext = context.GetRuleContext<LabCalculatorParser.ExpressionContext>(0);
            return leftContext != null ? Visit(leftContext) : 0.0;
        }

        private double WalkRight(LabCalculatorParser.ExpressionContext context)
        {
            var rightContext = context.GetRuleContext<LabCalculatorParser.ExpressionContext>(1);
            return rightContext != null ? Visit(rightContext) : 0.0;
        }

        public override double VisitCellReferenceExpr(LabCalculatorParser.CellReferenceExprContext context)
        {
            var cellId = context.GetText();
            string expression;
            if (cellExpressions.TryGetValue(cellId, out expression))
            {
                return Calculator.Evaluate(expression, cellExpressions);
            }
            else
            {
                return 0.0;
            }
        }
    }
}
