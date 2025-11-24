namespace Balanced_Brackets
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            string line = Console.ReadLine()!;

            BalancedBrackets b = new BalancedBrackets();

            Console.WriteLine(  b.IsBalanced( line ) );
        }
    }
}
