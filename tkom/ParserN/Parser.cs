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
            List<FunctionDeclaration> Functions = new List<FunctionDeclaration>();

            lexer.nextToken();

            var function = ParseFunction();
            while (function != null)
            {
                Functions.Add(function);
                function = ParseFunction();
            }
            if (lexer.Token.Type != TokenType.EOT)
                throw new ParserException(" Error with construction of the program. Excepted EOT after all function declarations."
                , lexer.Token);
            return new ProgramStructure(Functions);
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
                            throw new ParserException("ParseFunction(). Error in InstructionBlock. ", lexer.Token);
                        }
                        throw new ParserException("ParseFunction(). Error - missing brace right. ", lexer.Token);
                    }
                    throw new ParserException("ParseFunction(). Error - missing brace left.", lexer.Token);
                }
                throw new ParserException("ParseFunction(). Error - missing identifier (function name).", lexer.Token);
            }
            return null;
        }

        /************************************ ARGUMENTS ********************************************/

        private List<VarDeclaration> tryToParseArguments()
        {

            var arguement = tryToParseVarDeclaration();
            List<VarDeclaration> argList = new List<VarDeclaration>();
            while (arguement != null)
            {
                argList.Add(arguement);
                if (this.lexer.Token.Type != TokenType.Comma) break;
                this.lexer.nextToken();
                arguement = tryToParseVarDeclaration();
            }
            return argList;
        }

        private VarDeclaration tryToParseVarDeclaration()
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
                        default: throw new ParserException("tryToParseArgument(). Error - invalid type.", lexer.Token);
                    }
                }
                throw new ParserException("tryToParseArgument(). Error - missing identifier name. ", lexer.Token);

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
                    if (this.lexer.Token.Type != TokenType.Semicolon)
                        throw new ParserException("tryToParseInstructionBlock(). Error - missing Semicolon. ", lexer.Token);
                    this.lexer.nextToken();
                    instruction = tryToParseInstruction();
                }
                if (this.lexer.Token.Type == TokenType.ParenthesesRight)
                {
                    this.lexer.nextToken();
                    return instrList;
                }
                throw new ParserException("tryToParseInstructionBlock(). Error - missing Parentheses Right. ", lexer.Token);

            }
            throw new ParserException("tryToParseInstructionBlock(). Error - missing Parentheses Left. ", lexer.Token);
        }

        /************************************ INSTRUCTION ********************************************/

        private IInstruction tryToParseInstruction()
        {
            // instruction =  var_declaration | fun_call | init | object_call | control_statement;
            IInstruction instructionArg = tryToParseVarDeclaration();
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
                                if (lexer.Token.Type == TokenType.Semicolon)
                                {
                                    return new Statement(type, cond, block);
                                }
                                else throw new ParserException("tryToParseStatement(). Error - missing Semicolon after function declaration.", lexer.Token);
                            }
                            throw new ParserException("tryToParseStatement(). Error - missing block of instructions in statement. ", lexer.Token);

                        }
                        throw new ParserException("tryToParseStatement(). Error - missing BraceRight. ", lexer.Token);
                    }
                    throw new ParserException("tryToParseStatement(). Error - missing condition in statement.", lexer.Token);
                }
                throw new ParserException("tryToParseStatement(). Error - missing Brace Left.", lexer.Token);
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
            Stack<Value> SignStack = new Stack<Value>();
            Queue<IConditionQueueType> ConditionONPQueue = new Queue<IConditionQueueType>();
            if(f==null) return null;
            while (f != null)
            {
                ConditionONPQueue.Enqueue(f);

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
                    while (SignStack.Count > 0)
                    {
                        string sign = lexer.Token.Value;
                        if (priority(sign) > priority(SignStack.Peek().Identifier)) break;
                        //dodanie do kolejki i usunięcie ze stosu znaku o wyższym lub równym priorytecie
                        ConditionONPQueue.Enqueue(SignStack.Pop());
                    }
                    SignStack.Push(new Value(lexer.Token.Value));
                    lexer.nextToken();
                }
                else
                {
                    //skończyło się parsowanie condition - dodanie pozostałych znaków ze stosu do kolejki
                    while (SignStack.Count > 0)
                    {
                        ConditionONPQueue.Enqueue(SignStack.Pop());
                    }
                    return new Condition(ConditionONPQueue);
                }

                f = tryToParseConditionSingle();

            }
            throw new ParserException("tryToParseCondition(). The condition is not correct. ", lexer.Token);
        }

        public ConditionSingle tryToParseConditionSingle()
        {
            //["!"], ( "(", condition_statement, ")" | expression ) ;
            bool isNegation = false;
            IConditionType expression;
            if (lexer.Token.Type == TokenType.NegationOperator)
            {
                isNegation = true;
                lexer.nextToken();
            }
            if (lexer.Token.Type == TokenType.BraceLeft)
            {
                lexer.nextToken();
                Condition condNew = new Condition();
                condNew = tryToParseCondition();
                if (condNew != null)
                {
                    if (lexer.Token.Type == TokenType.BraceRight)
                    {
                        expression = condNew;
                        lexer.nextToken();
                        return new ConditionSingle(isNegation, expression);
                    }
                    throw new ParserException("tryToParseConditionSingle(). Error - missing BraceRight.", lexer.Token);
                }
                throw new ParserException("tryToParseConditionSingle(). Error - missing condition after BraceLeft.", lexer.Token);
            }
            Expression exp = tryToParseExpression();
            if (exp != null)
            {
                expression = exp;
                return new ConditionSingle(isNegation, expression);
            }
            if (exp == null && isNegation != true)
                return null;
            throw new ParserException("tryToParseConditionSingle(). Error -single negation with no expression. ", lexer.Token);

        }

        /************************************ EXPRESSION ********************************************/

        private Expression tryToParseExpression()
        { 
            ExpressionSingle f = tryToParseExpressionSingle();
            Stack<Value> SignStack = new Stack<Value>();
            Queue<IExpressionQueueType> ExpressionONPQueue = new Queue<IExpressionQueueType>();
            if(f == null) return null;
            while (f != null)
            {
                //dodanie expressions
                ExpressionONPQueue.Enqueue(f);

                if (lexer.Token.Type == TokenType.MinusOperator ||
                lexer.Token.Type == TokenType.PlusOperator ||
                lexer.Token.Type == TokenType.AsteriskOperator ||
                lexer.Token.Type == TokenType.SlashOperator)
                { 
                    //dodanie znaku
                    while (SignStack.Count > 0)
                    {
                        string sign = lexer.Token.Value;
                        if (priority(sign) > priority(SignStack.Peek().Identifier)) break;
                        //dodanie do kolejki i usunięcie ze stosu znaku o wyższym lub równym priorytecie
                        ExpressionONPQueue.Enqueue(SignStack.Pop());
                    }
                    SignStack.Push(new Value(lexer.Token.Value));
                    lexer.nextToken();
                }
                else
                {  
                    //skończyło się parsowanie expression - dodanie pozostałych znaków ze stosu do kolejki
                    while (SignStack.Count > 0)
                    {
                        ExpressionONPQueue.Enqueue(SignStack.Pop());
                    }
                    return new Expression(ExpressionONPQueue);
                }
                f = tryToParseExpressionSingle();
            }
            throw new ParserException("tryToParseExpression(). Error - the expression is not correct. ", lexer.Token);
        }

        private ExpressionSingle tryToParseExpressionSingle()
        {
            //F =  ["-"] ( integer | fun_call | color | name | "(", expression , ")" ) ;
            bool isMinus = false;
            IExpressionType expressionType;
            if (lexer.Token.Type == TokenType.MinusOperator)
            {
                isMinus = true;
                lexer.nextToken();
            }
            if (lexer.Token.Type == TokenType.Integer)
            {
                expressionType = new IntegerValue(Int32.Parse(lexer.Token.Value));
                lexer.nextToken();
                return new ExpressionSingle(isMinus, expressionType);
            }
            if (lexer.Token.Type == TokenType.Identifier)
            {
                string id = lexer.Token.Value;
                lexer.nextToken();
                FunctionCall FunCall = tryToParseFunctionCall(id);
                if (FunCall != null) { expressionType = FunCall; return new ExpressionSingle(isMinus, expressionType); }
                else
                {

                    expressionType = new Value(id);
                    return new ExpressionSingle(isMinus, expressionType);
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
                        expressionType = exp;
                        lexer.nextToken();
                        return new ExpressionSingle(isMinus, expressionType);
                    }
                    throw new ParserException("tryToParseExpressionSingle(). Error - missing BraceRight.", lexer.Token);
                }
            }
            if (isMinus == false)
                return null;
            throw new ParserException("tryToParseExpressionSingle(). Error - missing expression (single minus).", lexer.Token);
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
                        if (lexer.Token.Type != TokenType.Semicolon) throw new ParserException("tryToParseObjectCall(string id). Error - missing Semicolon.", lexer.Token); ;
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
                    throw new ParserException("tryToParseObjectCall(string id). Error - missing Parentheses Right.", lexer.Token);

                }
                else
                {
                    if (lexer.Token.Type == TokenType.ParenthesesRight)
                    {
                        lexer.nextToken();
                        //zwraca ObjectCall z pustą listą funkcji
                        return new ObjectCall(id, listFun);
                    }
                    throw new ParserException("tryToParseObjectCall(string id). Error - missing Parentheses Right. ", lexer.Token);
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
                throw new ParserException("tryToParseInitValue(string id). Error - missing expression in line: ", lexer.Token);
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
                throw new ParserException("tryToParseFunctionCall(string id). Error - missing BraceRight in line: ", lexer.Token);
            }
            return null;
        }

        private List<IExpressionType> tryToParseArgumentsFunctionCall()
        {
            List<IExpressionType> argList = new List<IExpressionType>();

            IExpressionType newVar = tryToParseExpression();
            if (newVar != null)
            {
                argList.Add(newVar);
                while (lexer.Token.Type == TokenType.Comma)
                {
                    lexer.nextToken();
                    newVar = tryToParseExpression();
                    if (newVar == null)
                        throw new ParserException("tryToParseArgumentsFunctionCall. Error - missing expression after comma.", lexer.Token);
                    argList.Add(newVar);
                }
                return argList;
            }
            return null;
        }

    }

   public class ParserException : Exception
    {
        public ParserException() { }
        public ParserException(string message) : base(message) { }
        public ParserException(string message, Exception inner) : base(message, inner) { }
        public ParserException(string message, Token Token) : base("Exception: " + message + " in line: " + Token.position.Line + " in column: " + Token.position.Column) { }
    }
}
