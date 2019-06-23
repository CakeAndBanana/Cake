using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Cake.Database.Queries
{
    public class ChannelQueries
    {
        protected ChannelQueries()
        {
        }

        public static async Task<List<CakeChannel>> GetAll()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeChannel>().ToListAsync();
            }
        }

        private static async Task<CakeChannel> Get(ulong id)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cc in db.CakeChannels
                                    where cc.Id == id
                                    select cc).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        public static async Task<CakeChannel> Update(CakeChannel channel)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(channel);
                return channel;
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

        public static async Task<CakeChannel> FindOrCreateChannel(ulong channelId, ulong guildId = 0) => await Get(channelId).ConfigureAwait(false) ?? await CreateChannel(channelId, guildId).ConfigureAwait(false);
    }
}
