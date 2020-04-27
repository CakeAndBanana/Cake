using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class ChannelQueries
    {
        public static List<CakeChannel> GetAll()
        {
            return new DataContext().GetTable<CakeChannel>().ToList();
        }
        private static CakeChannel Get(ulong id) => GetAll().Where(cg => cg.Id == id).FirstOrDefault();

        public static async Task Update(CakeChannel channel)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(channel);
            }
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

        public static async Task<CakeChannel> FindOrCreateChannel(ulong channelId, ulong guildId = 0) => Get(channelId) ?? await CreateChannel(channelId, guildId).ConfigureAwait(false);
    }
}
