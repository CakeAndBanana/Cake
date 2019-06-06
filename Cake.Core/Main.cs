using System;
using System.Threading.Tasks;
using Cake.Core.Discord.Configuration;
using Cake.Core.Discord.Handlers;
using Cake.Core.Logging;
using Cake.Storage;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Type = Cake.Core.Logging.Type;

namespace Cake.Core
{

    public class Main : ICake
    {
        private static DiscordShardedClient _client;
        private ICommandHandler _commandHandler;
        private CakeConfiguration _cakeConfiguration;
        private readonly Logger _logger = Logger.Get() as Logger;

        private const int RunningInterval = 1000;
        private bool _running;

        public async Task StartAsync()
        {
            while (true)
            {
                _logger.Log(Type.Info, "Starting");
                SetupBot();
                _cakeConfiguration.BotKey = CakeJson.GetConfig().BotKey;

                try
                {
                    _logger.Log(Type.Info, "Connecting");
                    await _client.LoginAsync(TokenType.Bot, _cakeConfiguration.BotKey).ConfigureAwait(false);
                    await _client.StartAsync().ConfigureAwait(false);
                    await _client.SetStatusAsync(UserStatus.Online);
                    await _commandHandler.InitializeAsync();

                    _running = true;
                    _logger.Log(Type.Info, "Connected");

                    break;
                }
                catch (Exception e)
                {
                    _logger.LogException(e);
                    _logger.Log(Type.Error, "Failed to connect");
                }
            }

            while (_running)
            {
                await Task.Delay(RunningInterval);
            }
        }

        public static DiscordShardedClient GetClient()
        {
            return _client;
        }

        private void SetupBot()
        {
            _logger.Log(Type.Info, "Setup Bot");
            _client = new DiscordShardedClient();
            _commandHandler = new CommandHandler(_client);
            _cakeConfiguration = new CakeConfiguration();
            _running = false;
        }
    }
}
