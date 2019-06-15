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
                .AddField("Message", joinedUser.Mention + " joined the server!")
                .AddField("Creation Date", joinedUser.CreatedAt)
                .WithThumbnailUrl(joinedUser.GetAvatarUrl())
                .WithFooter(footer =>
                {
                    footer
                        .WithText(username)
                        .WithIconUrl(avatarUrl);
                })
                .WithTimestamp(DateTime.Now) as CakeEmbedBuilder;
        }

        public static CakeEmbedBuilder ReturnLeaveEmbed(SocketGuildUser leftUser, string username, string avatarUrl)
        {
            return new CakeEmbedBuilder()
                .AddField("Message", leftUser.Mention + " left the server!")
                .AddField("Creation Date", leftUser.CreatedAt)
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
