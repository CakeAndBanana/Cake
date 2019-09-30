using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class GuildQueries
    {
        protected GuildQueries()
        {
        }

        public static async Task<List<CakeGuild>> GetAll()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeGuild>().ToListAsync();
            }
        }

        private static async Task<CakeGuild> Get(ulong id)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cg in db.CakeGuilds
                                    where cg.Id == id
                                    select cg).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        public static async Task<CakeGuild> Update(CakeGuild guild)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(guild);
            }
            return guild;
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

        public static async Task<CakeGuild> FindOrCreateGuild(ulong guildId) => await Get(guildId).ConfigureAwait(false) ?? await CreateGuild(guildId).ConfigureAwait(false);
    }
}
