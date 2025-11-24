using System;

namespace _100_Dors
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int numberOfDoors = Convert.ToInt32(Console.ReadLine());

            bool[] doors = new bool[numberOfDoors];

            RunDoorKata(doors);

            string result = GetDoorStateString(doors);

            Console.WriteLine(result);
        }

        static void RunDoorKata(bool[] doors)
        {
            int n = doors.Length;

            for (int pass = 1; pass <= n; pass++)
            {
                for (int door = pass - 1; door < n; door += pass)
                {
                    doors[door] = !doors[door]; // toggle
                }
            }
        }

        static string GetDoorStateString(bool[] doors)
        {
            char[] doorChars = new char[doors.Length];

            for (int i = 0; i < doors.Length; i++)
            {
                doorChars[i] = doors[i] ? '@' : '#';
            }

            return new string(doorChars);
        }
    }
}
