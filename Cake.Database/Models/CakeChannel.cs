using LinqToDB.Mapping;

namespace Cake.Database.Models
{
    [Table(Name = "CakeGuild")]
    public class CakeChannel
    {
        [PrimaryKey]
        public ulong Id { get; set; }
        [Column(Name = "GuildId"), NotNull]
        public ulong GuildId { get; set; }
        [Column(Name = "LastOsuMapId"), NotNull]
        public int OsuMapId { get; set; }
        [Column(Name = "Restrict"), NotNull]
        public bool Restrict { get; set; }
    }
}
