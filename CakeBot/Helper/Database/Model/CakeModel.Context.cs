﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CakeBot.Helper.Database.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using CakeBot.Core;

    public partial class CakeEntities : DbContext
    {
        public CakeEntities()
            : base(((EntityConnectionStringBuilder)new EntityConnectionStringBuilder()
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = Config.ConnectionString,
                Metadata =
                    @"res://*/Helper.Database.Model.DbModel.csdl|res://*/Helper.Database.Model.DbModel.ssdl|res://*/Helper.Database.Model.DbModel.msl"
            }).ToString())
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BugReport> BugReports { get; set; }
        public virtual DbSet<DeadChildren> DeadChildrens { get; set; }
        public virtual DbSet<DiscordChannel> DiscordChannels { get; set; }
        public virtual DbSet<DiscordGuild> DiscordGuilds { get; set; }
        public virtual DbSet<FishType> FishTypes { get; set; }
        public virtual DbSet<OsuUser> OsuUsers { get; set; }
        public virtual DbSet<ProfileBackground> ProfileBackgrounds { get; set; }
        public virtual DbSet<SpamTrack> SpamTracks { get; set; }
        public virtual DbSet<TwitterPost> TwitterPosts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserBackground> UserBackgrounds { get; set; }
        public virtual DbSet<UserEconomy> UserEconomies { get; set; }
        public virtual DbSet<UserFishInventory> UserFishInventories { get; set; }
        public virtual DbSet<UserFishLog> UserFishLogs { get; set; }
        public virtual DbSet<UserSlotLog> UserSlotLogs { get; set; }
        public virtual DbSet<BotInfo> BotInfoes { get; set; }
        public virtual DbSet<OsuTrackUser> OsuTrackUsers { get; set; }
    }
}
