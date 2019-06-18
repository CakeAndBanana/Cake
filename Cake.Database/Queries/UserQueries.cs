using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class UserQueries
    {
        protected UserQueries()
        {
        }

        public static async Task<List<CakeUser>> GetAll()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeUser>().ToListAsync();
            }
        }

        private static async Task<CakeUser> Get(ulong id)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cu in db.CakeUsers
                                    where cu.Id == id
                                    select cu).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        public static async Task Update(CakeUser record)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(record);
            }
        }

        private static async Task<CakeUser> CreateUser(ulong userId)
        {
            using (var db = new CakeDb())
            {
                var newUser = new CakeUser
                {
                    Id = userId,
                    TotalXp = 0,
                    Xp = 0,
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

        public static async Task<CakeUser> FindOrCreateUser(ulong userId) => await Get(userId).ConfigureAwait(false) ?? await CreateUser(userId).ConfigureAwait(false);
    }
}
