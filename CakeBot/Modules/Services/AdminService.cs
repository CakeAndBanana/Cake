using System;
using System.Threading.Tasks;
using CakeBot.Core.Services;
using CakeBot.Helper;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using Discord;
using Discord.WebSocket;

namespace CakeBot.Modules.Services
{
    public class AdminService : CustomBaseService
    {
        public async Task BanUser(IGuild contextGuild, IGuildUser user, string reason)
        {
            try
            {
                await Module.Context.Message.DeleteAsync();
                var banner = Module.Context.User as SocketGuildUser;

                if (user.Equals(banner))
                {
                    throw new NoSelfCommandException(user);
                }

                var dmChannel = await user.GetOrCreateDMChannelAsync();
                var embedBan = new CakeEmbedBuilder();
                var embedBanDm = new CakeEmbedBuilder();

                embedBanDm.AddField("Ban", "You were banned by '" + user.Username + "' from '" + Module.Context.Guild.Name + "'");
                embedBan.AddField("Ban", user.Username + " got banned from the server.");

                if (reason != null && reason.Length > 3)
                {
                    embedBanDm.AddField("Reason", reason);
                    embedBan.AddField("Reason", reason);
                }

                await dmChannel.SendMessageAsync("", false, embedBanDm.Build());

                await contextGuild.AddBanAsync(user, 0, reason);
                await SendEmbedAsync(embedBan);
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task UnbanUser(IGuild contextGuild, string bannedUsername)
        {
            try
            {
                await Module.Context.Message.DeleteAsync();

                IUser bannedUser = null;

                var isBanned = false;
                var bans = await Module.Context.Guild.GetBansAsync();

                foreach (var x in bans)
                {
                    if (x.User.Username == bannedUsername)
                    {
                        isBanned = true;
                        bannedUser = x.User;
                    }
                }

                if (!isBanned)
                {
                    throw new CakeException("User wasn't banned on this server.");
                }

                await UnbanInternal(bannedUser).ConfigureAwait(false);
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await Module.Context.Channel.SendMessageAsync("", embed: embedError.Build());
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private async Task UnbanInternal(IUser user)
        {
            var embedUnban = new CakeEmbedBuilder()
                .WithTitle("Unbanned")
                .WithDescription(user.Username + " got unbanned from this server") as CakeEmbedBuilder;

            await Module.Context.Guild.RemoveBanAsync(user).ConfigureAwait(false);

            await SendEmbedAsync(embedUnban);
        }

        public async Task KickUser(IGuild contextGuild, IGuildUser user, string reason)
        {
            try
            {
                await Module.Context.Message.DeleteAsync();
                var kicker = Module.Context.User as SocketGuildUser;

                if (user.Equals(kicker))
                {
                    throw new NoSelfCommandException(user);
                }

                if (kicker != null && kicker.GuildPermissions.KickMembers)
                {
                    var embedKick = new CakeEmbedBuilder();
                    embedKick.AddField("Kick", user.Username + " got kicked from the server.");

                    var dmChannel = await user.GetOrCreateDMChannelAsync();
                    await dmChannel.SendMessageAsync("You were kicked by '" + user.Nickname + "' from '" + Module.Context.Guild.Name + "'");

                    if (reason != null)
                    {
                        embedKick.AddField("Reason", reason);
                        await dmChannel.SendMessageAsync("Reason: " + reason);
                    }

                    await user.KickAsync();
                    await SendEmbedAsync(embedKick);
                }
                else
                {
                    throw new NoPermissionException(kicker);
                }
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}
