using LinqToDB.Mapping;

namespace Cake.Database.Models
{
    [Table(Name = "CakeUser")]
    public class CakeUser
    {
        [PrimaryKey]
        public ulong Id { get; set; }
        [Column(Name = "Restrict"), NotNull]
        public bool Restrict { get; set; }
        [Column(Name = "OsuId"), NotNull]
        public int OsuId { get; set; }
        [Column(Name = "Xp"), NotNull]
        public int Xp { get; set; }
        [Column(Name = "TotalXp"), NotNull]
        public long TotalXp { get; set; }
        [Column(Name = "Level"), NotNull]
        public int Level { get; set; }
        [Column(Name = "Admin"), NotNull]
        public bool Admin { get; set; }
        [Column(Name = "Money"), NotNull]
        public long Money { get; set; }
        [Column(Name = "BackgroundId"), Nullable]
        public int? BackgroundId { get; set; }
    }
}
