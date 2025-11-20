using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gilded_Rose
{
    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public int SellIn {  get; set; }
        public int Quality { get; set; }

    }

    public class NormalUpdater : IItemUpdater
    {
        public bool CanHandle(Item item) => item.Name == null ||
                                        (item.Name != "Aged Brie" &&
                                         item.Name != "Sulfuras, Hand of Ragnaros" &&
                                         !item.Name.StartsWith("Backstage") &&
                                         !item.Name.StartsWith("Conjured"));

        public void Update(Item item)
        {
            Helpers.DecreaseSellIn(item);

            int dec = item.SellIn < 0 ? 2 : 1;

            Helpers.DecreaseQuality(item, dec);
        }
    }

    public class AgedBrieUpdater : IItemUpdater
    {
        public bool CanHandle(Item item) => item.Name == "Aged Brie";

        public void Update(Item item)
        {
            Helpers.DecreaseSellIn(item);
            int increment = item.SellIn < 0 ? 2 : 1;
            Helpers.IncreaseQuality(item, increment);
        }
    }

    public class SulfurasUpdater : IItemUpdater
    {
        private readonly bool _enforceQuality80;

        public SulfurasUpdater(bool enforceQuality80 = true)
        {
            _enforceQuality80 = enforceQuality80;
        }

        public bool CanHandle(Item item) => item?.Name == "Sulfuras, Hand of Ragnaros";

        public void Update(Item item)
        {
            // Legendary: do not change SellIn or Quality by default.
            // Only enforce Quality=80 if the factory/requester wants to enforce the kata invariant.
            if (_enforceQuality80)
            {
                item.Quality = 80;
            }
        }
    }


    public class BackstageUpdater : IItemUpdater
    {
        public bool CanHandle(Item item) => item.Name != null && item.Name.StartsWith("Backstage");

        public void Update(Item item)
        {
            Helpers.DecreaseSellIn(item);

            if (item.SellIn < 0)
            {
                item.Quality = 0;
                return;
            }

            if (item.SellIn < 5)
                Helpers.IncreaseQuality(item, 3);
            else if (item.SellIn < 10)
                Helpers.IncreaseQuality(item, 2);
            else
                Helpers.IncreaseQuality(item, 1);
        }
    }

    public class ConjuredUpdater : IItemUpdater
    {
        public bool CanHandle(Item item) => item.Name != null && item.Name.StartsWith("Conjured");

        public void Update(Item item)
        {
            Helpers.DecreaseSellIn(item);

            int decrement = item.SellIn < 0 ? 4 : 2;
            Helpers.DecreaseQuality(item, decrement);
        }
    }

    public class UpdaterFactory
    {
        private readonly List<IItemUpdater> _updaters;

        // Add parameter to control Sulfuras enforcement (default true -> keep Quality=80)
        public UpdaterFactory(bool enforceSulfurasQuality = true, IEnumerable<IItemUpdater> updaters = null)
        {
            if (updaters != null)
            {
                _updaters = updaters.ToList();
            }
            else
            {
                _updaters = new IItemUpdater[] {
                new SulfurasUpdater(enforceSulfurasQuality),
                new AgedBrieUpdater(),
                new BackstageUpdater(),
                new ConjuredUpdater(),
                new NormalUpdater()
            }.ToList();
            }
        }

        public IItemUpdater GetUpdater(Item item)
        {
            var handler = _updaters.FirstOrDefault(u => u.CanHandle(item));
            return handler ?? new NormalUpdater();
        }
    }


    public class GildedRose
    {
        private readonly IList<Item> _items;
        private readonly UpdaterFactory _factory;

        public GildedRose(IList<Item> items, UpdaterFactory factory = null)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _factory = factory ?? new UpdaterFactory();
        }

        public IList<Item> Items => _items;

        public void UpdateQuality()
        {
            foreach (var item in _items)
            {
                var updater = _factory.GetUpdater(item);
                updater.Update(item);
            }
        }
    }

}
