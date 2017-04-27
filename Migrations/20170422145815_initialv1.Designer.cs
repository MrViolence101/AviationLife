using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using core.DatabaseRelated.Models;

namespace core.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20170422145815_initialv1")]
    partial class initialv1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("core.DatabaseRelated.Models.Airline", b =>
                {
                    b.Property<int>("airlineID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("airlineName");

                    b.Property<string>("amotd");

                    b.HasKey("airlineID");

                    b.ToTable("airlines");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.Ban", b =>
                {
                    b.Property<int>("banID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("banDate");

                    b.Property<string>("banReason");

                    b.Property<int?>("playerID");

                    b.HasKey("banID");

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

                    b.Property<DateTime>("dob");

                    b.Property<bool>("isBanned");

                    b.Property<string>("password");

                    b.Property<float>("posx");

                    b.Property<float>("posy");

                    b.Property<float>("posz");

                    b.Property<string>("username");

                    b.HasKey("playerID");

                    b.HasIndex("airlineID");

                    b.ToTable("players");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.Ban", b =>
                {
                    b.HasOne("core.DatabaseRelated.Models.User", "player")
                        .WithMany()
                        .HasForeignKey("playerID");
                });

            modelBuilder.Entity("core.DatabaseRelated.Models.User", b =>
                {
                    b.HasOne("core.DatabaseRelated.Models.Airline", "airline")
                        .WithMany()
                        .HasForeignKey("airlineID");
                });
        }
    }
}
