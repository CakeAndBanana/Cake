using System;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using Discord;

namespace CakeBot.Helper.Modules.Fishy
{
    public class Catch
    {
        private const int totalChance = 1000;
        private int TrashAmount = (int)(totalChance / 100 * 51.70); // Trash Chance 51,70% == 527
        private int commonAmount = (int)(totalChance / 100 * 37); // Common Chance 37% == 370
        private int exoticAmount = (int)(totalChance / 100 * 10); // Exotic Chance 10%
                                                                // private int legendaryAmount = (int)(totalChance / 100 * 0.3); // Legendary Chance 0,30%

        private CakeEntities _db;
        public FishType Fish { get; }
        public  DateTime catchDate { get; }
        public long catchReward { get; }
        public long userId { get; }

        public Catch(ulong usr, Random r, CakeEntities db)
        {
            _db = db;
            userId = (long)usr;
            catchDate = DateTime.Now;
            int rNumber = r.Next(totalChance+1);

            if(rNumber <= TrashAmount)
            {
                Fish = FishLists.TrashList[r.Next(FishLists.TrashList.Count)];
            }
            else if (rNumber <= TrashAmount + commonAmount){
                Fish = FishLists.CommonList[r.Next(FishLists.CommonList.Count)];
            }
            else if (rNumber <= TrashAmount + commonAmount + exoticAmount)
            {
                Fish = FishLists.ExoticList[r.Next(FishLists.ExoticList.Count)];
            }
            else if (rNumber > TrashAmount + commonAmount + exoticAmount)
            {
                Fish = FishLists.LegendaryList[r.Next(FishLists.LegendaryList.Count)];
            }

            FishQueries.AddToLog(this);
        }

        public string ToString(IUser user)
        {
            if (Fish.FishId == 25) return $"**{user.Username}**, {Fish.FishMessage.ToLower()}, you were charged :euro: **10** for casting";
            return $"**{Fish.FishMessage}** | **{user.Username}**, you catched a {Fish.FishEmoji}! You were charged :euro: **10** for casting";
        }
    }
}

