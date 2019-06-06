using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Cake.Modules.Services
{
    public class CakeService : CustomBaseService
    {
        public async Task GetStatus()
        {
            var embed = new EmbedBuilder();
            embed.Title = "Status of Cake";
            embed.Description = $"Test";
            await Module.Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
