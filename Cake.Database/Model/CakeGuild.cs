using LinqToDB.Mapping;

namespace Cake.Database.Model
{
    [Table(Name = "CakeGuild")]
    public class CakeGuild
    {
        [PrimaryKey]
        public ulong Id { get; set; }
        [Column(Name = "Prefix"), Nullable]
        public string Prefix { get; set; }
        [Column(Name = "Restrict"), NotNull]
        public bool Restrict { get; set; }
        [Column(Name = "WelcomeId"), Nullable]
        public ulong? WelcomeId { get; set; }
        [Column(Name = "LeaveId"), Nullable]
        public ulong? LeaveId { get; set; }
        [Column(Name = "LevelUpId"), Nullable]
        public ulong? LevelUpId { get; set; }
    }
}
