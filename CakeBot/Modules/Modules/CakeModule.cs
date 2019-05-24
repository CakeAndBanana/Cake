using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Modules.Services;
using Discord;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Group("cake")]
    [Alias("c")]
    [Name("Cake")]
    public class CakeModule : CustomBaseModule
    {
        private readonly CakeService _service;

        public CakeModule(CakeService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("prefix")]
        [Summary(">cake prefix (prefix)")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        [Remarks("Sets the prefix of the bot.")]
        public async Task SetPrefix([Remainder] string newPrefix)
        {
            await _service.SetPrefix(newPrefix);
        }

        [Command("welcome")]
        [Summary(">cake welcome")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        [Alias("w")]
        [Remarks("Sets the welcome messages channel of this guild.")]
        public async Task SetWelcome()
        {
            await _service.SetWelcome();
        }

        [Command("leave")]
        [Summary(">cake leave")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        [Alias("l")]
        [Remarks("Sets the leave message channel of this guild.")]
        public async Task SetLeave()
        {
            await _service.SetLeave();
        }

        [Command("setrole")]
        [Summary(">cake setrole (user) (rolename)")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Remarks("Sets the role of someone to the given role name.")]
        public async Task SetRoleTask(IGuildUser user, string roleName)
        {
            await _service.SetRole(user, roleName);
        }

        [Command("name")]
        [Summary(">cake name")]
        [RequireDeveloper]
        [Remarks("Sets the nickname of the bot.")]
        public async Task SetName([Remainder] string newName)
        {
            await _service.SetName(newName);
        }

        [Command("restrict")]
        [Remarks(">cake restrict (userid) *(0/1)")]
        [Summary("Restrict users for using the bot. Returns status when true/false is empty")]
        [RequireDeveloper]
        public async Task RestrictUser(ulong userid, int? status)
        {
            await _service.RestrictUser(userid, status);
        }

        [Command("ping")]
        [Summary(">cake ping")]
        [Remarks("Returns a ping to you")]
        public async Task GetPing()
        {
            await _service.GetPing();
        }

        [Command("server")]
        [Summary(">cake server")]
        [Remarks("Gives server info")]
        [RequireContext(ContextType.Guild)]
        public async Task GetServer()
        {
            await _service.GetServer();
        }

        [Command("user")]
        [Summary(">cake user (user)")]
        [Remarks("Gets information about user")]
        [RequireContext(ContextType.Guild)]
        public async Task GetUser(IGuildUser user)
        {
            await _service.GetUser(user);
        }

        [Command("info")]
        [Summary(">cake info ")]
        [Remarks("Gets info of bot")]
        public async Task GetInfo()
        {
            await _service.GetInfo();
        }

        [Command("bug")]
        [Summary(">cake bug (message)")]
        [Remarks("Reports bug to developers")]
        public async Task SetBug([Remainder] string message)
        {
            await _service.BugReport(message);
        }

        [Command("list")]
        [Remarks(">cake list")]
        [Summary("List of guilds that have CakeBot.")]
        public async Task ListGuilds()
        {
            await _service.ListGuilds();
        }
    }
}