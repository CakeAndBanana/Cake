using System;
using System.Threading.Tasks;
using Cake.Core.Discord.Configuration;
using Cake.Core.Discord.Handlers;
using Cake.Core.Discord.Services;
using Cake.Logger;
using Cake.Storage;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Type = Cake.Logger.Type;

namespace Cake.Core
{
    public class Cake : ICake
    {
        private DiscordShardedClient _client;
        private ICommandHandler _commandHandler;
        private CommandService _commandService;
        private CakeConfiguration _cakeConfiguration;
        private IServiceProvider _services;
        private ILogger _logger = Logger.Logger.Get();

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
                    await _commandHandler.InitializeAsync().ConfigureAwait(false);

                    _running = true;
                    _logger.Log(Type.Success, "Connected");

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
        private void SetupBot()
        {
            _logger.Log(Type.Info, "Setup Bot");
            _client = new DiscordShardedClient();
            _commandService = new CommandService();
            _commandHandler = new CommandHandler(_client, _commandService);
            _services = new SetupServices().ReturnProvider();
            _cakeConfiguration = new CakeConfiguration();
            _running = false;
        }
    }
}
