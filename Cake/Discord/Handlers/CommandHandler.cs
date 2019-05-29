using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Discord.Handlers
{
    class CommandHandler : ICommandHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _commandService;
        private IServiceProvider _services;

        public CommandHandler(DiscordShardedClient client, CommandService commandService)
        {
            _client = client;
            _commandService = commandService;
        }

        public async Task InitializeAsync()
        {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .BuildServiceProvider();

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services).ConfigureAwait(false);

            _client.MessageReceived += HandleCommandEvent;
        }

        private Task HandleCommandEvent(SocketMessage message)
        {
            if (!(message is SocketUserMessage msg)) return Task.CompletedTask;
            Task.Run(async () => HandleCommandAsync(msg).ConfigureAwait(false));
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketUserMessage msg)
        {
            try
            {
                if (msg.Author.IsBot) return;

                var context = new ShardedCommandContext(_client, msg);
                int argPos = 0;

                if (context.Message.HasCharPrefix(Convert.ToChar("="), ref argPos))
                {
                    await context.Channel.SendMessageAsync($"`Shard : {_client.GetShardIdFor(context.Guild)}`");
                }

            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}
