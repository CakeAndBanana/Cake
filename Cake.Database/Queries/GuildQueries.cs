using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class GuildQueries
    {
        public static async Task<List<CakeGuild>> GetAllGuilds()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeGuild>().ToListAsync();
            }
        }

        private static async Task<CakeGuild> GetGuild(ulong guildId)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cg in db.CakeGuilds
                                    where cg.Id == guildId
                                    select cg).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        public static async Task UpdateGuild(CakeGuild guild)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(guild);
            }
        }

        private static async Task<CakeGuild> CreateGuild(ulong guildId)
        {
            using (var db = new CakeDb())
            {
                var newGuild = new CakeGuild
                {
                    Id = guildId,
                    Prefix = ">",
                    Restrict = false,
                    LeaveId = null,
                    WelcomeId = null,
                    LevelUpId = null
                };

                await db.InsertAsync(newGuild);
                return newGuild;
            }
        }

        public static async Task<CakeGuild> FindOrCreateGuild(ulong guildId)
        {
            var guild = await GetGuild(guildId);
            if (guild == null)
            {
                guild = await CreateGuild(guildId);
            }
            return guild;
        }
    }
}
