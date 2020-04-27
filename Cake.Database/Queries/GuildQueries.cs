using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class GuildQueries
    {
        public static List<CakeGuild> GetAll()
        {
            return new DataContext().GetTable<CakeGuild>().ToList();
        }
        private static CakeGuild Get(ulong id) => GetAll().Where(cg => cg.Id == id).FirstOrDefault();

        public static async Task Update(CakeGuild guild)
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

        public static async Task<CakeGuild> FindOrCreateGuild(ulong guildId) => Get(guildId) ?? await CreateGuild(guildId).ConfigureAwait(false);
    }
}
