﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sidekick.Common.Database;

#nullable disable

namespace Sidekick.Common.Database.Migrations
{
    [DbContext(typeof(SidekickDbContext))]
    partial class SidekickDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("Sidekick.Common.Database.Tables.Setting", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Sidekick.Common.Database.Tables.WealthFullSnapshot", b =>
                {
                    b.Property<long>("Date")
                        .HasColumnType("INTEGER");

                    b.Property<string>("League")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<double>("Total")
                        .HasColumnType("REAL");

                    b.HasKey("Date");

                    b.ToTable("WealthFullSnapshots");
                });

            modelBuilder.Entity("Sidekick.Common.Database.Tables.WealthItem", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int>("Count")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GemLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Icon")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int?>("ItemLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("League")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int?>("MapTier")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MaxLinks")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<string>("StashId")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<double>("Total")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("WealthItems");
                });

            modelBuilder.Entity("Sidekick.Common.Database.Tables.WealthStash", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<long>("LastUpdate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("League")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Parent")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<double>("Total")
                        .HasColumnType("REAL");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("WealthStashes");
                });

            modelBuilder.Entity("Sidekick.Common.Database.Tables.WealthStashSnapshot", b =>
                {
                    b.Property<long>("Date")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StashId")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("League")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<double>("Total")
                        .HasColumnType("REAL");

                    b.HasKey("Date", "StashId");

                    b.ToTable("WealthStashSnapshots");
                });
#pragma warning restore 612, 618
        }
    }
}
