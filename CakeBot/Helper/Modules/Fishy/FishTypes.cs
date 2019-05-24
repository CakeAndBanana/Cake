using System.Collections.Generic;
using System.Linq;
using CakeBot.Helper.Database.Model;

namespace CakeBot.Helper.Modules.Fishy
{
    public enum FishRarities
    {
        Trash = 0,
        Common = 1,
        Exotic = 2,
        Legendary = 3,
    }
    public static class FishLists
    {
        private static CakeEntities _db = new CakeEntities();

        // List of Every Rarity with Emoji's
        public static List<FishType> TrashList = new List<FishType>();
        public static List<FishType> CommonList = new List<FishType>();
        public static List<FishType> ExoticList = new List<FishType>();
        public static List<FishType> LegendaryList = new List<FishType>();

        static FishLists()
        {
            var fishes = _db.FishTypes.ToList();

            foreach(var fish in fishes)
            {
                if (!fish.FishActive) continue;
                switch (fish.FishRarity)
                {
                    case 0:
                        TrashList.Add(fish);
                        break;
                    case 1:
                        CommonList.Add(fish);
                        break;
                    case 2:
                        ExoticList.Add(fish);
                        break;
                    case 3:
                        LegendaryList.Add(fish);
                        break;
                }
            }
        }
    }
}
