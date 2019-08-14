using Cake.Database.Models;
using System;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public class LevelHandler
    {
        public async Task GiveExpToUser(CakeUser user, int expToGive)
        {
            throw new NotImplementedException("Give experience to user, amount is depended on task");
        }

        public async Task IncrementLevel(CakeUser user, int amount = 1)
        {
            throw new NotImplementedException("Should also increase totalxp.");
        }

        public async Task DecreaseLevel(CakeUser user, int amount = 1)
        {
            throw new NotImplementedException("Should also increase totalxp.");
        }

        public async Task ResetUser(CakeUser user)
        {
            throw new NotImplementedException("Resets XP and Level of user.");
        }
    }
}
