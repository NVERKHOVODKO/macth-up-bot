﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MatchUpBot.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Entities.InterestEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Interests");
                });

            modelBuilder.Entity("Entities.LikesEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("LikedByUserId")
                        .HasColumnType("bigint");

                    b.Property<long>("LikedUserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("LikedByUserId");

                    b.HasIndex("LikedUserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("Entities.UserEntity", b =>
                {
                    b.Property<long>("TgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("TgId"));

                    b.Property<string>("About")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GenderOfInterest")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsNotified")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsZodiacSignMatters")
                        .HasColumnType("boolean");

                    b.Property<long>("LastShowedBlankTgId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Stage")
                        .HasColumnType("integer");

                    b.Property<string>("TgUsername")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ZodiacSign")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TgId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Entities.UserInterestsEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("InterestId")
                        .HasColumnType("uuid");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("InterestId");

                    b.HasIndex("UserId");

                    b.ToTable("UserInterestsEntities");
                });

            modelBuilder.Entity("Entities.LikesEntity", b =>
                {
                    b.HasOne("Entities.UserEntity", "Liker")
                        .WithMany("LikedUsers")
                        .HasForeignKey("LikedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Entities.UserEntity", "LikedUser")
                        .WithMany("LikedByUsers")
                        .HasForeignKey("LikedUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("LikedUser");

                    b.Navigation("Liker");
                });

            modelBuilder.Entity("Entities.UserInterestsEntity", b =>
                {
                    b.HasOne("Entities.InterestEntity", "Interest")
                        .WithMany("UserInterests")
                        .HasForeignKey("InterestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.UserEntity", "User")
                        .WithMany("UserInterests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Interest");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entities.InterestEntity", b =>
                {
                    b.Navigation("UserInterests");
                });

            modelBuilder.Entity("Entities.UserEntity", b =>
                {
                    b.Navigation("LikedByUsers");

                    b.Navigation("LikedUsers");

                    b.Navigation("UserInterests");
                });
#pragma warning restore 612, 618
        }
    }
}
