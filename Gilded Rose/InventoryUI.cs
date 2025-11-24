using Gilded_Rose;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Gilded_Rose
{
    public static class InventoryUI
    {
        private const string ItemsFileName = "items.json";

        private static readonly IList<Item> _items = new List<Item>();

        // Default factory enforces Sulfuras=80. If you want to see "actual values" without forcing 80, pass false.
        private static UpdaterFactory _factory = new UpdaterFactory(enforceSulfurasQuality: true);
        private static GildedRose _gildedRose = new GildedRose(_items, _factory);

        // Static ctor: load items from file if present
        static InventoryUI()
        {
            LoadItemsFromFile();
        }

        public static void ShowItems()
        {
            Console.Clear();
            Console.WriteLine("Current Inventory (actual stored values) + Predicted next state:");
            if (_items.Count == 0)
            {
                Console.WriteLine(" - No items.");
                Pause();
                return;
            }

            PrintTableHeader();
            for (int i = 0; i < _items.Count; i++)
            {
                var it = _items[i];
                PrintTableRow(i, it);
            }

            Pause();
        }

        public static void UpdateOneDay()
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("No items to update. Load sample items or add items first.");
                Pause();
                return;
            }

            // Print before
            Console.WriteLine("Before update (actual + predicted next):");
            PrintTableHeader();
            for (int i = 0; i < _items.Count; i++)
                PrintTableRow(i, _items[i]);

            // Perform update (this mutates the real items)
            _gildedRose.UpdateQuality();

            // Persist changes after update
            SaveItemsToFile();

            // Print after
            Console.WriteLine();
            Console.WriteLine("After update (actual + predicted next):");
            PrintTableHeader();
            for (int i = 0; i < _items.Count; i++)
                PrintTableRow(i, _items[i]);

            Pause();
        }

        public static void AddItem()
        {
            Console.Clear();
            Console.WriteLine("Add new item:");
            Console.Write("Name: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                Pause();
                return;
            }

            int sellIn = ReadInt("SellIn (days): ");
            int quality = ReadInt("Quality (0..50) (Sulfuras will be set to 80 if enforcement enabled): ");

            if (quality > 50 && !name.StartsWith("Sulfuras", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Quality > 50 is not allowed (except Sulfuras). Clamping to 50.");
                quality = 50;
            }

            quality = Math.Max(0, Math.Min(quality, 1000)); // allow >50 for inspection if you plan to disable enforcement

            var newItem = new Item { Name = name, SellIn = sellIn, Quality = quality };
            _items.Add(newItem);

            // Save to file immediately
            try
            {
                SaveItemsToFile();
                Console.WriteLine("Item added and saved to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Item added but failed to save to file: {ex.Message}");
            }

            Pause();
        }

        public static void SimulateDays()
        {
            Console.Clear();
            int days = ReadInt("Simulate how many days? ");
            if (days <= 0)
            {
                Console.WriteLine("Enter a positive number.");
                Pause();
                return;
            }

            for (int d = 1; d <= days; d++)
            {
                Console.WriteLine($"--- Day {d} before update ---");
                PrintTableHeader();
                for (int i = 0; i < _items.Count; i++)
                    PrintTableRow(i, _items[i]);

                _gildedRose.UpdateQuality();

                // Persist after each simulated day
                SaveItemsToFile();

                Console.WriteLine($"--- Day {d} after update ---");
                PrintTableHeader();
                for (int i = 0; i < _items.Count; i++)
                    PrintTableRow(i, _items[i]);

                Console.WriteLine();
            }

            Pause();
        }

        public static void LoadSampleItems()
        {
            _items.Clear();
            _items.Add(new Item { Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20 });
            _items.Add(new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 });
            _items.Add(new Item { Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7 });
            _items.Add(new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 });
            _items.Add(new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80 });
            _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 15, Quality = 20 });
            _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 49 });
            _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 49 });
            _items.Add(new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 6 });

            // Save sample items to file (overwrite)
            try
            {
                SaveItemsToFile();
                Console.WriteLine("Sample items loaded and saved.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sample items loaded but failed to save: {ex.Message}");
            }

            Pause();
        }

        public static void ClearItems()
        {
            _items.Clear();
            try
            {
                SaveItemsToFile();
                Console.WriteLine("All items cleared and file updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleared items but failed to save file: {ex.Message}");
            }

            Pause();
        }

        public static void SetSulfurasEnforcement(bool enforce)
        {
            _factory = new UpdaterFactory(enforceSulfurasQuality: enforce);
            _gildedRose = new GildedRose(_items, _factory);
        }

        // ---- helpers ----
        private static void PrintTableHeader()
        {
            Console.WriteLine("{0,-4} {1,-40} {2,8} {3,8} {4,12} {5,12}", "Idx", "Name", "SellIn", "Quality", "NextSellIn", "NextQuality");
            Console.WriteLine(new string('-', 90));
        }

        private static void PrintTableRow(int idx, Item it)
        {
            var next = GetNextState(it);
            Console.WriteLine("{0,-4} {1,-40} {2,8} {3,8} {4,12} {5,12}",
                idx,
                Truncate(it.Name, 40),
                it.SellIn,
                it.Quality,
                next?.SellIn.ToString() ?? "-",
                next?.Quality.ToString() ?? "-");
        }

        
        private static Item GetNextState(Item item)
        {
            if (item == null) return null;

            var clone = new Item { Name = item.Name, SellIn = item.SellIn, Quality = item.Quality };

            var updater = _factory.GetUpdater(clone);
            if (updater == null) return clone;

            try
            {
                updater.Update(clone);
            }
            catch
            {
                // If any updater throws (shouldn't), return the unchanged clone
            }

            return clone;
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v))
                    return v;
                Console.WriteLine("Invalid integer, try again.");
            }
        }

        // ---------- JSON persistence ----------
        private static void SaveItemsToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_items, options);
                File.WriteAllText(ItemsFileName, json);
            }
            catch
            {
                throw;
            }
        }

        private static void LoadItemsFromFile()
        {
            try
            {
                if (!File.Exists(ItemsFileName)) return;

                string json = File.ReadAllText(ItemsFileName);
                if (string.IsNullOrWhiteSpace(json)) return;

                var itemsFromFile = JsonSerializer.Deserialize<List<Item>>(json);
                if (itemsFromFile == null) return;

                _items.Clear();
                foreach (var it in itemsFromFile)
                {
                    _items.Add(it);
                }
            }
            catch
            {
                // ignore load errors for now (could log). Keep app running with empty list.
            }
        }
    }

}