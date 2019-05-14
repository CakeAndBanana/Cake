using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CakeBot.Core;
using CakeBot.Core.Helper;
using CakeBot.Core.Services;
using CakeBot.Helper;
using CakeBot.Helper.AntiSpam;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using CakeBot.Modules.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Tweetinvi.Core.Extensions;
using ConnectionState = Discord.ConnectionState;
using EmbedType = CakeBot.Helper.EmbedType;

namespace CakeBot
{
    public class Startup : IBotTasks
    {
        public static Random RNumb;
        public static string BotAvatarUrl { get; set; }
        
        public static string ConnectionStatus = "Disconnected";

        private static DiscordSocketClient _client;
        private CommandService _commandService;
        private IServiceProvider _services;

        private bool _running;
        private bool _retryConnecting = true;
        private const int RetryInterval = 1000;
        private const int RunningInterval = 1000;

        public async Task RunBotASync()
        {
            if (_client?.ConnectionState == ConnectionState.Connecting || _client?.ConnectionState == ConnectionState.Connected)
            {
                return;
            }

            BotSettings.SetupBotSettings();
            SetupBot();

            while (true)
            {
                try
                {
                    await _client.LoginAsync(TokenType.Bot, Config.BotKey);
                    await _client.StartAsync();
                    await _client.SetStatusAsync(UserStatus.Online);
                    await SetupCommands();
                    await RandomGen();
                    _running = true;

                    break;
                }
                catch (Exception)
                {
                    Logger.LogError("Failed to connect.");
                    if (_retryConnecting)
                    {
                        await Task.Delay(RetryInterval);
                        Config.GetConfigValues();
                    }
                }
            }

            while (_running)
            {
                await Task.Delay(RunningInterval);
            }

            if (_client.ConnectionState == ConnectionState.Connecting ||
                _client.ConnectionState == ConnectionState.Connected)
            {
                try
                {
                    _client.StopAsync().Wait();
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static DiscordSocketClient GetClient()
        {
            return _client;
        }

        public async Task RandomGen()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    RNumb = new Random();
                    var message = new LoggingMessage("Generated new Random", LoggingType.Debug);
                    Logger.LogMessage(message);
                    Thread.Sleep(100000);
                }
            });
        }

        public void GetBotAvatarUrl()
        {
            if (BotAvatarUrl.IsNullOrEmpty())
            {
                BotAvatarUrl = _client.CurrentUser.GetAvatarUrl();
            }
        }

        public async Task Cancel()
        {
            _retryConnecting = false;
            await Task.Delay(0);
        }

        public async Task Stop()
        {
            if (_running) _running = false;
            await Task.Delay(0);
        }

        private void SetupBot()
        {
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
            _services = SetupServices.ReturnProvider();
            _retryConnecting = true;
            _running = false;
        }

        public async Task HandleMessage(SocketMessage messageParams)
        {
            if (messageParams.Author.Id == 424606447867789312 && messageParams.Content.Contains("died at the age of"))
            {
                await DeathMessage.CreateDeathMessage(_client, messageParams);
                return;
            }

            if (messageParams is SocketUserMessage message)
            {
                var handleMessage = await CheckMessage(message, 0);

                while (handleMessage)
                {
                    var commandContext = new CommandContext(_client, message);

                    handleMessage = await SpamCheckMessage(commandContext);

                    var result = await _commandService.ExecuteAsync(commandContext, 1, _services);

                    Logger.LogInfo($"{commandContext.User.Id}({commandContext.User.Username}#{commandContext.User.Discriminator}) - {commandContext.Message.Content}",
                        result.ToString());
                    await HandleMessageResult(result, commandContext);

                    handleMessage = false; // Done with handling the message
                }
            }
        }

