using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Modules.BF4;
using CakeBot.Modules.Services;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Name("Games")]
    public class FunModule : CustomBaseModule
    {
        private readonly GamesService _service;

        private CakeEntities _db = new CakeEntities();

        public FunModule(GamesService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("coinflip")]
        [Remarks(">coinflip")]
        [Summary("Flips a coin")]
        [Alias("cf")]
        public async Task CoinFlip()
        {
            await _service.CoinFlip();
        }

        [Command("bf4")]
        [Summary(">bf4 (platform) (name)")]
        [Remarks("Sends your Battlefield 4 Stats")]
        public async Task BattleFieldStats(string platform, string name)
        {
            await _service.BattlefieldStats(platform, name);
        }

        [Command("deaths")]
        [Summary(">deaths")]
        [Remarks("Sends dead children in IdleRPG")]
        public async Task SendDeaths()
        {
            await _service.SendDeaths(_db);
        }
    }
}