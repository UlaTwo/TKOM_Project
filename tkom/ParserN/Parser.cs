using System;
using System.Collections.Generic;
using tkom.LexerN;
using tkom.ParserN.Structures;

namespace tkom.ParserN
{
    public class Parser
    {
        private Lexer lexer;
        public Parser(Lexer new_lexer)
        {
            lexer = new_lexer;
        }

        public ProgramStructure ParseProgram()
        {
            ProgramStructure program = new ProgramStructure();

            lexer.nextToken();
            var classes = ParseClass();
            while (classes != null)
            {
                program.Classes.Add(classes);
                classes = ParseClass();
            }

            var function = ParseFunction();
            while (function != null)
            {
                program.Functions.Add(function);
                function = ParseFunction();
            }
            if (lexer.Token.Type != TokenType.EOT)
                throw new ParserException("Exception: Error with construction of the program. Chcek if your class declarations are before function declaration. Excepted EOT in "
                + lexer.source.position.Line);
            return program;
        }

        /************************************ FUNCTION DECLARATION ********************************************/

        private FunctionDeclaration ParseFunction()
        {
            if (this.lexer.Token.Type == TokenType.Def)
            {
                lexer.nextToken();

                bool isIntReturned = false;
                if (this.lexer.Token.Type == TokenType.IntegerId)
                {
                    this.lexer.nextToken();
                    isIntReturned = true;
                }
                if (this.lexer.Token.Type == TokenType.Identifier)
                {
                    var Identifier = this.lexer.Token.Value;
                    this.lexer.nextToken();
                    if (this.lexer.Token.Type == TokenType.BraceLeft)
                    {
                        this.lexer.nextToken();
                        var arguments = tryToParseArguments();
                        if (this.lexer.Token.Type == TokenType.BraceRight)
                        {
                            this.lexer.nextToken();
                            var block = tryToParseInstructionBlock();
                            if (block != null)
                            {
                                return new FunctionDeclaration(isIntReturned, Identifier, arguments, block);
                            }
                            throw new ParserException("Exception: Error in InstructionBlock in line: " + lexer.source.position.Line);

                        }
                        throw new ParserException("Exception: Error - missing brace right in line: " + lexer.source.position.Line);
                    }
                    throw new ParserException("Exception: Error - missing brace left in line: " + lexer.source.position.Line);


                }
                throw new ParserException("Exception: Error - missing identifier (function name) in line: " + lexer.source.position.Line);
            }
            return null;
        }

        /************************************ ARGUMENTS ********************************************/

        private List<VarDeclaration> tryToParseArguments()
        {

            var arguement = tryToParseArgument();
            List<VarDeclaration> argList = new List<VarDeclaration>();
            while (arguement != null)
            {
                argList.Add(arguement);
                if (this.lexer.Token.Type != TokenType.Comma) break;
                this.lexer.nextToken();
                arguement = tryToParseArgument();
            }
            return argList;
        }

        private VarDeclaration tryToParseArgument()
        {
            var type = this.lexer.Token.Type;
            if (type == TokenType.IntegerId || type == TokenType.Turtle || type == TokenType.StringId)
            {
                lexer.nextToken();
                if (this.lexer.Token.Type == TokenType.Identifier)
                {
                    string iden = this.lexer.Token.Value;
                    lexer.nextToken();
                    switch (type)
                    {
                        case TokenType.IntegerId: return new IntVarDeclaration(iden);
                        case TokenType.Turtle: return new TurtleVarDeclaration(iden);
                        case TokenType.StringId: return new StringVarDeclaration(iden);
                        default: return null;
                    }
                }
                throw new ParserException("Exception: Error - missing identifier name in line: " + lexer.source.position.Line);

            }
            if (type == TokenType.Class)
            {
                lexer.nextToken();
                if (this.lexer.Token.Type == TokenType.Identifier)
                {
                    var idClass = this.lexer.Token.Value;
                    lexer.nextToken();
                    if (this.lexer.Token.Type == TokenType.Identifier)
                    {
                        var id = this.lexer.Token.Value;
                        lexer.nextToken();
                        return new ClassVarDeclaration(id, idClass);

                    }
                    throw new ParserException("Exception: Error - missing identifier name in line: " + lexer.source.position.Line);

                }
                throw new ParserException("Exception: Error - missing class name in line: " + lexer.source.position.Line);
            }
            return null;
        }

