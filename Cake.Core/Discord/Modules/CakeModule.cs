using System.Threading.Tasks;
using Cake.Core.Discord.Attributes;
using Cake.Core.Discord.Services;
using Discord.Commands;
using Discord.WebSocket;

namespace Cake.Core.Discord.Modules
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

        [Command("status")]
        [Summary("cake status")]
        [Alias("s")]
        [Remarks("Returns status of Cake.")]
        public async Task ReturnStatus()
        {
            await _service.GetStatus();
        }

        [Command("prefix")]
        [Summary("cake prefix (new prefix?)")]
        [Alias("p")]
        [Remarks("Returns or set the prefix of Cake.")]
        public async Task ReturnPrefix(string newPrefix = null)
        {
            if (newPrefix == null)
            {
                await _service.GetPrefix();
            }
            else
            {
;               await _service.SetPrefix(newPrefix);
            }
        }

        [Command("welcome")]
        [Summary("cake welcome")]
        [Alias("p")]
        [Remarks("Sets current channel as welcome message channel for Cake.")]
        public async Task SetWelcome()
        {
            await _service.SetWelcome(Context.Channel.Id);
        }

        [Command("leave")]
        [Summary("cake leave")]
        [Alias("p")]
        [Remarks("Sets current channel as leave message channel for Cake.")]
        public async Task SetLeave()
        {
            await _service.SetLeave(Context.Channel.Id);
        }

        [Command("restrict")]
        [Summary("cake restrict (user/guild) (id)")]
        [Alias("r")]
        [Remarks("Restricts user or guild from using Cake.")]
        [RequireBotAdmin]
        public async Task SetRestrict(string result, ulong id)
        {
            switch (result)
            {
                case "user":
                    await _service.RestrictUser(id);
                    break;
                case "guild":
                    await _service.RestrictGuild(id);
                    break;
                default:
                    await Context.Channel.SendMessageAsync("``Use guild or user as param``");
                    break;
            }
        }

        [Command("setadmin")]
        [Summary("cake setadmin (id/mention)")]
        [Alias("sa")]
        [Remarks("Makes an user a Bot Admin.")]
        [RequireBotAdmin]
        public async Task SetAdminId(ulong id)
        {
            await _service.SetAdmin(id);
        }

        [Command("setadmin")]
        [Alias("sa")]
        [RequireBotAdmin]
        public async Task SetAdminMention(SocketGuildUser user)
        {
            await _service.SetAdmin(user.Id);
        }
    }
}
