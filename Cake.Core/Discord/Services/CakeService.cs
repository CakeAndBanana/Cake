using System.Threading.Tasks;
using Cake.Core.Discord.Embed.Builder;

namespace Cake.Core.Discord.Services
{
    public class CakeService : CustomBaseService
    {
        public async Task GetStatus()
        {
            var embed = new CakeEmbedBuilder {Title = "Status of Cake", Description = $"Test"};
            await SendEmbedAsync(embed);
        }
    }
}
