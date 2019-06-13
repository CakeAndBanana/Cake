using Cake.Database.Model;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Query
{
    public class GuildQueries
    {
        public async static Task<List<CakeGuild>> GetGuilds()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeGuild>().ToListAsync();
            }
        }

        private async static Task<CakeGuild> GetGuild(ulong guildId)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cg in db.CakeGuilds
                                       where cg.Id == guildId
                                       select cg).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        private async static Task<CakeGuild> CreateGuild(ulong guildId)
        {
            using (var db = new CakeDb())
            {
                var newguild = new CakeGuild
                {
                    Id = guildId,
                    Prefix = ">",
                    Restrict = false,
                    LeaveId = null,
                    WelcomeId = null,
                    LevelUpId = null
                };

                await db.InsertAsync(newguild);
                return newguild;
            }
        }

        public async static Task<CakeGuild> FindOrCreateGuild(ulong guildId)
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
