using System.Threading.Tasks;
using CakeBot.Modules.Services;
using Discord;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Group("admin")]
    [Name("Admin")]
    [RequireContext(ContextType.Guild)]
    public class AdminModule : CustomBaseModule
    {
        private readonly AdminService _service;

        public AdminModule(AdminService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("ban")]
        [Remarks(">admin ban (username) (reason)")]
        [Summary("Bans the user from the discord server.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, [Remainder] string reason = null)
        {
            await _service.BanUser(Context.Guild, user, reason);
        }

        [Command("unban")]
        [Remarks(">admin unban (username)")]
        [Summary("Unbans a user from the discord server.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task UnbanUser(string user)
        {
            await _service.UnbanUser(Context.Guild, user);
        }

        [Command("kick")]
        [Remarks(">admin kick (username) (reason)")]
        [Summary("Kicks a person from the discord server.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, [Remainder] string reason = null)
        {
            await _service.KickUser(Context.Guild, user, reason);
        }
    }
}