
namespace AShortHike.Randomizer.Items
{
    internal static class LocationIdExtensions
    {
        public static string GetLocationId(this CollectOnInteract interact) =>
            $"{interact.transform.parent?.name}.{interact.transform.GetSiblingIndex()}";

        public static string GetLocationId(this CollectOnTouch touch) =>
            $"{touch.transform.parent?.name}.{touch.transform.GetSiblingIndex()}";

        public static string GetLocationId(this Holdable holdable) =>
            $"{holdable.transform.parent?.name}.{holdable.transform.GetSiblingIndex()}";

        public static string GetLocationId(this Chest chest) =>
            $"{chest.transform.parent?.name}.{chest.transform.GetSiblingIndex()}";

        public static string GetLocationId(this BuriedChest buriedChest) =>
            $"{buriedChest.transform.parent?.name}.{buriedChest.transform.GetSiblingIndex()}";

        public static string GetLocationId(this IConversation conversation, string item)
        {
            string speaker = conversation.originalSpeaker.name;

            switch (speaker)
            {
                case "CampRangerNPC": // Visitor's center shop salesman
                    {
                        int visitorFeathers = (int)Singleton<GlobalData>.instance.gameData.tags.GetFloat("$FeathersSold");
                        if (item == "GoldenFeather")
                            return speaker + $"[{visitorFeathers - 1}]";
                        else if (item == "ParkHat")
                            return speaker + "[9]";
                        break;
                    }
                case "ToughBirdNPC (1)": // Tough bird salesman
                    {
                        int toughBirdFeathers = (int)Singleton<GlobalData>.instance.gameData.tags.GetFloat("$ToughBirdSales");
                        if (item == "GoldenFeather")
                            return speaker + $"[{toughBirdFeathers - 1}]";
                        else if (item == "Watch")
                            return speaker + "[9]";
                        break;
                    }
                case "VolleyballOpponent": // Beachstickball
                    {
                        if (item == "GoldenFeather")
                            return speaker + "[0]";
                        else if (item == "Coin")
                            return speaker + "[1]";
                        else if (item == "KidHat")
                            return speaker + "[2]";
                        break;
                    }
                case "FishBuyer": // Fisherman
                    {
                        if (item == "FishEncyclopedia")
                            return speaker + "[0]";
                        else if (item == "GoldenFishingRod")
                            return speaker + "[1]";
                        break;
                    }
                case "CamperNPC": // Camper bribe
                    {
                        bool gotBribe = Singleton<GlobalData>.instance.gameData.tags.GetBool("Opened_CamperNPC[0]");
                        return speaker + (gotBribe ? "[1]" : "[0]");
                    }
                case "Bunny_WalkingNPC (1)": // Racing bunny
                    {
                        if (item == "RunningShoes")
                            return speaker + "[0]";
                        break;
                    }
                case "RaceOpponent": // Parkour racer
                    {
                        string race = Singleton<GlobalData>.instance.gameData.tags.GetString("RaceId");
                        int raceLevel = race == "MountainTopRace" ? 2 : (race == "OldBuildingRace" ? 1 : 0);
                        if (item == "Medal")
                            return speaker + $"[{raceLevel}]";
                        else if (item == "WalkieTalkie")
                            return speaker + "[9]";
                        break;
                    }
                case "LittleKidNPCVariant (1)": // Shell kid
                    {
                        if (item == "ShellNecklace")
                            return speaker + "[0]";
                        else if (item == "Shell")
                            return speaker + "[1]";
                        break;
                    }
                case "DadDeer": // Boat rental guy
                    {
                        if (item == "BoatKey")
                            return speaker + "[0]";
                        else if (item == "BoatManual")
                            return speaker + "[1]";
                        break;
                    }                    
            }

            return speaker + "[0]";
        }
    }
}
