using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Helper.Database.Model;
using CakeBot.Modules.Services;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Alias("f")]
    [Group("fish")]
    [Name("Fish")]
    public class FishModule : CustomBaseModule
    {
        private readonly FishService _service;

        public FishModule(FishService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        private CakeEntities _db = new CakeEntities();


        [Command]
        [Summary(">fish")]
        [Remarks("Fishes a fish")]
        public async Task Fish()
        {
            await _service.Fish(Startup.RNumb, _db);
        }

        [Command("sell")]
        [Summary(">fish sell <id> || <group>")]
        [Remarks("Sells selected group or Id of fish")]
        public async Task SellFishes(string input)
        {
            await _service.SellFishes(input, _db);
        }

        [Command("stats")]
        [Summary(">fish stats")]
        [Remarks("Shows Statistics of Fishy")]
        public async Task getFishStats()
        {
            await _service.GetFishStats(_db);
        }

        [Alias("inv")]
        [Command("inventory")]
        [Summary(">fish inventory")]
        [Remarks("Shows your Inventory of Fishy")]
        public async Task getFishInv()
        {
            await _service.getFishInv(_db);
        }

        [Command("add")]
        [RequireDeveloper]
        [Summary(">fish add (parms)")]
        [Remarks("Adds a custom fish to the database")]
        public async Task AddFish(string name, long worth,int rarity, string emoji, [Remainder] string message)
        {
            await _service.AddFish(name, worth, rarity, emoji, message);
        }

        [Command("list")]
        [Summary(">fish list")]
        [Remarks("Returns List of every fish")]
        public async Task GetFishes()
        {
            await _service.GetFishes(_db);
        }
    }
}
