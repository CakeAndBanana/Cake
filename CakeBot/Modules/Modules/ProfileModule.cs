using System.Threading.Tasks;
using CakeBot.Modules.Services;
using Discord.Commands;
using Discord.WebSocket;

namespace CakeBot.Modules.Modules
{
    [Alias("pf")]
    [Group("profile")]
    [Name("Profile")]
    public class ProfileModule : CustomBaseModule
    {
        
        private readonly ProfileService _service;

        public ProfileModule(ProfileService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command]
        [Name("")]
        [Remarks(">profile (?user)")]
        [Summary("Shows profile of own or user sent.")]
        public async Task GetProfile(SocketUser userTest = null)
        {
            await _service.GetProfile(userTest);
        }

        [Command("color")]
        [Remarks(">profile color (hex)")]
        [Summary("Changes color of the text on your profile card. (level 15+)")]
        public async Task SetColor(string Hex)
        {
            await _service.ChangeColor(Hex);
        }

        [Command("set")]
        [Summary(">profile set (background number)")]
        [Remarks("Sets/replaces your background if user owns it.")]
        public async Task SetBackground(int bgId)
        {
            await _service.SetBackground(bgId, Context.User.Id);
        }
        [Command("buy")]
        [Summary(">profile buy (background number)")]
        [Remarks("Buys a background instantly")]
        public async Task BuyBackground(int bgId)
        {
            await _service.BuyBackground(bgId, Context.User.Id);
        }
        [Command("list")]
        [Summary(">profile list")]
        [Remarks("Lists all purchasable backgrounds")]
        public async Task ListBackground(string Category = "")
        {
            await _service.ListBackgrounds(Context.User.Id, Category);
        }
    }
}
