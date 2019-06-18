using System.Threading.Tasks;

namespace Cake.Core.Discord.Services
{
    class OsuService : CustomBaseService
    {
        public async Task SetAccount(string username)
        {
            await SendMessageAsync($"``SetAccount (setaccounthere)``");
        }
        public async Task GetProfile(int id)
        {
            await SendMessageAsync($"``GetProfile (getaccounthere)``");
        }
    }
}
