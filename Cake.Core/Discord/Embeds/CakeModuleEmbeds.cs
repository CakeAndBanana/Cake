using Cake.Core.Discord.Embed.Builder;
using Discord;
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
                .WithTitle("Status of Cake")
                .WithAuthor(m => m.IconUrl = Main.GetClient().CurrentUser.GetAvatarUrl())
                .WithDescription($"Version: ``{Assembly.GetExecutingAssembly().GetName().Version}``\n" +
                              $"Discord Latency: ``{Main.GetClient().Latency}ms``\n" +
                              $"Shards: ``{Main.GetClient().Shards.Count}``" +
                              $"Guild Shard: ``{Main.GetClient().GetShardIdFor(guild) + 1}``") as CakeEmbedBuilder;
        }
    }
}
