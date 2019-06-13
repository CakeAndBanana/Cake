using Cake.Core.Discord.Embed.Builder;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public class JoinHandler
    {
        public static async Task UserJoined(SocketGuildUser joinedUser)
        {
            var embed = new CakeEmbedBuilder()
                .AddField("Message", joinedUser.Mention + " joined the server!")
                .AddField("Creation Date", joinedUser.CreatedAt)
                .WithThumbnailUrl(joinedUser.GetAvatarUrl())
                .WithFooter(footer =>
                {
                    footer
                        .WithText(Main.GetClient().CurrentUser.Username)
                        .WithIconUrl(Main.GetClient().CurrentUser.GetAvatarUrl());
                })
                .WithTimestamp(DateTime.Now);

            var guild = joinedUser.Guild;
            var cakeGuild = await Database.Query.GuildQueries.FindOrCreateGuild(guild.Id);
            SocketTextChannel channel = null;

            channel = cakeGuild.WelcomeId == null ? guild.DefaultChannel : guild.GetTextChannel(cakeGuild.WelcomeId.Value);

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task UserLeft(SocketGuildUser leftUser)
        {
            var embed = new CakeEmbedBuilder()
                .AddField("Message", leftUser.Mention + " left the server!")
                .AddField("Creation Date", leftUser.CreatedAt)
                .WithThumbnailUrl(leftUser.GetAvatarUrl())
                .WithFooter(footer =>
                {
                    footer
                        .WithText(Main.GetClient().CurrentUser.Username)
                        .WithIconUrl(Main.GetClient().CurrentUser.GetAvatarUrl());
                })
                .WithTimestamp(DateTime.Now);

            var guild = leftUser.Guild;
            var cakeGuild = await Database.Query.GuildQueries.FindOrCreateGuild(guild.Id);
            SocketTextChannel channel = null;

            channel = cakeGuild.LeaveId == null ? guild.DefaultChannel : guild.GetTextChannel(cakeGuild.LeaveId.Value);

            await channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
