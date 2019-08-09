using Cake.Core.Discord.Embed.Builder;
using Discord;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Cake.Core.Discord.Embed
{
    public class CakeModuleEmbeds
    {
        protected CakeModuleEmbeds()
        {
        }

        public static CakeEmbedBuilder ReturnStatusEmbed(IGuild guild)
        {
            return new CakeEmbedBuilder()
                .WithTitle($"Status of {Main.GetClient().CurrentUser.Username}")
                .WithAuthor(m => m.IconUrl = Main.GetClient().CurrentUser.GetAvatarUrl())
                .WithDescription($"Version: ``{Json.CakeJson.GetConfig().Version}``\n" + 
                              $"Discord Latency: ``{Main.GetClient().Latency}ms``\n" +
                              $"Shards: ``{Main.GetClient().Shards.Count}``\n" +
                              $"Guild Shard: ``{Main.GetClient().GetShardIdFor(guild) + 1}``\n" +
                              $"Uptime of {Main.GetClient().CurrentUser.Username}: ``{DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime.ToUniversalTime()).ToString()}``") as CakeEmbedBuilder;
        }
    }
}
