using System;

namespace Gilded_Rose
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("        GILDED ROSE INVENTORY");
                Console.WriteLine("======================================");
                Console.WriteLine("1. Show all items");
                Console.WriteLine("2. Update quality for one day");
                Console.WriteLine("3. Add a new item");
                Console.WriteLine("4. Simulate N days");
                Console.WriteLine("5. Load sample items");
                Console.WriteLine("6. Clear items");
                Console.WriteLine("0. Exit");
                Console.WriteLine("--------------------------------------");
                Console.Write("Choose an option: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        InventoryUI.ShowItems();
                        break;

                    case "2":
                        InventoryUI.UpdateOneDay();
                        break;

                    case "3":
                        InventoryUI.AddItem();
                        break;

                    case "4":
                        InventoryUI.SimulateDays();
                        break;

                    case "5":
                        InventoryUI.LoadSampleItems();
                        break;

                    case "6":
                        InventoryUI.ClearItems();
                        break;

                    case "0":
                        exit = true;
                        Console.WriteLine("\nExiting program...");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Press Enter and try again...");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
