using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;

namespace CakeBot.Helper.Database.Queries
{
    public class ChannelQueries
    {
        private static readonly CakeBotEntities Db = new CakeBotEntities();

        public static async Task<DiscordChannel> FindChannel(ulong channelId)
        {
            var result =
                await (from dc in Db.DiscordChannels
                    where dc.ChannelId == (long)channelId
                    select dc).ToListAsync();
            return result.FirstOrDefault();
        }

        public static async Task<int> GetMapId(ulong channelId, ulong guildId)
        {
            var channel = await FindChannel(channelId);

            if (channel == null)
            {
                var guild = await GuildQueries.FindGuild(guildId);
                if (guild == null)
                {
                    await GuildQueries.CreateGuild(guildId);
                }
                await CreateChannel(channelId, guildId);
                return 0;
            }
            else if (channel.LastMapId == null)
            {
                return 0;
            }

            return (int)channel.LastMapId;
        }

        public static async Task InsertMapId(ulong channelId, ulong guildId, int mapId)
        {
            var channel = await FindChannel(channelId);

            if (channel == null)
            {
                var guild = await GuildQueries.FindGuild(guildId);
                if (guild == null)
                {
                    await GuildQueries.CreateGuild(guildId);
                }
                await CreateChannel(channelId, guildId);
                return;
            }

            channel.LastMapId = mapId;

            Db.Entry(channel).State = EntityState.Modified;
            await Db.SaveChangesAsync();
        }

        public static async Task CreateChannel(ulong channelId, ulong guildId)
        {
            var newchannel = new DiscordChannel
            {
                ChannelId = Convert.ToInt64(channelId),
                GuildId = Convert.ToInt64(guildId),
                LastMapId = 0
            };

            Db.DiscordChannels.Add(newchannel);
            await Db.SaveChangesAsync();
        }
    }
}
