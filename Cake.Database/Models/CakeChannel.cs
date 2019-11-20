using LinqToDB.Mapping;

namespace Cake.Database.Models
{
    [Table(Schema = "dbo", Name = "CakeChannel")]
    public class CakeChannel
    {
        [PrimaryKey, NotNull]
        public ulong Id { get; set; }
        [Column(Name = "GuildId"), NotNull]
        public ulong GuildId { get; set; }
        [Column(Name = "LastOsuMapId"), NotNull]
        public int OsuMapId { get; set; }
        [Column(Name = "Restrict"), NotNull]
        public bool Restrict { get; set; }

        #region Associations
        /// <summary>
        /// FK_CakeChannel_CakeGuild
        /// </summary>
        [Association(ThisKey = "GuildId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "FK_CakeChannel_CakeGuild", BackReferenceName = "CakeChannels")]
        public CakeGuild Guild { get; set; }
        #endregion
    }
}
