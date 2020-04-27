using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class UserQueries
    {
        public static List<CakeUser> GetAll()
        {
            return new DataContext().GetTable<CakeUser>().ToList();
        }
        private static CakeUser Get(ulong id) => GetAll().Where(cu => cu.Id == id).FirstOrDefault();

        public static async Task Update(CakeUser user)
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

        public static async Task<CakeUser> FindOrCreateUser(ulong userId) => Get(userId) ?? await CreateUser(userId).ConfigureAwait(false);
    }
}
