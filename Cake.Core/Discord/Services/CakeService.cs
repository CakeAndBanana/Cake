using System.Threading.Tasks;

namespace Cake.Core.Discord.Services
{
    public class CakeService : CustomBaseService
    {
        public async Task GetStatus()
        {
            await SendEmbedAsync(Embed.CakeModuleEmbeds.ReturnStatusEmbed(Module.Context.Guild));
        }

        public async Task GetPrefix()
        {
            await SendMessageAsync($"``Current prefix in this server is {Database.Queries.GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id).Result.Prefix}.``");
        }

        public async Task SetPrefix(string newPrefix)
        {
            try
            {
                if (newPrefix.Length > 4)
                {
                    throw new System.Exception("New prefix is too long for database.");
                }

                var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id);
                var oldPrefix = guild.Prefix;
                guild.Prefix = newPrefix;
                await Database.Queries.GuildQueries.UpdateGuild(guild);

                await SendMessageAsync($"``Prefix changed to '{newPrefix}', old prefix was '{oldPrefix}'.``");
            }
            catch
            {
                //
            }
        }

        public async Task SetWelcome(ulong channelId)
        {
            var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id);
            guild.WelcomeId = channelId;
            await Database.Queries.GuildQueries.UpdateGuild(guild);

            await SendMessageAsync("`Set this channel as welcome message channel!`");
        }

        public async Task SetLeave(ulong channelId)
        {
            var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id);
            guild.LeaveId = channelId;
            await Database.Queries.GuildQueries.UpdateGuild(guild);

            await SendMessageAsync("`Set this channel as leave message channel!`");
        }
    }
}
