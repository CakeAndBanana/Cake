using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;

namespace CakeBot.Helper.Database.Queries
{
    public class EconomyQueries
    {
        public static async Task<long> FindMoney(ulong discordId, CakeEntities db)
        {
            var result =
                await (from u in db.UserEconomies
                    where u.UserId == (long)discordId
                    select u).ToListAsync();
            return result.FirstOrDefault().UserMoney;
        }

        public static async Task<bool> AddMoney(ulong discordId, long amount , CakeEntities db)
        {
            var result =
                await (from u in db.UserEconomies
                       where u.UserId == (long)discordId
                       select u).ToListAsync();
                result.FirstOrDefault().UserMoney += amount;
            db.SaveChanges();
            return true;
        }

        public static async Task<bool> RemoveMoney(ulong discordId, long amount, CakeEntities db)
        {
            var result =
                await (from u in db.UserEconomies
                    where u.UserId == (long)discordId
                    select u).ToListAsync();
            var newMoney = result.FirstOrDefault().UserMoney -= amount;
            if (newMoney < 0) return false;
            db.SaveChanges();
            return true;
        }
        public static async Task<string> CheckDaily(ulong discordId, CakeEntities db)
        {
            var result =
                await (from u in db.UserEconomies
                       where u.UserId == (long)discordId
                       select u).ToListAsync();

            if(result.FirstOrDefault().UserDaily.AddDays(1) < DateTime.Now)
            {
                await AddMoney(discordId, 250, db);
                result.FirstOrDefault().UserDaily = DateTime.Now;
                db.SaveChanges();
                return "you added your daily 💶 **250** to your balance!";
            }
            TimeSpan timeRemaining = (result.FirstOrDefault().UserDaily.AddDays(1) - DateTime.Now);
            return $"you need to wait {timeRemaining.Hours} hours, {timeRemaining.Minutes} minutes and {timeRemaining.Seconds} seconds before you get your Daily bonus again";
        }
    }
}
