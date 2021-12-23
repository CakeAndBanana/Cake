using Cake.Core.Discord.Embed;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public class JoinHandler
    {
        public static async Task UserJoined(SocketGuildUser joinedUser)
        {
            var embed = JoinHandlerEmbeds.ReturnJoinedEmbed(joinedUser, Main.GetClient().CurrentUser.Username, Main.GetClient().CurrentUser.GetAvatarUrl());

            var guild = joinedUser.Guild;
            var cakeGuild = await Database.Queries.GuildQueries.FindOrCreateGuild(guild.Id);
            var channel = cakeGuild.WelcomeId == null ? guild.DefaultChannel : guild.GetTextChannel(cakeGuild.WelcomeId.Value);

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task UserLeft(SocketGuild guild, SocketUser leftUser)
        {
            var embed = JoinHandlerEmbeds.ReturnLeaveEmbed(leftUser, Main.GetClient().CurrentUser.Username, Main.GetClient().CurrentUser.GetAvatarUrl());

            var cakeGuild = await Database.Queries.GuildQueries.FindOrCreateGuild(guild.Id);
            var channel = cakeGuild.LeaveId == null ? guild.DefaultChannel : guild.GetTextChannel(cakeGuild.LeaveId.Value);

            await channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
