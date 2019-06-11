using System.Threading.Tasks;

namespace Cake.Core.Discord.Services
{
    public class CakeService : CustomBaseService
    {
        public async Task GetStatus()
        {
            await SendEmbedAsync(Embed.CakeModuleEmbeds.ReturnStatusEmbed(Module.Context.Guild));
        }
    }
}
