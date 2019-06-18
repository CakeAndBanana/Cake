using System.Threading.Tasks;
using Cake.Core.Discord.Services;
using Discord.Commands;

namespace Cake.Core.Discord.Modules
{
    [Name("osu!")]
    [Group("osu")]
    [Alias("o")]
    class OsuModule : CustomBaseModule
    {
        private readonly OsuService _service;

        public OsuModule(OsuService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("set")]
        [Summary("osu set")]
        [Alias("s")]
        [Remarks("Binds your osu account to your discord user.")]
        public async Task SetAccount([Remainder]string username)
        {
            await _service.SetAccount(username);
        }

        [Command("profile")]
        [Summary("osu profile (?user)")]
        [Alias("u", "p")]
        [Remarks("Gets profile of current user or given user.")]
        public async Task GetProfile([Remainder]string username)
        {
            int osuId = 0;

            if (username == null)
            {
                //get account
            }
            else
            {
                //get id here from username   
            }

            await _service.GetProfile(osuId);
        }
    }
}
