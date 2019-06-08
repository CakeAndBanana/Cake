using System.Threading.Tasks;
using Cake.Core.Discord.Modules;
using Discord;

namespace Cake.Core.Discord.Services
{
    public class CakeService
    {
        protected CakeModule Module;

        public void SetBaseModule(CakeModule module)
        {
            Module = module;
        }

        public async Task GetStatus()
        {
            var embed = new EmbedBuilder {Title = "Status of Cake", Description = $"Test"};
            await Module.Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
