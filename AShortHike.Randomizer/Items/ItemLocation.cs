
namespace AShortHike.Randomizer.Items
{
    public class ItemLocation
    {
        public readonly long ap_id;
        public readonly string item_name;
        public readonly string player_name;
        public readonly int type;
        public readonly int chest_angle;

        public bool ShouldBeGolden => type == 1 || type == 4;

        public ItemLocation(long ap_id, string item_name, string player_name, int type, int chest_angle)
        {
            this.ap_id = ap_id;
            this.item_name = item_name;
            this.player_name = player_name;
            this.type = type;
            this.chest_angle = chest_angle;
        }
    }
}
