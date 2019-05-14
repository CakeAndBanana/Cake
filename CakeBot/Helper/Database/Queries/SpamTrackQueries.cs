using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;

namespace CakeBot.Helper.Database.Queries
{
    public class SpamTrackQueries
    {
        public static async Task<SpamTrack> FindUserTrack(ulong userId, ulong guildId)
        {
            using (var db = new CakeBotEntities())
            {
                var result =
                    await (from t in db.SpamTracks
                        where t.GuildId == (long) guildId && t.UserId == (long) userId
                        select t).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        public static async Task<SpamTrack> GetUserTrack(ulong userId, ulong guildId)
        {
            var track = await FindUserTrack(userId, guildId);

            if (track == null)
            {
                var newTrack = new SpamTrack
                {
                    UserId = (long) userId,
                    GuildId = (long) guildId,
                    LastMessage = 0,
                    Pressure = 0f
                };

                using (var db = new CakeBotEntities())
                {
                    db.SpamTracks.Add(newTrack);
                    await db.SaveChangesAsync();
                }

                track = newTrack;
            }

            return track;
        }

        public static async Task UpdateUserTrack(SpamTrack currentTrack)
        {
            using (var db = new CakeBotEntities())
            {
                db.Entry(currentTrack).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