        internal async Task<bool> CheckMessage(SocketUserMessage message, int argPos)
        {
            if (!await UserQueries.CheckUser(message.Author.Id))
                Logger.LogInfo($"Added {message.Author.Id} to the database");
            var restrictStatus = await UserQueries.GetRestrictStatus(message.Author.Id);

            if (!message.Author.IsBot && !message.Author.IsWebhook || !restrictStatus)
            {
#if DEBUG
                if (message.HasCharPrefix(Convert.ToChar(Config.BotPrefix), ref argPos) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    await GiveXp(message, 4);
                    return true;
                }
#else
                    if (message.HasCharPrefix(Convert.ToChar(GuildQueries.FindPrefix(PrefixHelper.GuildIdFinder(_client, message.Channel.Id)).Result), ref argPos) ||
                        message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                    {
                        await GiveXp(message, 4);
                        return true;
                    }
#endif
                await GiveXp(message, 2);
            }
            return false;
        }

        internal async Task<bool> SpamCheckMessage(CommandContext context)
        {
            //TODO add spam filter issue#28
            try
            {
                var executeMessage = await Filter.HandleMessage(context);

                if (!executeMessage)
                {
                    var builder = new CakeEmbedBuilder(EmbedType.Debug);
                    builder.AddField("Filter Test", "We don't handle this message!");
                    await context.Channel.SendMessageAsync("", embed: builder.Build());
                }

                return executeMessage;
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await context.Channel.SendMessageAsync("", embed: embedError.Build());
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

            return false;
        }

        internal async Task HandleMessageResult(IResult result, CommandContext context)
        {
            try
            {
                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case CommandError.UnmetPrecondition:
                            if (result.ErrorReason.Contains("NSFW"))
                            {
                                throw new NoNsfwChannalException(context.User);
                            }
                            else
                            {
                                throw new NoPermissionException(context.User);
                            }
                        case CommandError.BadArgCount:
                            throw new BadCommandUsageException(ModuleInfoHelper.Get(), context);
                        default:
                            throw new CakeException(result.ErrorReason);
                    }
                }
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await context.Channel.SendMessageAsync("", embed: embedError.Build());
            }
        }

        private async Task GiveXp(SocketUserMessage message, int xp)
        {
            var levelUp = await UserQueries.GrantXp(message.Author.Id, xp);

            if (levelUp)
            {
                await message.Channel.SendMessageAsync(message.Author.Mention + "You leveled up!");
            }
        }

        public async Task SetupCommands()
        {
#if DEBUG
            if (GetClient().LoginState != LoginState.LoggedIn) return;
#endif

            _client.MessageReceived += HandleMessage;
            _client.Ready += Ready;
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Log += Log;
            _client.ReactionAdded += OnReactionAdded;
            _client.ReactionRemoved += OnReactionRemoved;
            
            var modules = await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            ModuleInfoHelper.Get().Init(modules);
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            foreach (var user in Global.usersToTrack)
            {
                if (user.Message.Id == reaction.MessageId && user.user.Id == reaction.User.Value.Id)
                {
                    if (reaction.Emote.Name == "▶")
                    {
                        if (user.CurrentBgIndex == user.BackgroundStrings.Length-1) return;
                        user.CurrentBgIndex++;
                        await user.Message.ModifyAsync(x => x.Content = $"{user.BackgroundStrings[user.CurrentBgIndex]}");
                    }
                    else if (reaction.Emote.Name == "◀")
                    {
                        if (user.CurrentBgIndex == 0) return;
                        user.CurrentBgIndex--;
                        await user.Message.ModifyAsync(x => x.Content = $"{user.BackgroundStrings[user.CurrentBgIndex]}");
                    }
                }
            }
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            foreach(var user in Global.usersToTrack)
            {
                if (user.Message.Id == reaction.MessageId && user.user.Id == reaction.User.Value.Id)
                {
                    if(reaction.Emote.Name == "▶")
                    {
                        if (user.CurrentBgIndex == user.BackgroundStrings.Length-1) return;
                        user.CurrentBgIndex++;
                        await user.Message.ModifyAsync(x => x.Content = $"{user.BackgroundStrings[user.CurrentBgIndex]}");
                    }
                    else if(reaction.Emote.Name == "✅")
                    {
                        user.currentBgId = user.Backgrounds[user.CurrentBgIndex].BackgroundId;
                        await channel.SendMessageAsync(user.user.Mention + await BackgroundQueries.BuyBackground(user.currentBgId, user.user.Id));
                        await user.Message.DeleteAsync();
                        Global.usersToTrack.Remove(user);
                    }
                    else if (reaction.Emote.Name == "◀")
                    {
                        if (user.CurrentBgIndex == 0) return;
                        user.CurrentBgIndex--;
                        await user.Message.ModifyAsync(x => x.Content = $"{user.BackgroundStrings[user.CurrentBgIndex]}");
                    }
                    else if(reaction.Emote.Name == "🚫")
                    {
                        await user.Message.DeleteAsync();
                        Global.usersToTrack.Remove(user);
                    }
                }
            }
        }
           