        /************************************ INSTRUCTION BLOCK ********************************************/
        private List<IInstruction> tryToParseInstructionBlock()
        {
            if (this.lexer.Token.Type == TokenType.ParenthesesLeft)
            {
                lexer.nextToken();
                IInstruction instruction = tryToParseInstruction();
                List<IInstruction> instrList = new List<IInstruction>();
                while (instruction != null)
                {
                    instrList.Add(instruction);
                    if (this.lexer.Token.Type != TokenType.Semicolon) break;
                    this.lexer.nextToken();
                    instruction = tryToParseInstruction();
                }
                if (this.lexer.Token.Type == TokenType.ParenthesesRight)
                {
                    this.lexer.nextToken();
                    return instrList;
                }
                throw new ParserException("Exception: Error - missing Parentheses Right in line: " + lexer.source.position.Line);

            }
            throw new ParserException("Exception: Error - missing Parentheses Left in line: " + lexer.source.position.Line);
        }

        /************************************ INSTRUCTION ********************************************/

        private IInstruction tryToParseInstruction()
        {
            // instruction =  var_declaration | fun_call | init | object_call | control_statement;
            IInstruction instructionArg = tryToParseArgument();
            if (instructionArg != null)
            {
                return instructionArg;
            }

            if (lexer.Token.Type == TokenType.Identifier)
            {
                string id = lexer.Token.Value;
                lexer.nextToken();

                FunctionCall instructionFC = tryToParseFunctionCall(id);
                if (instructionFC != null)
                {
                    return instructionFC;
                }

                InitValue instructionInit = tryToParseInitValue(id);
                if (instructionInit != null)
                {
                    return instructionInit;
                }

                ObjectCall instructionObjCall = tryToParseObjectCall(id);
                if (instructionObjCall != null)
                {
                    return instructionObjCall;
                }

            }

            Statement instructionStatement = tryToParseStatement();
            if (instructionStatement != null)
            {
                return instructionStatement;
            }

            return null;
        }

        /************************************ STATEMENT ********************************************/

        private Statement tryToParseStatement()
        {
            if (lexer.Token.Type == TokenType.While || lexer.Token.Type == TokenType.If)
            {
                string type = lexer.Token.Value;
                lexer.nextToken();
                if (lexer.Token.Type == TokenType.BraceLeft)
                {
                    lexer.nextToken();
                    Condition cond = new Condition();
                    cond = tryToParseCondition();
                    if (cond != null)
                    {
                        if (lexer.Token.Type == TokenType.BraceRight)
                        {
                            lexer.nextToken();
                            var block = tryToParseInstructionBlock();
                            if (block != null)
                            {
                                if (lexer.Token.Type == TokenType.Semicolon) { lexer.nextToken(); return new Statement(type, cond, block); }
                                else throw new ParserException("Exception: Error - missing Semicolon after function declaration in line: " + lexer.source.position.Line);
                            }
                            throw new ParserException("Exception: Error - missing block of instructions in statement in line: " + lexer.source.position.Line);

                        }
                        throw new ParserException("Exception: Error - missing BraceRight in line: " + lexer.source.position.Line);
                    }
                    throw new ParserException("Exception: Error - missing condition in statement in line: " + lexer.source.position.Line);
                }
                throw new ParserException("Exception: Error - missing Brace Left in line: " + lexer.source.position.Line);
            }
            return null;
        }

        /************************************ CONDITION ********************************************/

