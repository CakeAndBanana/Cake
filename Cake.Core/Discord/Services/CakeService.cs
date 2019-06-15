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
                await Database.Queries.GuildQueries.Update(guild);

                await SendMessageAsync($"``Prefix changed to '{newPrefix}', old prefix was '{oldPrefix}'.``");
            }
            catch
            {
                // WIP
            }
        }

        public async Task SetWelcome(ulong channelId)
        {
            var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id);
            guild.WelcomeId = channelId;
            await Database.Queries.GuildQueries.Update(guild);

            await SendMessageAsync("`Set this channel as welcome message channel!`");
        }

        public async Task SetLeave(ulong channelId)
        {
            var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id);
            guild.LeaveId = channelId;
            await Database.Queries.GuildQueries.Update(guild);

            await SendMessageAsync("`Set this channel as leave message channel!`");
        }

        public async Task SetAdmin(ulong id)
        {
            var user = await Database.Queries.UserGueries.FindOrCreateUser(id);
            user.Admin = !user.Admin;
            await Database.Queries.UserGueries.Update(user);

            await SendMessageAsync($"Updated given user admin status to {user.Admin}.");
        }

        public async Task RestrictGuild(ulong guildId)
        {
            string message;
            var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(guildId);
            if (guild.Restrict)
            {
                guild.Restrict = false;
                message = $"`Unrestricted guild {guildId} from using Cake!`";
            }
            else
            {
                guild.Restrict = true;
                message = $"`Restricted guild {guildId} from using Cake!`";
            }
            await Database.Queries.GuildQueries.Update(guild);

            await SendMessageAsync(message);
        }

        public async Task RestrictUser(ulong userId)
        {
            string message;
            var user = await Database.Queries.UserGueries.FindOrCreateUser(userId);
            if (user.Restrict)
            {
                user.Restrict = false;
                message = $"`Unrestricted user {userId} from using Cake!`";
            }
            else
            {
                user.Restrict = true;
                message = $"`Restricted user {userId} from using Cake!`";
            }
            await Database.Queries.UserGueries.Update(user);

            await SendMessageAsync(message);
        }
    }
}
