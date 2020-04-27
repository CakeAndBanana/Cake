using Cake.Database.Models;
using LinqToDB;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class GuildQueries
    {
        public static IQueryable<CakeGuild> GetAll()
        {
            using (var db = new CakeDb())
            {
                return db.GetTable<CakeGuild>().AsQueryable();
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
        private static async Task<CakeGuild> Get(ulong id) => await GetAll().Where(cg => cg.Id == id).FirstOrDefaultAsync();
        public static async Task<CakeGuild> FindOrCreateGuild(ulong guildId) => await Get(guildId).ConfigureAwait(false) ?? await CreateGuild(guildId).ConfigureAwait(false);
    }
}
