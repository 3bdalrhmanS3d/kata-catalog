using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gilded_Rose
{
    public static class Helpers
    {
        public static void DecreaseSellIn(Item item) => item.SellIn--;

        public static void IncreaseQuality(Item item, int amount = 1)
        {
            item.Quality = Math.Min(50, item.Quality + amount);
        }

        public static void DecreaseQuality(Item item, int amount = 1)
        {
            item.Quality = Math.Max(0, item.Quality - amount);
        }
    }

}
