using System.Reflection;
using System.Threading.Tasks;
using Cake.Core.Discord.Embed.Builder;

namespace Cake.Core.Discord.Services
{
    public class CakeService : CustomBaseService
    {
        public async Task GetStatus()
        {
            var embed = new CakeEmbedBuilder
            {
                Title = "Status of Cake",
                Description = $"Version: ``{Assembly.GetExecutingAssembly().GetName().Version}``\n" +
                              $"Discord Latency: ``{Main.GetClient().Latency}ms``\n" +
                              $"Shards: ``{Main.GetClient().Shards.Count} / {Main.GetClient().GetShardIdFor(Module.Context.Guild) + 1}``"
            };
            await SendEmbedAsync(embed);
        }
    }
}