        /*metoda pomocnicz do ustalenia priorytetu znaku */
        private int priority(string sign)
        {
            switch (sign)
            {
                //for expression parse
                case "+": return 1;
                case "-": return 1;
                case "*": return 2;
                case "/": return 2;
                //for condition parse
                case "^": return 2;
                case "~": return 2;
                case "!=": return 1;
                case "<": return 1;
                case "<=": return 1;
                case ">": return 1;
                case ">=": return 1;
                case "==": return 1;
            }
            return 0;
        }
        private Condition tryToParseCondition()
        {
            ConditionSingle f = tryToParseConditionSingle();
            Condition cond = new Condition();
            while (f != null)
            {
                cond.ConditionONPQueue.Enqueue(f);

                if (lexer.Token.Type == TokenType.AndOperator ||
                lexer.Token.Type == TokenType.OrOperator ||
                lexer.Token.Type == TokenType.InequalityOperator ||
                lexer.Token.Type == TokenType.LessOperator ||
                lexer.Token.Type == TokenType.GreaterOrEqualOperator ||
                lexer.Token.Type == TokenType.GreaterOperator ||
                lexer.Token.Type == TokenType.EqualityOperator ||
                lexer.Token.Type == TokenType.LessOrEqualOperator)
                {
                    //dodanie znaku
                    while (cond.SignStack.Count > 0)
                    {
                        string sign = lexer.Token.Value;
                        if (priority(sign) > priority(cond.SignStack.Peek().Identifier)) break;
                        //dodanie do kolejki i usunięcie ze stosu znaku o wyższym lub równym priorytecie
                        cond.ConditionONPQueue.Enqueue(cond.SignStack.Pop());
                    }
                    cond.SignStack.Push(new Value(lexer.Token.Value));
                    lexer.nextToken();
                }
                else
                {
                    //skończyło się parsowanie condition - dodanie pozostałych znaków ze stosu do kolejki
                    while (cond.SignStack.Count > 0)
                    {
                        cond.ConditionONPQueue.Enqueue(cond.SignStack.Pop());
                    }
                    return cond;
                }

                f = tryToParseConditionSingle();

            }
            return null;
        }

        public ConditionSingle tryToParseConditionSingle()
        {
            //["!"], ( "(", condition_statement, ")" | expression ) ;
            ConditionSingle CondF = new ConditionSingle();
            CondF.isNegation = false;
            if (lexer.Token.Type == TokenType.NegationOperator)
            {
                CondF.isNegation = true;
                lexer.nextToken();
            }
            if (lexer.Token.Type == TokenType.BraceLeft)
            {
                lexer.nextToken();
                Condition condNew = new Condition();
                condNew = tryToParseCondition();
                if (CondF != null)
                {
                    if (lexer.Token.Type == TokenType.BraceRight)
                    {
                        CondF.expression = condNew;
                        lexer.nextToken();
                        return CondF;
                    }
                    throw new ParserException("Exception: Error - missing BraceRight in line: " + lexer.source.position.Line);
                }
            }
            Expression exp = tryToParseExpression();
            if (exp != null)
            {
                CondF.expression = exp;
                return CondF;
            }
            if (exp == null && CondF.isNegation != true)
                return null;
            throw new ParserException("Exception: Error -single negation with no expression in line: " + lexer.source.position.Line);

        }

        /************************************ EXPRESSION ********************************************/

        private Expression tryToParseExpression()
        {
            ExpressionSingle f = new ExpressionSingle();
            f = tryToParseExpressionSingle();
            Expression expression = new Expression();
            while (f != null)
            {
                //dodanie expressions
                expression.ExpressionONPQueue.Enqueue(f);

                if (lexer.Token.Type == TokenType.MinusOperator ||
                lexer.Token.Type == TokenType.PlusOperator ||
                lexer.Token.Type == TokenType.AsteriskOperator ||
                lexer.Token.Type == TokenType.SlashOperator)
                {
                    //dodanie znaku
                    while (expression.SignStack.Count > 0)
                    {
                        string sign = lexer.Token.Value;
                        if (priority(sign) > priority(expression.SignStack.Peek().Identifier)) break;
                        //dodanie do kolejki i usunięcie ze stosu znaku o wyższym lub równym priorytecie
                        expression.ExpressionONPQueue.Enqueue(expression.SignStack.Pop());
                    }
                    expression.SignStack.Push(new Value(lexer.Token.Value));
                    lexer.nextToken();
                }
                else
                {   //skończyło się parsowanie expression - dodanie pozostałych znaków ze stosu do kolejki
                    while (expression.SignStack.Count > 0)
                    {
                        expression.ExpressionONPQueue.Enqueue(expression.SignStack.Pop());
                    }
                    return expression;
                }

                f = tryToParseExpressionSingle();

            }
            return null;
        }

