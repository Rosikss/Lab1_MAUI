﻿using Antlr4.Runtime;
using System.Diagnostics;

namespace LabCalculator
{
    public static class Calculator
    {
        public static double Evaluate(string expression, Dictionary<string, string> cellExpressions)
        {
            try
            {
                var lexer = new LabCalculatorLexer(new AntlrInputStream(expression));
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(new ThrowExceptionErrorListener());

                var tokens = new CommonTokenStream(lexer);
                var parser = new LabCalculatorParser(tokens);

                var tree = parser.compileUnit();

                var visitor = new LabCalculatorVisitor(cellExpressions);

                return visitor.Visit(tree);
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine(ex.Message);
                return double.NaN; 
            }
        }
    }
}
