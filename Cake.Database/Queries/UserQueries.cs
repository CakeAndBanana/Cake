using Cake.Database.Models;
using LinqToDB;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class UserQueries
    {
        public static IQueryable<CakeUser> GetAll()
        {
            using (var db = new CakeDb())
            {
                return db.GetTable<CakeUser>().AsQueryable();
            }
        }

        public static async void Update(CakeUser user)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(user);
            }
        }

        private static async Task<CakeUser> CreateUser(ulong userId)
        {
            using (var db = new CakeDb())
            {
                CakeUser newUser = new CakeUser
                {
                    Id = userId,
                    TotalXp = 0,
                    Admin = false,
                    Restrict = false,
                    OsuId = 0,
                    Money = 0,
                    Level = 1
                };

                await db.InsertAsync(newUser);
                return newUser;
            }
        }
        private static async Task<CakeUser> Get(ulong id) => await GetAll().Where(cu => cu.Id == id).FirstOrDefaultAsync();
        public static async Task<CakeUser> FindOrCreateUser(ulong userId) => await Get(userId).ConfigureAwait(false) ?? await CreateUser(userId).ConfigureAwait(false);
    }
}
