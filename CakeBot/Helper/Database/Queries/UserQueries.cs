using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;

namespace CakeBot.Helper.Database.Queries
{
    public static  class UserQueries
    {
        private static CakeBotEntities _db = new CakeBotEntities();

        public static async Task<User> FindUser(ulong discordId)
        {
            var result = 
                await (from u in _db.Users
                where u.UserId == (long) discordId
                select u).ToListAsync();
            _db = new CakeBotEntities();
            return result.FirstOrDefault();
        }

        public static async Task<int> GetUserLevel(ulong discordId)
        {
            _db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)discordId
                       select u).ToListAsync();
            return result.FirstOrDefault().UserLevel;
        }

        public static async Task<int> GetUserRank(ulong discordId)
        {
            _db = new CakeBotEntities();
            var allUsers = await _db.Users.ToListAsync();
            allUsers = allUsers.OrderByDescending(x => x.TotalXp).ToList();
            return allUsers.FindIndex(x => x.UserId == (long)discordId) + 1;
        }

        public static async Task<string> GetUserBackground(ulong discordId)
        {
            _db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)discordId
                       select u).ToListAsync();
            return result.FirstOrDefault().ProfileBackground.BackgroundDir;
        }

        public static async Task<bool> GrantXp(ulong discordId, int amount)
        {
            
            _db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)discordId
                       select u).ToListAsync();
            var user = result.FirstOrDefault();
            if (user != null)
            {
                user.UserXp += amount;
                user.TotalXp += amount;
                int nextLevelXp = (int) (Global.baseXp * (user.UserLevel * 1.45));
                if (user.UserXp >= nextLevelXp)
                {
                    user.UserLevel += 1;
                    user.UserXp = (0 + user.UserXp - nextLevelXp);
                    await _db.SaveChangesAsync();
                    return true;
                }
            }

            await _db.SaveChangesAsync();
            return false;
        }

        public static async Task<long> GetTotalXp(ulong discordId)
        {
            _db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)discordId
                       select u).ToListAsync();
            var user = result.FirstOrDefault();
            return user.TotalXp;
        }
        public static async Task<int> GetUserXp(ulong discordId)
        {
            _db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)discordId
                       select u).ToListAsync();
            return result.FirstOrDefault().UserXp;
        }

        public static async Task<UserEconomy> FindEcoUser(ulong discordId)
        {
            var result =
                await (from u in _db.UserEconomies
                       where u.UserId == (long)discordId
                       select u).ToListAsync();
            _db = new CakeBotEntities();
            return result.FirstOrDefault();
        }

        public static async Task<bool> CheckUser(ulong discordId)
        {
            //Find the user
            var dbUser = await FindUser(discordId);
            var ecoUser = await FindEcoUser(discordId);

            if (dbUser != null && ecoUser != null)
            {
                return true;
            }
            if (dbUser == null)
            {
                var newUser = new User
                {
                    UserId = (long)discordId,
                    UserAdmin = false,
                    UserLevel = 1,
                    UserXp = 0,
                    BackgroundId = 3,
                    ColorHex = "FFFFFF",
                    Restricted = false
                };
                _db.Users.Add(newUser);
            }
            if (ecoUser == null)
            {
                var econUser = new UserEconomy
                {
                    UserId = (long)discordId,
                    UserMoney = 100,
                    UserDaily = DateTime.Parse("1-1-1970 00:00"),
                };
                _db.UserEconomies.Add(econUser);
            }
            // Create a new record if dont have the user in the DB
            
            await _db.SaveChangesAsync();
            _db = new CakeBotEntities();
            return false;
        }

        public static async Task AddDeadChild(string name, int age, ulong guildId)
        {
            var guild = await GuildQueries.FindGuild(guildId);
            if (guild != null)
            {
                var child = new DeadChildren()
                {
                    Name = name,
                    DateOfDeath = DateTime.Now,
                    Age = age,
                    GuildId = guild.GuildId
                };
                _db.DeadChildrens.Add(child);
            }
            else
            {
                var newguild = await GuildQueries.CreateGuild(guildId);
                var child = new DeadChildren()
                {
                    Name = name,
                    DateOfDeath = DateTime.Now,
                    Age = age,
                    GuildId = newguild.GuildId
                };
                _db.DeadChildrens.Add(child);
            }

            await _db.SaveChangesAsync();
        }

        public static async Task<List<DeadChildren>> GetDeadChildren(CakeBotEntities db, ulong guildId)
        {
            var result =
                await (from u in _db.DeadChildrens
                    where u.GuildId == (long)guildId
                       select u).OrderBy(x => x.DateOfDeath).ToListAsync();
            return result;
        }


        public static async Task<bool> CheckAdmin(ulong discordId)
        {
            var result =
                await (from u in _db.Users
                    where u.UserId == (long) discordId && u.UserAdmin
                    select u).FirstOrDefaultAsync();
            _db = new CakeBotEntities();
            return result != null && result.UserAdmin;
        }

        public static bool CheckMoney(ulong discordId, CakeBotEntities _db)
        {
            return _db.UserEconomies.Find((long)discordId).UserMoney >=  10;
        }

        public static async Task<string> CorrectLevels(CakeBotEntities Db)
        {
            string correctedusers = "";
            foreach(var user in Db.Users.ToList())
            {
                long userxp = await GetTotalXp((ulong)user.UserId);
                long xpToCurrentLevel = 0;
                long currentxp = 0;
                long level = 0;
                correctedusers += $"`{user.UserId}: Level {user.UserLevel} ->";
                for (int i = 0; i < 99; i++)
                {
                    int neededxp = (int)(Global.baseXp * (i * 1.45));
                    long lastlevelneededxp = xpToCurrentLevel;
                    xpToCurrentLevel += neededxp;
                    if (userxp < xpToCurrentLevel)
                    {
                        currentxp = userxp - lastlevelneededxp;
                        break;
                    }
                    else level++;
                }
                correctedusers += $" {level}`\n";
                user.UserLevel = (int)level;
                user.UserXp = (int)currentxp;
                await Db.SaveChangesAsync();
            }

            return correctedusers;
        }

        public static async Task<bool> GetRestrictStatus(ulong userId)
        {
            _db = new CakeBotEntities();
            await CheckUser(userId);
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)userId
                       select u).ToListAsync();
             return result.FirstOrDefault().Restricted;
        }

        public static async Task SetRestrictStatus(ulong userId, bool status)
        {
            _db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)userId
                       select u).ToListAsync();
            var user = result.FirstOrDefault();
            if (user == null)
            {
                await CheckUser(userId);
                user = await FindUser(userId);
            }

            user.Restricted = status;
            _db.Entry(user).State = EntityState.Modified;

            await _db.SaveChangesAsync();
        }

        public static async Task<string> SetProfileColor(ulong userId, string Hex)
        {
            _db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)userId
                       select u).ToListAsync();
            var user = result.FirstOrDefault();
            if (user.UserLevel < 15) return ", Profile Colors are not unlocked until level 15!";
            user.ColorHex = Hex;
            await _db.SaveChangesAsync();
            return $", successfully changed color to {Hex}!";
        }

        public static async Task<string> GetProfileColor(ulong userId)
        {
            var Db = new CakeBotEntities();
            var result =
                await (from u in _db.Users
                       where u.UserId == (long)userId
                       select u).ToListAsync();
            var user = result.FirstOrDefault();
            return user?.ColorHex;
        }
    }
}
