using LinqToDB.Mapping;
using System.Collections.Generic;

namespace Cake.Database.Models
{
    [Table(Schema = "dbo", Name = "CakeGuild")]
    public class CakeGuild
    {
        [PrimaryKey, NotNull]
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

        #region Associations

        /// <summary>
        /// FK_CakeChannel_CakeGuild_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "GuildId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<CakeChannel> CakeChannels { get; set; }

        #endregion
    }
}
