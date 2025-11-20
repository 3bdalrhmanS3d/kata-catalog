using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gilded_Rose
{
    public interface IItemUpdater
    {
        bool CanHandle(Item item);

        void Update(Item item);

    }
}
