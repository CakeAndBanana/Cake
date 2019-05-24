using CakeBot.Helper.Database.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeBot.Helper.Database.Queries
{
    public class BackgroundQueries
    {
        private static CakeEntities Db = new CakeEntities();
        private static string baseAddress = Directory.GetCurrentDirectory();
        public static async Task<List<ProfileBackground>> GetListOfAllBackgrounds()
        {
            Db = new CakeEntities();
            var result =
             await (from bgs in Db.ProfileBackgrounds
                    where bgs.BackgroundActive
                    select bgs).ToListAsync();
            return result;

        }

        public static async Task<List<UserBackground>> GetListOfOwnedBackgrounds(ulong userId)
        {
            Db = new CakeEntities();
            var result =
             await (from bgs in Db.UserBackgrounds
                    where bgs.UserId == (long) userId
                    select bgs).ToListAsync();
            return result;
        }

        public static async Task<string> SetUserBackground(int bgId, ulong userId)
        {
            Db = new CakeEntities();
            var bgList = await GetListOfOwnedBackgrounds(userId);
            var reqBg = bgList.Where(i => i.BackgroundId == bgId).FirstOrDefault();

            if(reqBg != null)
            {
                Db.Users.Where(u => u.UserId == (long)userId).FirstOrDefault().BackgroundId = reqBg.ProfileBackground.BackgroundId;
                Db.SaveChanges();
                return $", background succesfully changed to: {reqBg.ProfileBackground.BackgroundDir}! Enjoy!";
            }

            return ", background not changed, you do not own the background!";
        }

        public static async Task<string> BuyBackground(int bgId, ulong userId)
        {
            var bgList = await GetListOfAllBackgrounds();
            var bgToBuy = bgList.Where(bg => bg.BackgroundId == bgId).FirstOrDefault();
            if (bgToBuy == null) return ", Invalid Background Id!";

            Db = new CakeEntities();
            var result =
             await (from bgs in Db.UserBackgrounds
                    where bgs.UserId == (long)userId
                    select bgs).ToListAsync();
            var user = result.Where(x => x.BackgroundId == bgId && x.UserId == (long)userId).FirstOrDefault();
            if (user != null)
            {
                await SetUserBackground(user.BackgroundId, (ulong)user.UserId);
                return ", you already own that background, but we equipped it!";

            }

            UserBackground userBg = new UserBackground()
            {
                BackgroundId = bgToBuy.BackgroundId,
                UserId = (long)userId
            };

            if (!await EconomyQueries.RemoveMoney(userId, bgToBuy.BackgroundPrice, Db)) { return ", you did not have enough money to buy the bakground!"; }

            Db.UserBackgrounds.Add(userBg);
            Db.SaveChanges();
            string s = await SetUserBackground(bgId, userId);
            return $", succesfully purchased and equipped **{bgToBuy.BackgroundDir}** for :euro: {bgToBuy.BackgroundPrice}";
        }

        public static async Task<string> SendFullBackgroundLocation(int index)
        {
            Db = new CakeEntities();
            var result = await Db.ProfileBackgrounds.ToListAsync();
            if(result[index] != null) return baseAddress + @"\Images\" + result[index].BackgroundDir + ".png";
            return "0";
        }
    }

    
}
