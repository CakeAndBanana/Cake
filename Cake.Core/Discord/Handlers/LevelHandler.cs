using Cake.Database.Models;
using Cake.Database.Queries;
using System;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public class LevelHandler
    {
        public async Task<bool> GiveExpToUser(CakeUser user, int expToGive)
        {
            user.Xp += expToGive;
            user.TotalXp += expToGive;
            int nextLevelXp = (int)(125 * (user.Level * 1.45));
            if (user.Xp >= nextLevelXp)
            {
                user.Level += 1;
                user.Xp = (0 + user.Xp - nextLevelXp);
                await UserQueries.Update(user);
                return true;
            }

            await UserQueries.Update(user);
            return false;
        }

        public async Task IncrementLevel(CakeUser user, int amount = 1)
        {
            throw new NotImplementedException("Should also increase totalxp.");
        }

        public async Task DecreaseLevel(CakeUser user, int amount = 1)
        {
            throw new NotImplementedException("Should also increase totalxp.");
        }

        public CakeUser ResetUser(CakeUser user)
        {
            user.Xp = 0;
            user.TotalXp += 0;
            user.Level = 1;
            return user;
        }
    }
}
