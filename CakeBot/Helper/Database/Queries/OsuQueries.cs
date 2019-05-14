using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Exceptions;

namespace CakeBot.Helper.Database.Queries
{
    public class OsuQueries
    {
        private static readonly CakeBotEntities Db = new CakeBotEntities();

        public static async Task<OsuUser> FindUser(ulong discordId)
        {
            var result = 
                await (from ou in Db.OsuUsers
                    where ou.UserId == (long) discordId
                       select ou).ToListAsync();
            return result.FirstOrDefault();
        }

        public static async Task CreateOrUpdateUser(ulong discordId, int osuId)
        {
            //Find user
            await UserQueries.CheckUser(discordId);

            //Find OsuUser
            var databaseProfile = await FindUser(discordId);
            if (databaseProfile == null)
            {
                var newUser = new OsuUser
                {
                    UserId = Convert.ToInt64(discordId),
                    OsuId = osuId,
                    OsuMode = 0
                };
                Db.OsuUsers.Add(newUser);
                await Db.SaveChangesAsync();
            }
            else
            {
                databaseProfile.OsuId = osuId;
                Db.Entry(databaseProfile).State = EntityState.Modified;
                await Db.SaveChangesAsync();
            }
        }

        public static async Task SetUserMode(ulong discordId, int mode)
        {
            var foundUser = await FindUser(discordId);

            if (foundUser == null)
            {
                throw new CakeException("User not found in the database, cannot set your osu mode");
            }

            foundUser.OsuMode = mode;
            await Db.SaveChangesAsync();
        }

        public static async Task<OsuUser> ChangeMode(ulong discordid, int mode)
        {
            //Find user
            await UserQueries.CheckUser(discordid);

            //Find OsuUser
            var databaseProfile = await FindUser(discordid);
            if (databaseProfile == null)
            {
                throw new CakeException("You have to setup your osu account with >osu set (username)");
            }

            databaseProfile.OsuMode = mode;

            Db.Entry(databaseProfile).State = EntityState.Modified;
            await Db.SaveChangesAsync();

            return databaseProfile;
        }
    }
}
