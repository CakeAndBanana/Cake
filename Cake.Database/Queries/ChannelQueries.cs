using Cake.Database.Models;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Cake.Database.Queries
{
    public class ChannelQueries
    {
        public static async Task<List<CakeChannel>> GetAll()
        {
            using (var db = new CakeDb())
            {
                return await db.GetTable<CakeChannel>().ToListAsync();
            }
        }

        private static async Task<CakeChannel> Get(ulong id)
        {
            using (var db = new CakeDb())
            {
                var result = await (from cc in db.CakeChannels
                                    where cc.Id == id
                                    select cc).ToListAsync();
                return result.FirstOrDefault();
            }
        }

        public static async Task Update(CakeChannel record)
        {
            using (var db = new CakeDb())
            {
                await db.UpdateAsync(record);
            }
        }
    }
}
