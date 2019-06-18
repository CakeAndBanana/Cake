using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Cake.Core.Logging;
using Discord;
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
            if (!(message is SocketUserMessage msg) || msg.Author.IsBot || await message.Author.GetOrCreateDMChannelAsync(RequestOptions.Default) != null)
            {
                return;
            }

            await PrefixCommandAsync(new ShardedCommandContext(_client, msg)).ConfigureAwait(false);
        }

        private async Task PrefixCommandAsync(ShardedCommandContext context, int argPos = 0)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                var guild = await Database.Queries.GuildQueries.FindOrCreateGuild(context.Guild.Id);
                if (context.Message.HasCharPrefix(Convert.ToChar(guild.Prefix), ref argPos))
                {
                    if (Database.Queries.UserQueries.FindOrCreateUser(context.User.Id).Result.Restrict && guild.Restrict)
                    {
                        return;
                    }

                    var stopwatch = Stopwatch.StartNew();
                    var result = await _commandService.ExecuteAsync(context, 1, _services);
                    stopwatch.Stop();

                    if (!result.IsSuccess)
                    {
                        switch (result.Error)
                        {
                            case CommandError.ParseFailed:
                                _logger.Log(Type.Info, result.ErrorReason);
                                await context.Channel.SendMessageAsync("``Command couldn't parse correctly, check usage of command.``");
                                break;
                            case CommandError.Unsuccessful:
                                _logger.Log(Type.Info, result.ErrorReason);
                                await context.Channel.SendMessageAsync("``Command was unsuccessful, check usage of command.``");
                                break;
                            case CommandError.UnknownCommand:
                                // Annoying to use.
                                break;
                            case CommandError.UnmetPrecondition:
                                _logger.Log(Type.Info, $"{context.User}: {result.ErrorReason}");
                                await context.Channel.SendMessageAsync($"``{context.User} lack the sufficient permissions, check usage of command.``");
                                break;
                            case CommandError.Exception:
                                if(result.ErrorReason.Contains("Missing Permissions"))
                                {
                                    _logger.Log(Type.Info,$"\nNo permissions in guild {context.Guild}{context.Guild.Id}\nTrying to PM {context.User}.");
                                    try
                                    {
                                        var dmChannel = await context.User.GetOrCreateDMChannelAsync();
                                        await dmChannel.SendMessageAsync("I couldn't respond to your message because I lack the permissions to respond.\nHave you given me the correct permissions?");
                                        _logger.Log(Type.Info, $"Successfully send a PM to {context.User}");
                                    }
                                    catch
                                    {
                                        _logger.Log(Type.Info, $"Couldn't send PM to user");
                                    }
                                }
                                else
                                {
                                    _logger.Log(Type.Info, result.ErrorReason);
                                }
                                break;
                            case CommandError.BadArgCount:
                                await context.Channel.SendMessageAsync("``You have too few or too many arguments, check usage of command.``");
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        _logger.Log(Type.Info, $"\nCommand executed by {context.User}({context.User.Id}) in guild {context.Guild}({context.Guild.Id})\nTime taken to execute command is {stopwatch.ElapsedMilliseconds}ms");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }
        }
    }
}