        private void SetConnectionStatus(string s, Exception arg = null)
        {
            ConnectionStatus = s;
            if (arg != null) Console.WriteLine(arg);
        }

        public async Task Ready()
        {
            GetBotAvatarUrl();
            await TwitterRealtime.ToggleStream();
            await _client.SetGameAsync("Baking a cake ^^");
            BotUtil.Init();
        }

        public async Task UserJoined(SocketGuildUser joinedUser)
        {
            var embed = new CakeEmbedBuilder()
                .AddField("Message", joinedUser.Mention + " joined the server!")
                .AddField("Creation Date", joinedUser.CreatedAt)
                .WithThumbnailUrl(joinedUser.GetAvatarUrl())
                .WithFooter(footer =>
                {
                    footer
                        .WithText(Config.BotName)
                        .WithIconUrl(BotAvatarUrl);
                })
                .WithTimestamp(DateTime.Now);

            var guild = joinedUser.Guild;
            var welcomeChannel = await GuildQueries.FindWelcome(guild.Id);
            SocketTextChannel channel = null;

            channel = welcomeChannel.GuildWelcome == null ? guild.DefaultChannel : guild.GetTextChannel((ulong)welcomeChannel.GuildWelcome);

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public async Task UserLeft(SocketGuildUser leftUser)
        {
            var embed = new CakeEmbedBuilder()
                .AddField("Message", leftUser.Mention + " left the server!")
                .AddField("Creation Date", leftUser.CreatedAt)
                .WithThumbnailUrl(leftUser.GetAvatarUrl())
                .WithFooter(footer =>
                {
                    footer
                        .WithText(Config.BotName)
                        .WithIconUrl(BotAvatarUrl);
                })
                .WithTimestamp(DateTime.Now);

            var guild = leftUser.Guild;
            var leaveChannel = await GuildQueries.FindLeave(guild.Id);
            SocketTextChannel channel = null;

            channel = leaveChannel.GuildLeave == null ? guild.DefaultChannel : guild.GetTextChannel((ulong)leaveChannel.GuildLeave);

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public Task Connected()
        {
            SetConnectionStatus("Connected");
            return Task.CompletedTask;
        }

        public Task Disconnected(Exception ex)
        {
            SetConnectionStatus("Disconnected", ex);
            return Task.CompletedTask;
        }

        public Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Debug:
                    Logger.LogDebug(message.ToString());
                    break;
                case LogSeverity.Info:
                    Logger.LogInfo(message.ToString());
                    break;
                case LogSeverity.Warning:
                    Logger.LogWarning(message.ToString());
                    break;
                case LogSeverity.Error:
                    Logger.LogError(message.ToString());
                    break;
                case LogSeverity.Critical:
                    Logger.LogError(message.ToString());
                    break;
                default:
                    Logger.LogInfo(message.ToString());
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
