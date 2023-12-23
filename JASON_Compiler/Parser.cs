using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace tinyCompiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;

        public Node()
        {
        }

        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program_function = new Node("Program Function");
            while (InputPointer < TokenStream.Count() && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
            {
                program_function.Children.Add(Function_Statement());

            }
            program_function.Children.Add(Main_Function());
            return program_function;
        }
        bool checkValid(int idx,Token_Class tokenVal)
        {
            return idx<TokenStream.Count&&TokenStream[InputPointer].token_type==tokenVal;
        }
        Token_Class arithmaticOPS(int idx)
        {
            Token_Class tok = TokenStream[idx].token_type;
            switch (tok)
            {
                case Token_Class.PlusOp: return Token_Class.PlusOp;
                case Token_Class.MinusOp: return Token_Class.MinusOp;
                case Token_Class.MultiplyOp: return Token_Class.MultiplyOp;
                case Token_Class.DivideOp: return Token_Class.DivideOp;
                default: return Token_Class.nullReturn;
            }
        }
        Token_Class conditionalOP(int idx)
        {
            Token_Class tok = TokenStream[idx].token_type;
            switch (tok)
            {
                case Token_Class.LessThanOp: return Token_Class.LessThanOp;
                case Token_Class.GreaterThanOp: return Token_Class.GreaterThanOp;
                case Token_Class.EqualOp: return Token_Class.EqualOp;
                default: return Token_Class.nullReturn;
            }
        }
        Token_Class dataType(int idx)
        {
            Token_Class tok = TokenStream[idx].token_type;
            switch (tok)
            {
                case Token_Class.Int: return Token_Class.Int;
                case Token_Class.Float: return Token_Class.Float;
                case Token_Class.String: return Token_Class.String;
                default: return Token_Class.nullReturn;
            }
        }
        Token_Class booleanOP(int idx)
        {
            Token_Class tok = TokenStream[idx].token_type;
            switch (tok)
            {
                case Token_Class.andOP: return Token_Class.andOP;
                case Token_Class.orOP: return Token_Class.orOP;
                default: return Token_Class.nullReturn;
            }
        }
        Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count())
            {
                if (Token_Class.Idenifier == TokenStream[InputPointer].token_type && Token_Class.AssignOp == TokenStream[InputPointer + 1].token_type)
                {
                    statement.Children.Add(assignStatement());
                }
                else if (dataType(InputPointer) != Token_Class.nullReturn)
                    statement.Children.Add(declarationStatement());
                else if (Token_Class.read == TokenStream[InputPointer].token_type)
                    statement.Children.Add(read());
                else if (Token_Class.write == TokenStream[InputPointer].token_type)
                    statement.Children.Add(write());
                else if (Token_Class.If == TokenStream[InputPointer].token_type)
                    statement.Children.Add(ifStatement());
                else if (Token_Class.repeat == TokenStream[InputPointer].token_type)
                    statement.Children.Add(Repeat_Statement());
                else if (Token_Class.Idenifier == TokenStream[InputPointer].token_type && Token_Class.LParanthesis == TokenStream[InputPointer + 1].token_type)
                {
                    statement.Children.Add(functionCall());
                }
                else if (Token_Class.Return == TokenStream[InputPointer].token_type)
                    statement.Children.Add(returnStatement());
                else InputPointer++;
            }
            return statement;
        }
        Node functionCall()
        {
            Node function_call = new Node("function call");
            function_call.Children.Add(match(Token_Class.Idenifier));
            function_call.Children.Add(match(Token_Class.LParanthesis));
            if (checkValid(InputPointer,Token_Class.Idenifier))
            {
                function_call.Children.Add(ArgumentList());
            }
            function_call.Children.Add(match(Token_Class.RParanthesis));
            return function_call;
        }

         Node ArgumentList()
        {
            Node argument_list = new Node("argument list");
            argument_list.Children.Add(match(Token_Class.Idenifier));
            while(checkValid(InputPointer,Token_Class.Comma))
            {
                argument_list.Children.Add(match(Token_Class.Comma));
                argument_list.Children.Add(match(Token_Class.Idenifier));

            }
            return argument_list;
        }
        Node term()
        {
            Node term = new Node("term");
            if (checkValid(InputPointer, Token_Class.Number))
            {
                term.Children.Add(match(Token_Class.Number));
            }
            else if (checkValid(InputPointer, Token_Class.Idenifier))
            {
                if(checkValid(InputPointer+1, Token_Class.LParanthesis))
                {
                    term.Children.Add(functionCall());
                }
                else
                {
                    term.Children.Add(match(Token_Class.Idenifier));
                }
            }
            return term;
        }
        Node equation()
        {
            Node equation = new Node("equation");
            Node testNode = term();
            if (testNode.Children.Count>0)
            {
               equation.Children.Add(testNode); 
            }
            else if (checkValid(InputPointer,Token_Class.LParanthesis))
            {
                equation.Children.Add(match(Token_Class.LParanthesis));
                equation.Children.Add(match(Token_Class.Idenifier));
                while (checkValid(InputPointer,arithmaticOPS(InputPointer)))
                {
                    equation.Children.Add(match(arithmaticOPS(InputPointer)));
                    equation.Children.Add(match(Token_Class.Idenifier));
                }
                equation.Children.Add(match(Token_Class.RParanthesis));
            }
            return equation;
        }
        Node expression()
        {
            Node expression = new Node("expression");
            if(checkValid(InputPointer,Token_Class.StringVal))
            {
                expression.Children.Add(match(Token_Class.StringVal));
            }
            else
            {
                Node testNode = term();
                if(testNode.Children.Count==0)
                {
                    testNode=equation();
                }
                if (testNode.Children.Count > 0)
                {
                    expression.Children.Add(testNode);
                }
            }
           
            
            return expression;
        }
        Node assignStatement()
        {
            Node assignStatement = new Node("assign statement");

            assignStatement.Children.Add(match(Token_Class.Idenifier));
            assignStatement.Children.Add(match(Token_Class.AssignOp));
            assignStatement.Children.Add(expression());
            return assignStatement;
        }
        Node declarationStatement()
        {
            Node declarationStatement = new Node("declaration statement");
            declarationStatement.Children.Add(match(dataType(InputPointer)));
            declarationStatement.Children.Add(identifierList());
            declarationStatement.Children.Add(match(Token_Class.Semicolon));
            return declarationStatement;
        }

         Node identifierList()
        {
            Node identifierList = new Node("identifier list");
            identifierList.Children.Add(match(Token_Class.Idenifier));
            if (checkValid(InputPointer, Token_Class.AssignOp))
            {
                identifierList.Children.Add(assignList());
                while(checkValid(InputPointer,Token_Class.Comma))
                {
                    identifierList.Children.Add(match(Token_Class.Comma));
                    identifierList.Children.Add(match(Token_Class.Idenifier));
                    if (checkValid(InputPointer, Token_Class.EqualOp))
                    {
                        identifierList.Children.Add(assignList());
                    }
                }
            }
            return identifierList;
        }

         Node assignList()
        {
            Node assignList = new Node("assignment list");
            assignList.Children.Add(match(Token_Class.AssignOp));
            assignList.Children.Add(expression());
            return assignList;
        }
        Node write()
        {
            Node write = new Node("write");
            write.Children.Add(match(Token_Class.write));
            
            if (checkValid(InputPointer, Token_Class.endl))
            {
                write.Children.Add(match(Token_Class.endl));
            }
            else
            {

                write.Children.Add(expression());
            }
           
            write.Children.Add(match(Token_Class.Semicolon));

            return write;
        }
        Node read()
        {
            Node read = new Node("read");
            read.Children.Add(match(Token_Class.read));
            read.Children.Add(match(Token_Class.Idenifier));
            read.Children.Add(match(Token_Class.Semicolon));
            return read;
        }
        Node returnStatement()
        {
            Node returnStatement = new Node("return statement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));

            return returnStatement;
        }
        Node condition()
        {
            Node condition = new Node("condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(match(conditionalOP(InputPointer)));
            condition.Children.Add(term());
            return condition;
        }
        Node conditionStatement()
        {
            Node conditionStatement = new Node("condition statement");
            conditionStatement.Children.Add(condition());
            while(checkValid(InputPointer,booleanOP(InputPointer))) { 
            conditionStatement.Children.Add(match(booleanOP(InputPointer)));
                conditionStatement.Children.Add(condition());
            }
            return conditionStatement;
        }
        Node ifStatement()
        {
            Node ifStatement = new Node("if statement");
            ifStatement.Children.Add(match(Token_Class.If));
            ifStatement.Children.Add(conditionStatement());
            ifStatement.Children.Add(match(Token_Class.then));
            while (TokenStream[InputPointer].token_type != Token_Class.end && TokenStream[InputPointer].token_type != Token_Class.elseif && TokenStream[InputPointer].token_type != Token_Class.Else)
            {
                ifStatement.Children.Add(Statement());

            }
            if (InputPointer < TokenStream.Count())
            {
                if (Token_Class.elseif == TokenStream[InputPointer].token_type)
                {
                    ifStatement.Children.Add(Else_If_Statement());
                }
                else if (Token_Class.Else == TokenStream[InputPointer].token_type)
                {
                    ifStatement.Children.Add(Else_Statements());
                }
                else
                {
                    ifStatement.Children.Add(match(Token_Class.end));
                }

            }
                return ifStatement;
        }
        Node Else_If_Statement()
        {
            Node else_if_Statement = new Node("Else If Statement");
            else_if_Statement.Children.Add(match(Token_Class.elseif));
            else_if_Statement.Children.Add(conditionStatement());
            else_if_Statement.Children.Add(match(Token_Class.then));
            while (TokenStream[InputPointer].token_type != Token_Class.end || TokenStream[InputPointer].token_type != Token_Class.elseif || TokenStream[InputPointer].token_type != Token_Class.Else)
            {
                else_if_Statement.Children.Add(Statement());

            }
           
            if (InputPointer < TokenStream.Count())
            {
                if(Token_Class.elseif == TokenStream[InputPointer].token_type)
                {
                    else_if_Statement.Children.Add(Else_If_Statement());
                }
                else if(Token_Class.Else == TokenStream[InputPointer].token_type)
                {
                    else_if_Statement.Children.Add(Else_Statements());
                }
                else
                {
                    else_if_Statement.Children.Add(match(Token_Class.end)); 
                }
            }
            return else_if_Statement;
        }
        Node Else_Statements()
        {
            Node else_statement = new Node("Else Statement");
            else_statement.Children.Add(match(Token_Class.Else));
            while (TokenStream[InputPointer].token_type != Token_Class.end)
            {
                else_statement.Children.Add(Statement());

            }
            else_statement.Children.Add(match(Token_Class.end));   

            return else_statement;
        }
        Node Repeat_Statement()
        {
            Node repeat_statement = new Node("Repeat Statement");
            repeat_statement.Children.Add(match(Token_Class.repeat));
            while (TokenStream[InputPointer].token_type != Token_Class.until)
            {
                repeat_statement.Children.Add(Statement());

            }
            repeat_statement.Children.Add(match(Token_Class.until));
            repeat_statement.Children.Add(conditionStatement());
            return repeat_statement;


        }
        Node Parameter()
        {
            Node parameter = new Node("parameter");
            parameter.Children.Add(match(dataType(InputPointer)));
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }
        Node Parameter_list()
        {
            Node parameter_list = new Node("parameter list");
            parameter_list.Children.Add(Parameter());
            while (TokenStream[InputPointer].token_type == Token_Class.Comma && TokenStream[InputPointer + 1].token_type == dataType(InputPointer+1))
            {
                parameter_list.Children.Add(match(Token_Class.Comma));
                parameter_list.Children.Add(Parameter());
            }
            return parameter_list;
        }
        Node Function_Declaration()
        {
            Node function_declaration = new Node("function declaration");

            function_declaration.Children.Add(match(dataType(InputPointer)));
            function_declaration.Children.Add(match(Token_Class.Idenifier));
            function_declaration.Children.Add(match(Token_Class.LParanthesis));
            if (TokenStream[InputPointer].token_type != Token_Class.RParanthesis)
            {
                function_declaration.Children.Add(Parameter_list());

            }
            function_declaration.Children.Add(match(Token_Class.RParanthesis));

            

            return function_declaration;
            
        }
        Node Function_Body()
        {
            Node body = new Node("Function Body");
            body.Children.Add(match(Token_Class.Lcurly));
            while (TokenStream[InputPointer].token_type != Token_Class.Return)
            {
                body.Children.Add(Statement());
            }
            body.Children.Add(returnStatement());
            body.Children.Add(match(Token_Class.Rcurly));

            return body;
        }
        Node Function_Statement()
        {
            Node function_statement = new Node("Function Statement");

            function_statement.Children.Add(Function_Declaration());
            function_statement.Children.Add(Function_Body());
           
            return function_statement;
        }
        Node Main_Function()
        {
            Node main_function = new Node("Main Function");

            main_function.Children.Add(match(dataType(InputPointer)));
            main_function.Children.Add(match(Token_Class.Main));
            main_function.Children.Add(match(Token_Class.LParanthesis));
            main_function.Children.Add(match(Token_Class.RParanthesis));
            main_function.Children.Add(Function_Body());

            return main_function;
        }
        
        public Node match(Token_Class ExpectedToken)
        {


            if (InputPointer < TokenStream.Count())
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
