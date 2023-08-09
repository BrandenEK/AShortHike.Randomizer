using System.Collections.Generic;

namespace AShortHike.Randomizer.Items
{
    public interface IItemHandler
    {
        public void StoreShuffledItems(Dictionary<string, string> items);

        public void ResetShuffledItems();

        public string GetItemAtLocation(string locationId);
    }
}
