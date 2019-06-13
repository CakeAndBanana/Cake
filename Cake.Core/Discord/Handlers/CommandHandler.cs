using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Cake.Core.Logging;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Type = Cake.Core.Logging.Type;

namespace Cake.Core.Discord.Handlers
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;
        private readonly Logger _logger = Logger.Get() as Logger;

        public CommandHandler(DiscordShardedClient client, ServiceCollection serviceCollection)
        {
            _client = client;
            _services = serviceCollection.BuildServiceProvider();
            _commandService = new CommandService();
        }

        public async Task InitializeAsync()
        {
            _client.MessageReceived += HandleCommandEvent;
            _client.UserJoined += JoinHandler.UserJoined;
            _client.UserLeft += JoinHandler.UserLeft;

            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
        }

        public async Task HandleCommandEvent(SocketMessage message)
        {
            if (!(message is SocketUserMessage msg) || msg.Author.IsBot) return;
            await PrefixCommandAsync(new ShardedCommandContext(_client, msg));
        }

        private async Task PrefixCommandAsync(ShardedCommandContext context, int argPos = 0)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            try
            {
                var guild = await Database.Query.GuildQueries.FindOrCreateGuild(context.Guild.Id);
                if (context.Message.HasCharPrefix(Convert.ToChar(guild.Prefix), ref argPos))
                {
                    var stopwatch = Stopwatch.StartNew();
                    var result = await _commandService.ExecuteAsync(context, 1, _services);
                    stopwatch.Stop();

                    if (!result.IsSuccess)
                    {
                        // Error handler
                    }
                    else
                    {
                        _logger.Log(Type.Info, $"\nCommand executed by {context.User}({context.User.Id}) in guild {context.Guild}({context.Guild.Id})\nTime taken to execute command is {stopwatch.ElapsedMilliseconds}ms");
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
