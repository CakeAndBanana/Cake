using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;

namespace CakeBot.Helper.Database.Queries
{
    class BotQueries
    {
        private static readonly CakeBotEntities Db = new CakeBotEntities();

        public static async Task IncreaseCommands()
        {
            var result =
                await (from dc in Db.BotInfoes
                    select dc).FirstOrDefaultAsync();
            if (result != null)
            {
                result.Commands++;
                Db.Entry(result).State = EntityState.Modified;
            }
            await Db.SaveChangesAsync();
        }

        public static async Task SetGuilds(int guilds)
        {
            var result =
                await (from dc in Db.BotInfoes
                    select dc).FirstOrDefaultAsync();
            if (result != null)
            {
                result.Guilds = guilds;
                Db.Entry(result).State = EntityState.Modified;
            }
            await Db.SaveChangesAsync();
        }
    }
}
