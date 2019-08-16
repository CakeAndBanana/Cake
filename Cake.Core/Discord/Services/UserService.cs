using Cake.Database.Queries;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Services
{
    public class UserService : CustomBaseService
    {
        public async Task GetUserExperience()
        {
            var user = await UserQueries.FindOrCreateUser(Module.Context.User.Id);

            await SendMessageAsync($"``{user.GetCurrentExp()} xp``");
        }
    }
}
