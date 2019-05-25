using CakeBot.Helper.Database.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CakeBot.Helper.Database.Queries
{
    public class GuildQueries
    {
        private static readonly CakeEntities Db = new CakeEntities();

        public static async Task<DiscordGuild> FindGuild(ulong guildid)
        {
            var result =
                await (from dc in Db.DiscordGuilds
                       where dc.GuildId == (long)guildid
                       select dc).ToListAsync();
            return result.FirstOrDefault();
        }

        public static async Task<string> FindPrefix(ulong guildid)
        {
            var result =
                await (from dc in Db.DiscordGuilds
                       where dc.GuildId == (long)guildid
                       select dc).ToListAsync();
            if (result.Count == 0)
            {
                var guild = await CreateGuild(guildid);

                return guild.GuildPrefix;
            }
            return result.FirstOrDefault().GuildPrefix;
        }

        public static async Task<DiscordGuild> FindLeave(ulong guildid)
        {
            var result =
                await (from dc in Db.DiscordGuilds
                       where dc.GuildId == (long)guildid
                       select dc).ToListAsync();
            if (result.Count != 0) return result.FirstOrDefault();
            var guild = await CreateGuild(guildid);
            return guild;

        }
        public static async Task<DiscordGuild> FindWelcome(ulong guildid)
        {
            var result =
                await (from dc in Db.DiscordGuilds
                       where dc.GuildId == (long)guildid
                       select dc).ToListAsync();
            if (result.Count != 0) return result.FirstOrDefault();
            var guild = await CreateGuild(guildid);
            return guild;

        }

        public static async Task SetPrefix(ulong guildId, string prefix)
        {
            var guild = await FindGuild(guildId);

            if (guild == null)
            {
                await CreateGuild(guildId);
            }

            guild.GuildPrefix = prefix;

            Db.Entry(guild).State = EntityState.Modified;
            await Db.SaveChangesAsync();
        }

        public static async Task SetLeave(ulong guildId, ulong channelId)
        {
            var guild = await FindGuild(guildId);
            var channel = await ChannelQueries.FindChannel(channelId);

            if (guild == null)
            {
                await CreateGuild(guildId);
            }

            if (channel == null)
            {
                await ChannelQueries.CreateChannel(channelId, guildId);
            }

            guild.GuildLeave = (long?)channelId;
            Db.Entry(guild).State = EntityState.Modified;

            await Db.SaveChangesAsync();
        }

        public static async Task SetWelcome(ulong guildId, ulong channelId)
        {
            var guild = await FindGuild(guildId);
            var channel = await ChannelQueries.FindChannel(channelId);

            if (guild == null)
            {
                await CreateGuild(guildId);
            }

            if (channel == null)
            {
                await ChannelQueries.CreateChannel(channelId, guildId);
            }

            guild.GuildWelcome = (long?)channelId;
            Db.Entry(guild).State = EntityState.Modified;

            await Db.SaveChangesAsync();
        }

        public static async Task<DiscordGuild> CreateGuild(ulong guildId)
        {
            var newguild = new DiscordGuild
            {
                GuildId = Convert.ToInt64(guildId),
                GuildPrefix = ">"
            };

            Db.DiscordGuilds.Add(newguild);
            await Db.SaveChangesAsync();
            return newguild;
        }
    }
}
