using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using core.DatabaseRelated.Models;

namespace core.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20170430131218_initialv11")]
    partial class initialv11
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("core.DatabaseRelated.Models.AirlineRelated.Airline", b =>
                {
                    b.Property<int>("airlineID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AppAuthRank");

                    b.Property<string>("airlineName");

                    b.Property<string>("amotd");

                    b.Property<int>("bankBalance");

                    b.HasKey("airlineID");

                    b.ToTable("airlines");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.AirlineRelated.AirlineAnnouncement", b =>
                {
                    b.Property<int>("announcementID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<int?>("airlineID");

                    b.Property<string>("announcement");

                    b.Property<int?>("createdByplayerID");

                    b.HasKey("announcementID");

                    b.HasIndex("airlineID");

                    b.HasIndex("createdByplayerID");

                    b.ToTable("airlineAnnouncements");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.AirlineRelated.PendingAirlineApp", b =>
                {
                    b.Property<int>("appID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateApplied");

                    b.Property<int?>("airlineID");

                    b.Property<int?>("applyingPlayerplayerID");

                    b.HasKey("appID");

                    b.HasIndex("airlineID");

                    b.HasIndex("applyingPlayerplayerID");

                    b.ToTable("pendingApplications");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.AirlineRelated.Ranklist", b =>
                {
                    b.Property<int>("rankID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("airlineID");

                    b.Property<string>("rankName");

                    b.Property<int>("rankSpot");

                    b.HasKey("rankID");

                    b.HasIndex("airlineID");

                    b.ToTable("ranks");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.Ban", b =>
                {
                    b.Property<int>("banID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("adminplayerID");

                    b.Property<DateTime>("banDate");

                    b.Property<string>("banReason");

                    b.Property<int?>("playerID");

                    b.HasKey("banID");

                    b.HasIndex("adminplayerID");

                    b.HasIndex("playerID");

                    b.ToTable("bans");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.User", b =>
                {
                    b.Property<int>("playerID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("airlineID");

                    b.Property<int>("alevel");

                    b.Property<int>("arank");

                    b.Property<int>("cash");

                    b.Property<DateTime>("dob");

                    b.Property<bool>("isBanned");

                    b.Property<bool>("isOnline");

                    b.Property<string>("password");

                    b.Property<float>("posx");

                    b.Property<float>("posy");

                    b.Property<float>("posz");

                    b.Property<string>("username");

                    b.HasKey("playerID");

                    b.HasIndex("airlineID");

                    b.ToTable("players");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.AirlineRelated.AirlineAnnouncement", b =>
                {
                    b.HasOne("core.DatabaseRelated.Models.AirlineRelated.Airline", "airline")
                        .WithMany()
                        .HasForeignKey("airlineID");

                    b.HasOne("core.DatabaseRelated.Models.User", "createdBy")
                        .WithMany()
                        .HasForeignKey("createdByplayerID");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.AirlineRelated.PendingAirlineApp", b =>
                {
                    b.HasOne("core.DatabaseRelated.Models.AirlineRelated.Airline", "airline")
                        .WithMany()
                        .HasForeignKey("airlineID");

                    b.HasOne("core.DatabaseRelated.Models.User", "applyingPlayer")
                        .WithMany()
                        .HasForeignKey("applyingPlayerplayerID");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.AirlineRelated.Ranklist", b =>
                {
                    b.HasOne("core.DatabaseRelated.Models.AirlineRelated.Airline", "airline")
                        .WithMany()
                        .HasForeignKey("airlineID");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.Ban", b =>
                {
                    b.HasOne("core.DatabaseRelated.Models.User", "admin")
                        .WithMany()
                        .HasForeignKey("adminplayerID");

                    b.HasOne("core.DatabaseRelated.Models.User", "player")
                        .WithMany()
                        .HasForeignKey("playerID");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.User", b =>
                {
                    b.HasOne("core.DatabaseRelated.Models.AirlineRelated.Airline", "airline")
                        .WithMany()
                        .HasForeignKey("airlineID");
                });
        }
    }
}
