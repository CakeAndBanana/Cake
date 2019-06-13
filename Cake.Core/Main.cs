using System;
using System.Threading.Tasks;
using Cake.Core.Configuration;
using Cake.Core.Discord.Handlers;
using Cake.Core.Logging;
using Cake.Storage;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Type = Cake.Core.Logging.Type;

namespace Cake.Core
{
    public class Main : ICake
    {
        private static DiscordShardedClient _client;
        private ServiceCollection _services;
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
                    await _client.LoginAsync(TokenType.Bot, _cakeConfiguration.BotKey);
                    await _client.StartAsync().ConfigureAwait(false);
                    await _client.SetStatusAsync(UserStatus.Online);
                    await _commandHandler.InitializeAsync();

                    var Guilds = await Database.Queries.GuildQueries.GetGuilds();

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

        public void SetupBot()
        {
            _logger.Log(Type.Info, "Setting up Cake");

            _client = new DiscordShardedClient();
            _services = new SetupServices().ReturnProvider();
            _commandHandler = new CommandHandler(_client, _services);
            _cakeConfiguration = new CakeConfiguration();
            Database.Init.Startup();

            _running = false;
        }
    }
}
