using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
           // root.Children.Add(Program());
            return root;
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

            return ifStatement;
        }
        public Node match(Token_Class ExpectedToken)
        {


            if (InputPointer < TokenStream.Count)
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
