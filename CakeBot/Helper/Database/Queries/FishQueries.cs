using CakeBot.Helper.Database.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.Fishy;

namespace CakeBot.Helper.Database.Queries
{
    public static class FishQueries
    {
        private static CakeEntities Db = new CakeEntities();
        private const long catchPrice = 10;
        public static async Task<bool> AddFish(FishType fish)
        {
            try
            {
                Db.FishTypes.Add(fish);
                await Db.SaveChangesAsync();
                Db = new CakeEntities();
                switch (fish.FishRarity)
                {
                    case 0:
                        FishLists.TrashList.Add(fish);
                        break;
                    case 1:
                        FishLists.CommonList.Add(fish);
                        break;
                    case 2:
                        FishLists.ExoticList.Add(fish);
                        break;
                    case 3:
                        FishLists.LegendaryList.Add(fish);
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return false;
            }

        }
        public static async Task AddToLog(Catch userCatch)
        {
                // Logs the fish and adds it to the User's inventory
                var result =
                await (from ufi in Db.UserFishInventories
                    where ufi.UserId == userCatch.userId
                    select ufi).ToListAsync();

            var invUser = result.FirstOrDefault(n => n.FishId == userCatch.Fish.FishId);

            try
            {
                var user = await Db.Users.FindAsync(userCatch.userId);
                var fish = await Db.FishTypes.FindAsync(userCatch.Fish.FishId);


                if (fish != null && user != null)
                {
                    var log = new UserFishLog()
                    {
                        UserId = userCatch.userId,
                        FishId = fish.FishId,
                        DateFished = userCatch.catchDate,
                        MoneyBefore = user.UserEconomy.UserMoney,
                    	MoneyAfter = user.UserEconomy.UserMoney -= catchPrice,
                    };

                    if(invUser != null)
                    {
                        invUser.Amount += 1;
                    }
                    else
                    {
                        var inv = new UserFishInventory()
                        {
                            UserId = user.UserId,
                            FishId = fish.FishId,
                            Amount = 1,
                            AmountSold = 0
                        };
                        Db.UserFishInventories.Add(inv);
                    }
                    Db.UserFishLogs.Add(log);
                    
                }

                await Db.SaveChangesAsync();
                Db = new CakeEntities();
            }
            catch(Exception e)
            {
                if (e.InnerException != null) Logger.LogError(e.InnerException.Message);
            }
        }
        public static async Task<bool> CheckEligibility(ulong userId)
        {
            Db = new CakeEntities();
            var result =
                await (from ufi in Db.UserFishLogs
                       where ufi.UserId == (long)userId
                       select ufi).ToListAsync();
            if (result.Count == 0) return true;
            if (result.OrderByDescending(x => x.DateFished).FirstOrDefault().DateFished.AddSeconds(20) <= DateTime.Now) return true;
            return false;
        }
        public static async Task<string> GetTimeRemaining(ulong userId)
        {
            Db = new CakeEntities();
            var result =
                await (from ufi in Db.UserFishLogs
                       where ufi.UserId == (long)userId
                       select ufi).OrderByDescending(x => x.DateFished).ToListAsync();
            TimeSpan timeRemaining = result.FirstOrDefault().DateFished.AddSeconds(20) - DateTime.Now;
            return timeRemaining.Seconds.ToString();
        }

        public static async Task<string> SellFish(ulong userId,int fishId, CakeEntities db)
        {
            // Sells the fish and adds the reward to the User's money
            var result =
            await (from ufi in db.UserFishInventories
                   where ufi.UserId == (long)userId
                   select ufi).ToListAsync();

            var invUser = result.FirstOrDefault(n => n.FishId == fishId);
            if (invUser != null)
            {
                if (invUser.Amount>0)
                {
                    var amountToSell = invUser.Amount;
                    var moneyToAdd = invUser.Amount * invUser.FishType.FIshWorth;
                    invUser.AmountSold += invUser.Amount;
                    invUser.Amount = 0;
                    if (await EconomyQueries.AddMoney(userId, (long)moneyToAdd, db)) return $"You succesfully sold {amountToSell} x {invUser.FishType.FishEmoji} for 💶 **{moneyToAdd}**";
                }
                else return "You dont have that fish in your Inventory";
            }
            return "You have never even caught this fish yet";

        }

        public static async Task<string> SellFishByGroup(ulong userId, string groupName , CakeEntities db)
        {
            int totalSold = 0;
            long totalReward = 0;
            int groupId = 100;

            switch (groupName.ToLower())
            {
                case "trash":
                    groupId = 0;
                    break;
                case "common":
                case "commons":
                    groupId = 1;
                    break;
                case "exotic":
                case "exotics":
                    groupId = 2;
                    break;
                case "legendary":
                case "legendaries":
                    groupId = 3;
                    break;
                default:
                    return $"**{groupName}** Group doesnt exist!";
            }

            var result = 
            await (from ufi in db.UserFishInventories
                   where ufi.UserId == (long)userId
                   select ufi).ToListAsync();

            var fishGroup = result.Where(n => n.FishType.FishRarity == groupId).ToList();

            if (fishGroup != null)
            {
                foreach (var invUser in fishGroup)
                {
                    if (invUser.Amount > 0)
                    {
                        totalReward += invUser.Amount * invUser.FishType.FIshWorth;
                        totalSold += invUser.Amount;
                        invUser.AmountSold += invUser.Amount;
                        invUser.Amount = 0;
                    }
                    else continue;
                }
                if (totalReward == 0) return "you do not have any fishes of this group in your inventory!";
                if (await EconomyQueries.AddMoney(userId, totalReward, db))
                {
                    return $"you succesfully sold **{totalSold}** x **{groupName}** fishes for 💶 **{totalReward}**";
                }
            }
            return "you have never even caught a in this group fish yet";
        }

        public static async Task<long> GetTotalCasts(ulong userId)
        {
            Db = new CakeEntities();
            var result =
                await (from ufi in Db.UserFishLogs
                       where ufi.UserId == (long)userId
                       select ufi).ToListAsync();
            return result.Count;
        }
    }
}
