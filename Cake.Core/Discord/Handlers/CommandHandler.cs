using System;
using System.Reflection;
using System.Threading.Tasks;
using Cake.Storage;
using Cake.Storage.DbQueries;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Discord.Handlers
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

        private async Task HandleCommandEvent(SocketMessage message)
        {
            if (!(message is SocketUserMessage msg)) return;
            await PrefixCommandAsync(msg);
        }


        private async Task PrefixCommandAsync(SocketUserMessage msg)
        {
            try
            {
                if (msg.Author.IsBot) return;

                var context = new ShardedCommandContext(_client, msg);

                var argPos = 0; 

                if (context.Message.HasCharPrefix(Convert.ToChar(new GuildQueries().GetPrefixGuild(context.Guild.Id)), ref argPos))
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
