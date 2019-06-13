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

        public async Task SetPrefix(string newprefix)
        {
            try
            {
                if (newprefix.Length > 4)
                {
                    throw new System.Exception("New prefix is too long for database.");
                }

                var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id);
                var oldprefix = guild.Prefix;
                guild.Prefix = newprefix;
                await Database.Queries.GuildQueries.UpdateGuild(guild);

                await SendMessageAsync($"``Prefix changed to '{newprefix}', old prefix was '{oldprefix}'.``");
            }
            catch
            {
                //
            }

        }
    }
}
