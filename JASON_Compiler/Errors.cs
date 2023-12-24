using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tinyCompiler
{
    public static class Errors
    {
        private static bool addedErr = false;
        public static List<string> Error_List = new List<string>();
        private static Stack<char> brackets = new Stack<char>();

        // For Brackets Error Handling
        public static void bracketBalance(char bracket)
        {

            if (bracket == '(' || bracket == '{')
            {
                brackets.Push(bracket);
            }
            else
            {
                if (brackets.Count != 0)
                {
                    char head = brackets.Pop();
                    if ((head == '(' && bracket != ')')  || (head == '{' && bracket != '}'))
                    {
                        Error_List.Add("Error in Brackets Balance");
                        addedErr = true;
                        
                    }
                }
                else
                {
                    Error_List.Add("Error in Brackets Balance");
                    addedErr = true;
                    
                }
            }
            
        }
        public static void endOfCode()
        {
            if (!addedErr)
            {
                if (brackets.Count != 0)
                {
                    Error_List.Add("Error in Brackets Balance");
                }
            }
        }



    }
}
