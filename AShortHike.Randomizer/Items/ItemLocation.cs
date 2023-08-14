
namespace AShortHike.Randomizer.Items
{
    public class ItemLocation
    {
        public readonly long ap_id;
        public readonly string item_name;
        public readonly string player_name;
        public readonly int type;

        public bool ShouldBeGolden => (type & 0x01) > 0 || (type & 0x04) > 0;

        public ItemLocation(long ap_id, string item_name, string player_name, int type)
        {
            this.ap_id = ap_id;
            this.item_name = item_name;
            this.player_name = player_name;
            this.type = type;
        }
    }
}
