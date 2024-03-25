namespace AShortHike.Randomizer.Extensions
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

                        int id = item switch
                        {
                            "GoldenFeather" => visitorFeathers - 1,
                            "ParkHat" => 9,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "ToughBirdNPC (1)": // Tough bird salesman
                    {
                        int toughBirdFeathers = (int)Singleton<GlobalData>.instance.gameData.tags.GetFloat("$ToughBirdSales");

                        int id = item switch
                        {
                            "GoldenFeather" => toughBirdFeathers - 1,
                            "Watch" => 9,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "VolleyballOpponent": // Beachstickball
                    {
                        int id = item switch
                        {
                            "GoldenFeather" => 0,
                            "Coin" => 1,
                            "KidHat" => 2,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "FishBuyer": // Fisherman
                    {
                        int id = item switch
                        {
                            "FishEncyclopedia" => 0,
                            "GoldenFishingRod" => 1,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "CamperNPC": // Camper bribe
                    {
                        bool gotBribe = Singleton<GlobalData>.instance.gameData.tags.GetBool("Opened_CamperNPC[0]");
                        return speaker + (gotBribe ? "[1]" : "[0]");
                    }
                case "Bunny_WalkingNPC (1)": // Racing bunny
                    {
                        int id = item switch
                        {
                            "RunningShoes" => 0,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "RaceOpponent": // Parkour racer
                    {
                        string race = Singleton<GlobalData>.instance.gameData.tags.GetString("RaceId");
                        int raceLevel = race == "MountainTopRace" ? 2 : race == "OldBuildingRace" ? 1 : 0;

                        int id = item switch
                        {
                            "Medal" => raceLevel,
                            "WalkieTalkie" => 9,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "LittleKidNPCVariant (1)": // Shell kid
                    {
                        int id = item switch
                        {
                            "ShellNecklace" => 0,
                            "Shell" => 1,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "DadDeer": // Boat rental guy
                    {
                        int id = item switch
                        {
                            "BoatKey" => 0,
                            "BoatManual" => 1,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
                case "Player": // Talking to yourself
                    {
                        int id = item switch
                        {
                            "CampingPermit" => 0,
                            _ => -1
                        };
                        return $"{speaker}[{id}]";
                    }
            }

            return speaker + "[0]";
        }
    }
}
