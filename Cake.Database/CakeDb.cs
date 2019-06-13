using Cake.Database.Model;
using LinqToDB;

namespace Cake.Database
{
    public partial class CakeDb : LinqToDB.Data.DataConnection
    {
        public CakeDb() : base("CakeContext") { }
        public ITable<CakeGuild> CakeGuilds => GetTable<CakeGuild>();
    }
}
