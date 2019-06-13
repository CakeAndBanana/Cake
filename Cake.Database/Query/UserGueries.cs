using Cake.Database.Model;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Database.Query
{
    public class UserGueries
    {
        public async static Task<List<CakeUser>> GetUsers()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeUser>().ToListAsync();
            }
        }

        private async static Task<CakeUser> GetUser(ulong userId)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cu in db.CakeUsers
                                    where cu.Id == userId
                                    select cu).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        private async static Task<CakeUser> CreateUser(ulong userId)
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

        public async static Task<CakeUser> FindOrCreateUser(ulong userId)
        {
            var user = await GetUser(userId);
            if (user == null)
            {
                user = await CreateUser(userId);
            }
            return user;
        }
    }
}
