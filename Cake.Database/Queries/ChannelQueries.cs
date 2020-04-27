using Cake.Database.Models;
using LinqToDB;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class ChannelQueries
    {
        public static IQueryable<CakeChannel> GetAll()
        {
            using (var db = new CakeDb())
            {
                return db.GetTable<CakeChannel>().AsQueryable();
            }
        }

        public static async Task<CakeChannel> Update(CakeChannel channel)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(channel);
            }
            return channel;
        }

        public static async Task<CakeChannel> CreateChannel(ulong channelId, ulong guildId)
        {
            using (var db = new CakeDb())
            {
                var newChannel = new CakeChannel
                {
                    Id = channelId,
                    GuildId = guildId,
                    Restrict = false,
                    OsuMapId = 0
                };

                await db.InsertAsync(newChannel);
                return newChannel;
            }
        }
        private static async Task<CakeChannel> Get(ulong id) => await GetAll().Where(cc => cc.Id == id).FirstOrDefaultAsync();
        public static async Task<CakeChannel> FindOrCreateChannel(ulong channelId, ulong guildId = 0) => await Get(channelId).ConfigureAwait(false) ?? await CreateChannel(channelId, guildId).ConfigureAwait(false);
    }
}
