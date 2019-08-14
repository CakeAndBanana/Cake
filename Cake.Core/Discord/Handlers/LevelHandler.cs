using Cake.Database.Models;
using System;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public class LevelHandler
    {
        public async Task GiveExpToUser(CakeUser user, int expToGive)
        {
            throw new NotImplementedException("");
        }

        private async Task IncrementLevel(CakeUser user, int amount = 1)
        {
            throw new NotImplementedException("");
        }

        private async Task DecreaseLevel(CakeUser user, int amount = 1)
        {
            throw new NotImplementedException("");
        }
    }
}