        private ExpressionSingle tryToParseExpressionSingle()
        {
            ExpressionSingle ExpF = new ExpressionSingle();
            //F =  ["-"] ( integer | fun_call | color | name | "(", expression , ")" ) ;
            ExpF.isMinus = false;
            if (lexer.Token.Type == TokenType.MinusOperator)
            {
                ExpF.isMinus = true;
                lexer.nextToken();
            }
            if (lexer.Token.Type == TokenType.Integer)
            {
                ExpF.expressionType = new IntegerValue(Int32.Parse(lexer.Token.Value));
                lexer.nextToken();
                return ExpF;
            }
            if (lexer.Token.Type == TokenType.Identifier)
            {
                string id = lexer.Token.Value;
                lexer.nextToken();
                FunctionCall FunCall = tryToParseFunctionCall(id);
                if (FunCall != null) { ExpF.expressionType = FunCall; return ExpF; }
                else
                {

                    ExpF.expressionType = new Value(id);
                    return ExpF;
                }
            }
            if (lexer.Token.Type == TokenType.BraceLeft)
            {
                lexer.nextToken();
                Expression exp = tryToParseExpression();
                if (exp != null)
                {
                    if (lexer.Token.Type == TokenType.BraceRight)
                    {
                        ExpF.expressionType = exp;
                        lexer.nextToken();
                        return ExpF;
                    }
                    throw new ParserException("Exception: Error - missing BraceRight in line: " + lexer.source.position.Line);
                }
            }
            if (ExpF.isMinus == false)
                return null;
            throw new ParserException("Exception: Error - missing expression (single minus) in line: " + lexer.source.position.Line);
        }

        /************************************ OBJECT CALL ********************************************/

        private ObjectCall tryToParseObjectCall(string id)
        {
            //object_call = name, "{", {fun_call}  "}" ;
            if (lexer.Token.Type == TokenType.ParenthesesLeft)
            {
                lexer.nextToken();
                List<FunctionCall> listFun = new List<FunctionCall>();
                if (lexer.Token.Type == TokenType.Identifier)
                {
                    string ide = lexer.Token.Value;
                    lexer.nextToken();
                    FunctionCall f = tryToParseFunctionCall(ide);
                    while (f != null)
                    {
                        listFun.Add(f);
                        if (lexer.Token.Type != TokenType.Semicolon) break;
                        lexer.nextToken();
                        if (lexer.Token.Type == TokenType.Identifier)
                        {
                            ide = lexer.Token.Value;
                            lexer.nextToken();
                            f = tryToParseFunctionCall(ide);
                        }
                        else break;
                    }
                    if (lexer.Token.Type == TokenType.ParenthesesRight)
                    {
                        lexer.nextToken();
                        return new ObjectCall(id, listFun);
                    }
                    throw new ParserException("Exception: Error - missing Parentheses Right in line : " + lexer.source.position.Line);

                }
                else
                {
                    lexer.nextToken();
                    if (lexer.Token.Type == TokenType.ParenthesesRight)
                    {

                        lexer.nextToken();
                        return null;
                    }
                    throw new ParserException("Exception: Error - missing Parentheses Right in line: " + lexer.source.position.Line);
                }

            }
            return null;
        }

        /************************************ INIT VALUE ********************************************/

