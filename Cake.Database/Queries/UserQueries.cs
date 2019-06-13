using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Queries
{
    public class UserGueries
    {
        public static async Task<List<CakeUser>> GetAllUsers()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeUser>().ToListAsync();
            }
        }

        private static async Task<CakeUser> GetUser(ulong userId)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cu in db.CakeUsers
                                    where cu.Id == userId
                                    select cu).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        public static async Task UpdateUser(CakeUser user)
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

        public static async Task<CakeUser> FindOrCreateUser(ulong userId) => await GetUser(userId) ?? await CreateUser(userId);
    }
}
