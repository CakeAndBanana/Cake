using System;
using System.Threading.Tasks;
using Cake.Core.Discord.Configuration;
using Cake.Core.Discord.Handlers;
using Cake.Storage;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cake.Core
{
    public class Cake : ICake
    {
        private DiscordShardedClient _client;
        private ICommandHandler _commandHandler;
        private CommandService _commandService;
        private CakeConfiguration _cakeConfiguration;

        private const int RunningInterval = 1000;
        private bool _running;

        public async Task StartAsync()
        {
            while (true)
            {
                SetupBot();
                _cakeConfiguration.BotKey = CakeJson.GetConfig().BotKey;

                try
                {
                    await _client.LoginAsync(TokenType.Bot, _cakeConfiguration.BotKey).ConfigureAwait(false);
                    await _client.StartAsync().ConfigureAwait(false);
                    await _client.SetStatusAsync(UserStatus.Online);
                    await _commandHandler.InitializeAsync().ConfigureAwait(false);

                    _running = true;

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    Console.WriteLine("Failed to connect.");
                }
            }

            while (_running)
            {
                await Task.Delay(RunningInterval);
            }
        }
        private void SetupBot()
        {
            _client = new DiscordShardedClient();
            _commandService = new CommandService();
            _commandHandler = new CommandHandler(_client, _commandService);
            _cakeConfiguration = new CakeConfiguration();
            _running = false;
        }
    }
}
