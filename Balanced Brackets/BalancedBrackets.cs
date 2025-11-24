using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balanced_Brackets
{
    public class BalancedBrackets
    {
        public bool IsBalanced(string line)
        {

            if (string.IsNullOrEmpty(line))
                return false;

            Stack<char> stack = new Stack<char>();

            foreach (char c in line)
            {
                if (c == '[')
                {
                    stack.Push(c);
                }
                else if (c == ']')
                {
                    if (stack.Count == 0)
                        return false;

                    stack.Pop();
                }

            }
            return stack.Count == 0;
        }
    }
}