        private InitValue tryToParseInitValue(string id)
        {
            //init = name, "=", expression ";"  ;
            if (lexer.Token.Type == TokenType.Assignment)
            {
                lexer.nextToken();
                Expression newExpression = new Expression();
                newExpression = tryToParseExpression();
                if (newExpression != null)
                {
                    return new InitValue(id, newExpression);
                }
                throw new ParserException("Exception: Error - missing expression in line: " + lexer.source.position.Line);
            }
            return null;

        }

        /************************************ FUNCTION CALL ********************************************/

        private FunctionCall tryToParseFunctionCall(string id)
        {
            if (lexer.Token.Type == TokenType.BraceLeft)
            {
                lexer.nextToken();
                List<IExpressionType> arguments = tryToParseArgumentsFunctionCall();
                if (lexer.Token.Type == TokenType.BraceRight)
                {
                    lexer.nextToken();
                    return new FunctionCall(id, arguments);
                }
                throw new ParserException("Exception: Error - missing BraceRight in line: " + lexer.source.position.Line);
            }
            return null;
        }

        private List<IExpressionType> tryToParseArgumentsFunctionCall()
        {
            List<IExpressionType> argList = new List<IExpressionType>();
            while (lexer.Token.Type == TokenType.Identifier || lexer.Token.Type == TokenType.Integer || lexer.Token.Type == TokenType.MinusOperator || lexer.Token.Type == TokenType.BraceLeft)
            {
                string id = lexer.Token.Value;
                IExpressionType newVar = tryToParseExpression();
                argList.Add(newVar);

                if (lexer.Token.Type != TokenType.Comma) return argList;
                lexer.nextToken();
            }
            return null;
        }

        /************************************ CLASS DECLARATION ********************************************/

        /*class declaration parse methods*/
        //class_declaration = "class", name , "{", {class_instruction_block} , "}" ;
        //class_instruction_block = var_declaration | function_declaration ;
        private ClassDeclaration ParseClass()
        {
            if (this.lexer.Token.Type == TokenType.Class)
            {
                lexer.nextToken();
                if (this.lexer.Token.Type == TokenType.Identifier)
                {
                    var Identifier = this.lexer.Token.Value;
                    this.lexer.nextToken();
                    if (this.lexer.Token.Type == TokenType.ParenthesesLeft)
                    {
                        this.lexer.nextToken();
                        List<IClassInstruction> ClassInstructions = new List<IClassInstruction>();
                        VarDeclaration varDeclaration = tryToParseArgument();
                        FunctionDeclaration FunctionDeclaration = ParseFunction();
                        while (varDeclaration != null || FunctionDeclaration != null)
                        {
                            if (varDeclaration != null)
                            {
                                if (lexer.Token.Type == TokenType.Semicolon) { lexer.nextToken(); ClassInstructions.Add(varDeclaration); }
                                else throw new ParserException("Exception: Error - missing Semicolon after variable declaration in line: " + lexer.source.position.Line);
                            }
                            if (FunctionDeclaration != null)
                            {
                                if (lexer.Token.Type == TokenType.Semicolon) { lexer.nextToken(); ClassInstructions.Add(FunctionDeclaration); }
                                else throw new ParserException("Exception: Error - missing Semicolon after function declaration in line: " + lexer.source.position.Line);
                            }
                            varDeclaration = tryToParseArgument();
                            FunctionDeclaration = ParseFunction();

                        }

                        if (this.lexer.Token.Type == TokenType.ParenthesesRight)
                        {
                            this.lexer.nextToken();
                            return new ClassDeclaration(Identifier, ClassInstructions);
                        }
                        throw new ParserException("Exception: Error - missing ParenthesesRight in line: " + lexer.source.position.Line);
                    }
                    throw new ParserException("Exception: Error - missing Parentheses Left in line: " + lexer.source.position.Line);
                }
                throw new ParserException("Exception: Error - missing Identifier (class name) in line: " + lexer.source.position.Line);
            }
            return null;
        }
    }

    class ParserException : Exception
    {
        public ParserException() { }
        public ParserException(string message) : base(message) { }
        public ParserException(string message, Exception inner) : base(message, inner) { }
    }
}
