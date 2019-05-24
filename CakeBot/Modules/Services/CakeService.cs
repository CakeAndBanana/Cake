using System;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Core;
using CakeBot.Helper;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using Discord;
using Discord.Commands;
using EmbedType = CakeBot.Helper.EmbedType;

namespace CakeBot.Modules.Services
{
    public class CakeService : CustomBaseService
    {
        public async Task SetPrefix(string newPrefix)
        {
            var embedPrefix = new CakeEmbedBuilder();

            if (newPrefix == "")
            {
                embedPrefix.AddField("Prefix", $"The current prefix is '{GuildQueries.FindPrefix(Module.Context.Guild.Id)}'");
            }
            else
            {
                await GuildQueries.SetPrefix(Module.Context.Guild.Id, newPrefix);
                embedPrefix.AddField("Prefix", $"Prefix has changed to '{newPrefix}'");
            }

            await SendEmbedAsync(embedPrefix);
        }

        public async Task SetWelcome()
        {
            var embedWelcome = new CakeEmbedBuilder();

            await GuildQueries.SetWelcome(Module.Context.Guild.Id, Module.Context.Channel.Id);
            embedWelcome.AddField("Welcome Channel", $"The welcome messages will be send to this channel.");

            await SendEmbedAsync(embedWelcome);
        }

        public async Task SetLeave()
        {
            var embedLeave = new CakeEmbedBuilder();

            await GuildQueries.SetLeave(Module.Context.Guild.Id, Module.Context.Channel.Id);
            embedLeave.AddField("Leave Channel", $"The leave messages will be send to this channel.");

            await SendEmbedAsync(embedLeave);
        }

        public async Task SetName([Remainder] string newName)
        {
            await BotUtil.SetBotName(newName);
        }

        public async Task GetPing()
        {
            await SendMessageAsync($"Pong! `Took {Startup.GetClient().Latency}ms`");
        }

        public async Task RestrictUser(ulong userid, int? value)
        {
            switch (value)
            {
                case null:
                {
                    var status = await UserQueries.GetRestrictStatus(userid);
                    await SendMessageAsync($"`User({userid}) restricted status is {status}`");
                    break;
                }
                case 1:
                    await UserQueries.SetRestrictStatus(userid, true);
                    await SendMessageAsync($"`User({userid}) is restricted`");
                    break;
                case 0:
                    await UserQueries.SetRestrictStatus(userid, false);
                    await SendMessageAsync($"`User({userid}) has been unrestricted`");
                    break;
            }
        }

        public async Task GetInfo()
        {
            var embedInfo = new CakeEmbedBuilder()
                .WithTitle("CakeBot")
                .WithDescription($"issue#49") as CakeEmbedBuilder;
            embedInfo.EmbedType = EmbedType.Info;

            await SendEmbedAsync(embedInfo);
        }

