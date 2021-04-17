﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YSI.CurseOfSilverCrown.Web.Data;

namespace YSI.CurseOfSilverCrown.Web.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210416150723_UserLastActivityDate")]
    partial class UserLastActivityDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Command", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Coffers")
                        .HasColumnType("int");

                    b.Property<string>("OrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TargetOrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("Warriors")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TargetOrganizationId");

                    b.HasIndex("Type");

                    b.ToTable("Commands");

                    b.HasData(
                        new
                        {
                            Id = "126ef807-7c48-4c9b-bafb-a67ac5e30663",
                            Coffers = 0,
                            OrganizationId = "TinMines",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "cdb29eaa-ae45-45aa-a9ac-9f35f634da19",
                            Coffers = 0,
                            OrganizationId = "CapeRaptor",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "e4ba5409-28bd-4dbf-ba66-b3d1720f950a",
                            Coffers = 0,
                            OrganizationId = "MouthOfPolaima",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "3ceaf5d1-15ff-4b6d-8291-9a0646c59b53",
                            Coffers = 0,
                            OrganizationId = "HeatherOfDimmoria",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "0441c49c-bbc7-4f64-b213-4e567ed62eb6",
                            Coffers = 0,
                            OrganizationId = "DimmoriaValley",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "0d7ec146-48a0-423b-8173-70398b102040",
                            Coffers = 0,
                            OrganizationId = "SummerCoast",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "a2d27f52-71af-43c0-9559-5ccb5ec9ba59",
                            Coffers = 0,
                            OrganizationId = "DimmoriaFarms",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "6550b859-f22a-4ce9-8a99-96463ad24a41",
                            Coffers = 0,
                            OrganizationId = "ChalRocks",
                            Type = 0,
                            Warriors = 0
                        },
                        new
                        {
                            Id = "d5cd667c-fc0e-4f9a-b03d-598af85f1bad",
                            Coffers = 0,
                            OrganizationId = "LimestoneRidges",
                            Type = 0,
                            Warriors = 0
                        });
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.EventStory", b =>
                {
                    b.Property<int>("TurnId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("EventStoryJson")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TurnId", "Id");

                    b.ToTable("EventStories");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Coffers")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrganizationType")
                        .HasColumnType("int");

                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.Property<string>("SuzerainId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Warriors")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationType");

                    b.HasIndex("ProvinceId");

                    b.HasIndex("SuzerainId");

                    b.ToTable("Organizations");

                    b.HasData(
                        new
                        {
                            Id = "TinMines",
                            Coffers = 7000,
                            Name = "Оловянные шахты",
                            OrganizationType = 1,
                            ProvinceId = 1,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "CapeRaptor",
                            Coffers = 7000,
                            Name = "Мыс ящера",
                            OrganizationType = 1,
                            ProvinceId = 2,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "MouthOfPolaima",
                            Coffers = 7000,
                            Name = "Устье Полаймы",
                            OrganizationType = 1,
                            ProvinceId = 3,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "HeatherOfDimmoria",
                            Coffers = 7000,
                            Name = "Верещатник Диммории",
                            OrganizationType = 1,
                            ProvinceId = 4,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "DimmoriaValley",
                            Coffers = 7000,
                            Name = "Долина Диммории",
                            OrganizationType = 1,
                            ProvinceId = 5,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "SummerCoast",
                            Coffers = 7000,
                            Name = "Летний берег",
                            OrganizationType = 1,
                            ProvinceId = 6,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "DimmoriaFarms",
                            Coffers = 7000,
                            Name = "Фермы Диммории",
                            OrganizationType = 1,
                            ProvinceId = 7,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "ChalRocks",
                            Coffers = 7000,
                            Name = "Меловые скалы",
                            OrganizationType = 1,
                            ProvinceId = 8,
                            Warriors = 100
                        },
                        new
                        {
                            Id = "LimestoneRidges",
                            Coffers = 7000,
                            Name = "Известняковые хребты",
                            OrganizationType = 1,
                            ProvinceId = 9,
                            Warriors = 100
                        });
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.OrganizationEventStory", b =>
                {
                    b.Property<int>("TurnId")
                        .HasColumnType("int");

                    b.Property<string>("OrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("EventStoryId")
                        .HasColumnType("int");

                    b.Property<int>("Importance")
                        .HasColumnType("int");

                    b.HasKey("TurnId", "OrganizationId", "EventStoryId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TurnId", "EventStoryId");

                    b.ToTable("OrganizationEventStories");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Province", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Provinces");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Оловянные шахты"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Мыс ящера"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Устье Полаймы"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Верещатник Диммории"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Долина Диммории"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Летний берег"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Фермы Диммории"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Меловые скалы"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Известняковые хребты"
                        });
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Turn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Started")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Turns");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsActive = true,
                            Started = new DateTime(2021, 4, 16, 15, 7, 21, 823, DateTimeKind.Utc).AddTicks(3098)
                        });
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastActivityTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("OrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("OrganizationId")
                        .IsUnique()
                        .HasFilter("[OrganizationId] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Command", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", "Organization")
                        .WithMany("Commands")
                        .HasForeignKey("OrganizationId");

                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", "Target")
                        .WithMany("ToOrganizationCommands")
                        .HasForeignKey("TargetOrganizationId");

                    b.Navigation("Organization");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.EventStory", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Turn", "Turn")
                        .WithMany("EventStories")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Turn");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Province", "Province")
                        .WithMany("Organizations")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", "Suzerain")
                        .WithMany("Vassals")
                        .HasForeignKey("SuzerainId");

                    b.Navigation("Province");

                    b.Navigation("Suzerain");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.OrganizationEventStory", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", "Organization")
                        .WithMany("OrganizationEventStories")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Turn", "Turn")
                        .WithMany("OrganizationEventStories")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.EventStory", "EventStory")
                        .WithMany("OrganizationEventStories")
                        .HasForeignKey("TurnId", "EventStoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EventStory");

                    b.Navigation("Organization");

                    b.Navigation("Turn");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", "Organization")
                        .WithOne("User")
                        .HasForeignKey("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", "OrganizationId");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.EventStory", b =>
                {
                    b.Navigation("OrganizationEventStories");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", b =>
                {
                    b.Navigation("Commands");

                    b.Navigation("OrganizationEventStories");

                    b.Navigation("ToOrganizationCommands");

                    b.Navigation("User");

                    b.Navigation("Vassals");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Province", b =>
                {
                    b.Navigation("Organizations");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Turn", b =>
                {
                    b.Navigation("EventStories");

                    b.Navigation("OrganizationEventStories");
                });
#pragma warning restore 612, 618
        }
    }
}
