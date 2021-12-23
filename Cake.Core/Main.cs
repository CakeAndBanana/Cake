using System;
using System.Threading.Tasks;
using Cake.Core.Configuration;
using Cake.Core.Discord.Handlers;
using Cake.Core.Logging;
using Cake.Json;
using Discord;
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
        private bool Running { get; set; }

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
                    await _client.LoginAsync(TokenType.Bot, _cakeConfiguration.BotKey);
                    await _client.StartAsync().ConfigureAwait(false);
                    await _client.SetStatusAsync(UserStatus.Online);
                    await _commandHandler.InitializeAsync();

                    Running = true;
                    _logger.Log(Type.Info, "Connected");
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogException(e);
                    _logger.Log(Type.Error, "Failed to connect");
                }
            }

            while (Running)
            {
                await Task.Delay(RunningInterval).ConfigureAwait(false);
            }
        }

        public async Task StopASync()
        {
            await _client.LogoutAsync();
            _client.Dispose();
            _logger.Log(Type.Info, "~Stopped Cake~");
        }

        public static DiscordShardedClient GetClient()
        {
            return _client;
        }

        public void SetupBot()
        {
            _logger.Log(Type.Info, "Setting up Cake");
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };
            _client = new DiscordShardedClient(config);
            _commandHandler = new CommandHandler(_client, new SetupServices().ReturnProvider());
            _cakeConfiguration = new CakeConfiguration();
            _ = new Database.Init();

            Running = false;
        }
    }
}
