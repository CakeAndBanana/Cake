using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Modules.Services;
using Discord;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Group("eco")]
    [Name("Economy")]
    public class EconomyModule : CustomBaseModule
    {
        private readonly EconomyService _service;

        public EconomyModule(EconomyService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("add")]
        [RequireDeveloper]
        [Summary(">eco add (user) (amount)")]
        [Remarks("Get amount of money.")]
        public async Task AddMoney(IGuildUser user, long amount)
        {
            await _service.AddMoney(user, amount);
        }

        [Command("remove")]
        [RequireDeveloper]
        [Summary(">eco remove (user) (amount)")]
        [Remarks("Removes amount of money.")]
        public async Task RemoveMoney(IGuildUser user, long amount)
        {
            await _service.RemoveMoney(user, amount);
        }

        [Command("balance")]
        [Summary(">eco balance")]
        [Remarks("Get your balance.")]
        public async Task GetBalance()
        {
            await _service.GetBalance();
        }

        [Command("daily")]
        [Summary(">eco daily")]
        [Remarks("Get your daily amount of money.")]
        public async Task GetDaily()
        {
            await _service.Daily();
        }

        [Command("rank")]
        [Remarks(">rank")]
        [Summary("Shows your profile rank of all users")]
        public async Task ShowRank()
        {
            await _service.GetRank();
        }

        [Command("leaderboard")]
        [Alias("top")]
        [Remarks(">leaderboard")]
        [Summary("Leaderboard in guild")]
        public async Task Leaderboard()
        {
            await _service.Leaderboard();
        }

        [Command("correctusers")]
        [RequireDeveloper]
        public async Task Correctify()
        {
            await _service.Correctify();
        }
    }
}

