using Cake.Core.Discord.Embed.Builder;
using Discord.WebSocket;
using System;

namespace Cake.Core.Discord.Embed
{
    public class JoinHandlerEmbeds
    {
        protected JoinHandlerEmbeds()
        {
        }

        public static CakeEmbedBuilder ReturnJoinedEmbed(SocketGuildUser joinedUser, string username, string avatarUrl)
        {
            return new CakeEmbedBuilder()
                .AddField("User joined! :)", joinedUser.Mention + " joined the server!")
                .AddField("Creation Date", joinedUser.CreatedAt.DateTime.ToShortDateString())
                .WithThumbnailUrl(joinedUser.GetAvatarUrl())
                .WithFooter(footer =>
                {
                    footer
                        .WithText(username)
                        .WithIconUrl(avatarUrl);
                })
                .WithTimestamp(DateTime.Now) as CakeEmbedBuilder;
        }

        public static CakeEmbedBuilder ReturnLeaveEmbed(SocketUser leftUser, string username, string avatarUrl)
        {
            return new CakeEmbedBuilder()
                .AddField("User left... :(", leftUser.Mention + " left the server!")
                .AddField("Creation Date", leftUser.CreatedAt.DateTime.ToShortDateString())
                .WithThumbnailUrl(leftUser.GetAvatarUrl())
                .WithFooter(footer =>
                {
                    footer
                        .WithText(username)
                        .WithIconUrl(avatarUrl);
                })
                .WithTimestamp(DateTime.Now) as CakeEmbedBuilder;
        }
    }
}
