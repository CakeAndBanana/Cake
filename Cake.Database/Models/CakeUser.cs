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
        [Column(Name = "OsuId"), Nullable]
        public int OsuId { get; set; }
        [Column(Name = "OsuMode"), Nullable]
        public int OsuMode { get; set; }
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

        public long GetCurrentExp() => TotalXp - (long)(125 * ((Level - 1) * 1.45));

        public long GetNextLevelExp() => (long)(125 * (Level * 1.45));
    }
}
