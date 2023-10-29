grammar LabCalculator;

/*
 * Parser Rules
 */

compileUnit : expression EOF;

expression
    :   '(' expression ')'                                           # ParenthesizedExpr
    |   expression '^' expression                                    # ExponentialExpr
    |   expression operatorToken=(MULTIPLY | DIVIDE) expression      # MultiplicativeExpr
    |   expression operatorToken=(ADD | SUBTRACT) expression         # AdditiveExpr
    |   expression operatorToken=(MOD | DIV) expression              # ModDivExpr
    |   expression operatorToken=(LT | LEQ | GT | GEQ | EQ | NEQ) expression # RelationalExpr
    |   'not' expression                                             # NotExpr
    |   expression operatorToken=(OR | AND) expression               # LogicalExpr
    |   expression '^' expression                                    # PowerExpr
    |   'inc' '(' expression ')'                                     # IncrementExpr
    |   'dec' '(' expression ')'                                     # DecrementExpr
    |   'max' '(' expression ',' expression ')'                      # MaxExpr
    |   'min' '(' expression ',' expression ')'                      # MinExpr
    |   'mmax' '(' expression (',' expression)+ ')'                 # MultiMaxExpr
    |   'mmin' '(' expression (',' expression)+ ')'                 # MultiMinExpr
    |   'eqv' '(' expression ',' expression ')'                      # EqvExpr
    |   '+' expression                                               # UnaryPlusExpr
    |   '-' expression                                               # UnaryMinusExpr
    |   NUMBER                                                      # NumberExpr
    |   IDENTIFIER                                                  # IdentifierExpr
    |   CELL_REFERENCE                                              # CellReferenceExpr
    ;

/*
 * Lexer Rules
 */

NUMBER : INT ('.' INT)?; 
IDENTIFIER : [a-zA-Z]+[1-9][0-9]+;
CELL_REFERENCE : [A-Z]+ [0-9]+;

INT : ('0'..'9')+;

MULTIPLY : '*';
DIVIDE : '/';
SUBTRACT : '-';
ADD : '+';
LT : '<';
LEQ : '<=';
GT : '>';
GEQ : '>=';
EQ : '=';
NEQ : '<>';
MOD : 'mod';
DIV : 'div';
OR : 'or';
AND : 'and';

WS : [ \t\r\n] -> channel(HIDDEN);