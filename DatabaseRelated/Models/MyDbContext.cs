using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using core.DatabaseRelated.Models.AirlineRelated;

namespace core.DatabaseRelated.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<PendingAirlineApp> pendingApplications { get; set; }
        public DbSet<User> players { get; set; }
        public DbSet<Airline> airlines { get; set; }
        public DbSet<Ban> bans { get; set; }
        public DbSet<Ranklist> ranks { get; set; }
        public DbSet<AirlineAnnouncement> airlineAnnouncements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.playerID);
            modelBuilder.Entity<User>().HasOne(x => x.airline);

            modelBuilder.Entity<Airline>().HasKey(x => x.airlineID);

            modelBuilder.Entity<Ban>().HasKey(x => x.banID);
            modelBuilder.Entity<Ban>().HasOne(x => x.player);
            modelBuilder.Entity<Ban>().HasOne(x => x.admin);

            modelBuilder.Entity<Ranklist>().HasKey(x => x.rankID);
            modelBuilder.Entity<Ranklist>().HasOne(x => x.airline);

            modelBuilder.Entity<PendingAirlineApp>().HasKey(x => x.appID);
            modelBuilder.Entity<PendingAirlineApp>().HasOne(x => x.airline);
            modelBuilder.Entity<PendingAirlineApp>().HasOne(x => x.applyingPlayer);

            modelBuilder.Entity<AirlineAnnouncement>().HasKey(x => x.announcementID);
            modelBuilder.Entity<AirlineAnnouncement>().HasOne(x => x.createdBy);
            modelBuilder.Entity<AirlineAnnouncement>().HasOne(x => x.airline);

            base.OnModelCreating(modelBuilder);
        }
    }
}
