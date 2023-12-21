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
