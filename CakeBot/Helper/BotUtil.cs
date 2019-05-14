using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace CakeBot.Helper
{
    public class BotUtil
    {
        private static DiscordSocketClient _client;

        public static void Init()
        {
            _client = Startup.GetClient();
        }

        public static async Task SetBotStatus(UserStatus status)
        {
            Init();
            await _client.SetStatusAsync(status);
        }

        public static async Task SetBotGame(string gameStatus)
        {
            Init();
            await _client.SetGameAsync(gameStatus);
        }

        public static async Task SetBotName(string name)
        {
            Init();
            await _client.CurrentUser.ModifyAsync(x => x.Username = name);
        }

        public static SocketGuild GetGuild(ulong guildId)
        {
            Init();
            return _client.GetGuild(guildId);
        }
    }
}
