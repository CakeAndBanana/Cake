using Cake.Database.Models;
using Cake.Database.Queries;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public static class LevelHandler
    {
        public static async Task<bool> GiveExpToUser(CakeUser user, int expToGive)
        {
            user.TotalXp += expToGive;
            if (user.GetCurrentExp() >= user.GetNextLevelExp())
            {
                user.Level += 1;
                await UserQueries.Update(user);
                return true;
            }
            await UserQueries.Update(user);
            return false;
        }

        public static async Task<CakeUser> IncrementLevels(CakeUser user, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                user.TotalXp += user.GetNextLevelExp();
                user.Level++;
            }
            await UserQueries.Update(user);
            return user;
        }

        public static async Task<CakeUser> DecreaseLevels(CakeUser user, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                if (user.GetNextLevelExp() < user.GetCurrentExp())
                {
                    user.TotalXp -= user.GetNextLevelExp();
                    user.Level--;
                }
            }
            await UserQueries.Update(user);
            return user;
        }

        public static async Task<CakeUser> ResetUser(CakeUser user)
        {
            user.TotalXp = 0;
            user.Level = 1;
            await UserQueries.Update(user);
            return user;
        }
    }
}
