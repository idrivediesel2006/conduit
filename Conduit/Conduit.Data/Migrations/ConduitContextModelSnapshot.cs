﻿// <auto-generated />
using System;
using Conduit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Conduit.Data.Migrations
{
    [DbContext(typeof(ConduitContext))]
    partial class ConduitContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Conduit.Data.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varbinary(128)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varbinary(128)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Email" }, "IX_Accounts_Email_Unique")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Conduit.Data.Follow", b =>
                {
                    b.Property<int>("Follower")
                        .HasColumnType("int");

                    b.Property<int>("Following")
                        .HasColumnType("int");

                    b.HasKey("Follower", "Following");

                    b.HasIndex("Following");

                    b.ToTable("Follows");
                });

            modelBuilder.Entity("Conduit.Data.Person", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Bio")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasDefaultValue("'https://static.productionready.io/images/smiley-cyrus.jpg'");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("Conduit.Data.Follow", b =>
                {
                    b.HasOne("Conduit.Data.Person", "FollowerNavigation")
                        .WithMany("FollowerNavigations")
                        .HasForeignKey("Follower")
                        .HasConstraintName("FK_Follower_Profiles")
                        .IsRequired();

                    b.HasOne("Conduit.Data.Person", "FollowingNavigation")
                        .WithMany("FollowingNavigations")
                        .HasForeignKey("Following")
                        .HasConstraintName("FK_Following_Profiles")
                        .IsRequired();

                    b.Navigation("FollowerNavigation");

                    b.Navigation("FollowingNavigation");
                });

            modelBuilder.Entity("Conduit.Data.Person", b =>
                {
                    b.HasOne("Conduit.Data.Account", "Account")
                        .WithOne("Person")
                        .HasForeignKey("Conduit.Data.Person", "Id")
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Conduit.Data.Account", b =>
                {
                    b.Navigation("Person");
                });

            modelBuilder.Entity("Conduit.Data.Person", b =>
                {
                    b.Navigation("FollowerNavigations");

                    b.Navigation("FollowingNavigations");
                });
#pragma warning restore 612, 618
        }
    }
}
