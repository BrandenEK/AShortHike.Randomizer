
namespace AShortHike.Randomizer.Extensions
{
    public static class GameDataExtensions
    {
        public static bool HasFishingRod(this GlobalData.GameData data)
        {
            return data.GetCollected(CollectableItem.Load("FishingRod")) > 0 || data.tags.GetString("HeldItem") == "FishingRod";
        }
    }
}
