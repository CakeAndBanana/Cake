using System.Threading.Tasks;
using Discord.WebSocket;

namespace Cake.Core
{
    interface ICake
    {
        Task StartAsync();

        void SetupBot();
        DiscordShardedClient GetClient();
    }
}