        public async Task GetServer()
        {
            var regionId = Module.Context.Guild.VoiceRegionId;
            var regionList = Module.Context.Guild.GetVoiceRegionsAsync().Result;
            var featureList = Module.Context.Guild.Features;
            var userList = Module.Context.Guild.GetUsersAsync().Result;

            int online = 0, busy = 0, offline = 0, bot = 0;

            var regionName = "";
            var features = featureList.Aggregate("", (current, feature) => current + $"`{feature}`', ");

            foreach (var region in regionList)
            {
                if (region.Id == regionId) regionName = region.Name;
            }

            foreach (var user in userList)
            {
                if (!user.IsBot)
                {
                    switch (user.Status)
                    {
                        case UserStatus.Online:
                            online++;
                            break;
                        case UserStatus.AFK:
                        case UserStatus.DoNotDisturb:
                        case UserStatus.Idle:
                            busy++;
                            break;
                        case UserStatus.Offline:
                        case UserStatus.Invisible:
                            offline++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    bot++;
                }
            }

            var embedServer = (CakeEmbedBuilder) new CakeEmbedBuilder()
                .WithAuthor(author =>
                {
                    author
                        .WithName(Module.Context.Guild.Name)
                        .WithIconUrl(Module.Context.Guild.IconUrl);
                })
                .WithThumbnailUrl(Module.Context.Guild.IconUrl)
                .WithDescription($"**Owner:** `{Module.Context.Guild.GetOwnerAsync().Result.Username}#{Module.Context.Guild.GetOwnerAsync().Result.Discriminator}`\n" +
                                 $"**Created:** {TimeFormat.ToShortTimeSpan(TimeSpan.FromTicks(DateTime.UtcNow.Date.Ticks - Module.Context.Guild.CreatedAt.Date.Ticks))} ago `{Module.Context.Guild.CreatedAt.Date.ToShortDateString()}`\n" +
                                 $"**Region:** `{regionName}`\n" +
                                 $"**Channels:** {Module.Context.Guild.GetChannelsAsync().Result.Count}\n" +
                                 $"**Features:** {features}\n\n" +
                                 $"**>Members:** {userList.Count} ({bot} bots)\n" +
                                 $"<:online:547392159914131466> - {online}\n" +
                                 $"<:busy:547392159964332052> - {busy}\n" +
                                 $"<:offline:547392159884509185> - {offline}\n");


            await SendEmbedAsync(embedServer);
        }

        public async Task GetUser(IGuildUser user)
        {
            var status = "";

            switch (user.Status)
            {
                case UserStatus.Online:
                    status = "<:online:547392159914131466>";
                    break;
                case UserStatus.AFK:
                case UserStatus.DoNotDisturb:
                case UserStatus.Idle:
                    status = "<:busy:547392159964332052>";
                    break;
                case UserStatus.Offline:
                case UserStatus.Invisible:
                    status = "<:offline:547392159884509185>";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var embedUser = new CakeEmbedBuilder()
                .WithAuthor(author =>
                {
                    author
                        .WithName($"{user.Username}")
                        .WithIconUrl(user.GetAvatarUrl());
                })
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithDescription(
                    $"**User:** {status}{user.Username}#{user.Discriminator}\n" +
                    $"**Nickname:** {user.Nickname}\n" +
                    $"**Created:** {user.CreatedAt.Date.ToShortDateString()}\n" +
                    $"**Joined:** {user.JoinedAt?.Date.ToShortDateString()}\n" +
                    $"**Bot:** {user.IsBot}")
                .WithFooter($"Guild {Module.Context.Guild.Name}") as CakeEmbedBuilder;

            await SendEmbedAsync(embedUser);
        }

        public async Task SetRole(IGuildUser user, string roleName)
        {
            var roles = Module.Context.Guild.Roles;
            foreach (var role in roles)
            {
                if (role.Name != roleName) continue;

                await user.AddRoleAsync(role);
                await SendMessageAsync($"Added to {role.Name} role");
                break;
            }
        }

        public async Task BugReport(string message)
        {
            try
            {
                var report = await BugQueries.CreateNewBugReport(message, Module.Context.User.Id, Module.Context.Guild.Id);
                var embed = new CakeEmbedBuilder()
                    .WithTitle($"Bug report | id {report.Id}")
                    .WithDescription($"Message: {report.Message}\n")
                    .WithFooter(
                    $"Send by {Module.Context.User}({Module.Context.User.Id}) in {Module.Context.Guild}({Module.Context.Guild.Id})") as CakeEmbedBuilder;
                await SendEmbedAsync(embed);
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
        }

        public async Task ListGuilds()
        {
            var count = 1;
            var embedList = new CakeEmbedBuilder();

            foreach (var clientGuild in Startup.GetClient().Guilds)
            {
                embedList.AddField($" Guild {count}", clientGuild.ToString());
                count++;
            }

            await SendEmbedAsync(embedList);
        }
    }
}
