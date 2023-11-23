using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
/*    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant*/
    Main,Int,Float,String,If,elseif,read,write,repeat,until,Else,then,Return,endl,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,AssignOp,
    Idenifier, Number, StringVal,Lcurly,Rcurly,andOP,orOP
}
namespace tinyCompiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> DataTypes = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("Main", Token_Class.Main);
            ReservedWords.Add("THEN", Token_Class.then);
            ReservedWords.Add("UNTIL", Token_Class.until);
            ReservedWords.Add("WRITE", Token_Class.write);
            ReservedWords.Add("elseif", Token_Class.elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("read", Token_Class.read);
            ReservedWords.Add("repeat", Token_Class.repeat);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.endl);


            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("!", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("–", Token_Class.MinusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add(":=", Token_Class.AssignOp);
            Operators.Add("}", Token_Class.Rcurly);
            Operators.Add("{", Token_Class.Lcurly);
            Operators.Add("&&", Token_Class.andOP);
            Operators.Add("||", Token_Class.orOP);
            DataTypes.Add("int",Token_Class.Int);
            DataTypes.Add("float",Token_Class.Float);
            DataTypes.Add("string",Token_Class.String);

        }

    public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();


                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                if (CurrentChar == '/' && SourceCode[j + 1] == '*')
                {
                    while (true)
                    {
                        j++;
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar;
                        if (CurrentChar == '*' && SourceCode[j + 1] == '/')
                        {
                            CurrentLexeme += CurrentChar;
                            i = j + 1;
                            break;
                        }
                        else if (j == SourceCode.Length - 1)
                        {
                            FindTokenClass(CurrentLexeme);

                            i = j;

                        }
                    }
                    //FindTokenClass(CurrentLexeme);

                }
                else if (CurrentChar == ':' && SourceCode[j + 1]=='=')
                {
                    i=j + 2;
                    FindTokenClass(":=");

                }
                else if (CurrentChar == ':' ||CurrentChar=='&'|| CurrentChar == '|')
                {
                    
                    switch (SourceCode[j+1]) 
                    {
                        case '=': FindTokenClass(":="); break;
                        case '&': FindTokenClass("&&"); break;
                        case '|': FindTokenClass("||"); break;
                        
                    }

                    i = j + 2;
                }

                else if ((CurrentChar >= 'A' && CurrentChar <= 'z')) //if you read a character
                {
                    j++;
                    CurrentChar= SourceCode[j];
                    while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar))
                    {

                        CurrentLexeme+= CurrentChar;
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j++;
                    CurrentChar = SourceCode[j];
                    while (char.IsDigit(CurrentChar)||CurrentChar=='.')
                    {
                        CurrentLexeme += CurrentChar;
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;


                }
                else if (CurrentChar == '\"')
                {
                    
                    j++;
                    CurrentChar = SourceCode[j];
                    CurrentLexeme += CurrentChar;
                    while (CurrentChar != '\"')
                    {
                        j++;
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i =j;
                    
                }
                else
                {
                    FindTokenClass(CurrentLexeme);

                    i = j;
                }
                
            }
            
            tiny_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?

            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
               
            }
            else if (DataTypes.ContainsKey(Lex))
            {
                Tok.token_type = DataTypes[Lex];
            }
            //Is it an identifier?

           else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
            }
            //Is it a Constant?
           else if (isConstant(Lex)) {
                Tok.token_type = Token_Class.Number;
            }
            else if (isString(Lex))
            {
                Tok.token_type=Token_Class.StringVal;
            }
            //Is it an operator?
           else if (Operators.ContainsKey(Lex))
            {
                
                Tok.token_type= Operators[Lex];
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
                return;
            }
            Tokens.Add(Tok);
        }
        
    

        bool isIdentifier(string lex)
        {
            bool isValid=true;
            // Check if the lex is an identifier or not.
            string pattern = @"^[a-zA-Z][a-zA-Z0-9]*";
            Match match=Regex.Match(lex, pattern);
            if (!match.Success)
            {
                isValid = false;
            }

            return isValid;
        }


bool isString(string lex)
    {
        bool isValid = false; // Initialize isValid to false

        // Check if the lex is a constant (Number) or not.
        string pattern = "^\"[^\"]*\"$"; // Use a proper regex pattern to match a string in double quotes
        Match match = Regex.Match(lex, pattern);

        if (match.Success)
        {
            isValid = true; // Set isValid to true if the match is successful
        }

        return isValid;
    }

        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.
            string pattern = @"^\d*\.?\d+$";
            Match match = Regex.Match(lex, pattern);
            if (!match.Success)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
