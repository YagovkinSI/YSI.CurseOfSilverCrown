﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YSI.CurseOfSilverCrown.Web.Data;

namespace YSI.CurseOfSilverCrown.Web.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("OrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Result")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetOrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TurnId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TargetOrganizationId");

                    b.HasIndex("TurnId");

                    b.HasIndex("Type");

                    b.ToTable("Commands");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("OrganizationType")
                        .HasColumnType("int");

                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.Property<string>("SuzerainId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationType");

                    b.HasIndex("ProvinceId");

                    b.HasIndex("SuzerainId");

                    b.ToTable("Organizations");

                    b.HasData(
                        new
                        {
                            Id = "TinMines",
                            OrganizationType = 1,
                            ProvinceId = 1
                        },
                        new
                        {
                            Id = "CapeRaptor",
                            OrganizationType = 1,
                            ProvinceId = 2
                        },
                        new
                        {
                            Id = "MouthOfPolaima",
                            OrganizationType = 1,
                            ProvinceId = 3
                        },
                        new
                        {
                            Id = "HeatherOfDimmoria",
                            OrganizationType = 1,
                            ProvinceId = 4
                        },
                        new
                        {
                            Id = "DimmoriaValley",
                            OrganizationType = 1,
                            ProvinceId = 5
                        },
                        new
                        {
                            Id = "SummerCoast",
                            OrganizationType = 1,
                            ProvinceId = 6
                        },
                        new
                        {
                            Id = "DimmoriaFarms",
                            OrganizationType = 1,
                            ProvinceId = 7
                        },
                        new
                        {
                            Id = "ChalRocks",
                            OrganizationType = 1,
                            ProvinceId = 8
                        },
                        new
                        {
                            Id = "LimestoneRidges",
                            OrganizationType = 1,
                            ProvinceId = 9
                        });
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

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Turns");
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

                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Turn", "Turn")
                        .WithMany("Commands")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");

                    b.Navigation("Target");

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

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", b =>
                {
                    b.HasOne("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", "Organization")
                        .WithOne("User")
                        .HasForeignKey("YSI.CurseOfSilverCrown.Web.Models.DbModels.User", "OrganizationId");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("YSI.CurseOfSilverCrown.Web.Models.DbModels.Organization", b =>
                {
                    b.Navigation("Commands");

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
                    b.Navigation("Commands");
                });
#pragma warning restore 612, 618
        }
    }
}
