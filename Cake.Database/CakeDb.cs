using Cake.Database.Models;
using LinqToDB;

namespace Cake.Database
{
    public partial class CakeDb : LinqToDB.Data.DataConnection
    {
        public CakeDb() : base("CakeContext") { }
        public ITable<CakeChannel> CakeChannels => GetTable<CakeChannel>();
        public ITable<CakeGuild> CakeGuilds => GetTable<CakeGuild>();
        public ITable<CakeUser> CakeUsers => GetTable<CakeUser>();

    }
}
