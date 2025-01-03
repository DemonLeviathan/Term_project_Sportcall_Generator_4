﻿// <auto-generated />
using System;
using Generator.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Generator.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Challenge", b =>
                {
                    b.Property<int>("ChallengeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ChallengeId"));

                    b.Property<int>("CallId")
                        .HasColumnType("integer");

                    b.Property<int>("ReceiverId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("RespondedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SenderId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ChallengeId");

                    b.HasIndex("CallId");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("Generator.Domain.Activities", b =>
                {
                    b.Property<int>("activity_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("activity_id"));

                    b.Property<string>("activity_name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("activity_type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("activity_id");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("Generator.Domain.Calls", b =>
                {
                    b.Property<int>("call_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("call_id"));

                    b.Property<int?>("Usersuser_id")
                        .HasColumnType("integer");

                    b.Property<string>("call_date")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("call_name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("friend_id")
                        .HasColumnType("integer");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.HasKey("call_id");

                    b.HasIndex("Usersuser_id");

                    b.HasIndex("friend_id");

                    b.ToTable("Calls");
                });

            modelBuilder.Entity("Generator.Domain.DailyActivity", b =>
                {
                    b.Property<int>("dailyAcivityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("dailyAcivityId"));

                    b.Property<DateTime>("date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float?>("otherActivityTime")
                        .HasColumnType("real");

                    b.Property<int>("stepQuantity")
                        .HasColumnType("integer");

                    b.Property<int>("userId")
                        .HasColumnType("integer");

                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.HasKey("dailyAcivityId");

                    b.HasIndex("userId");

                    b.HasIndex("user_id");

                    b.ToTable("DailyActivities");
                });

            modelBuilder.Entity("Generator.Domain.Friendship", b =>
                {
                    b.Property<int>("friend_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("friend_id"));

                    b.Property<bool>("IsPending")
                        .HasColumnType("boolean");

                    b.Property<string>("friendship_date")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("user1_id")
                        .HasColumnType("integer");

                    b.Property<int>("user2_id")
                        .HasColumnType("integer");

                    b.HasKey("friend_id");

                    b.HasIndex("user2_id");

                    b.HasIndex("user1_id", "user2_id")
                        .IsUnique();

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("Generator.Domain.UserData", b =>
                {
                    b.Property<int>("data_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("data_id"));

                    b.Property<int>("activity_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("date_info")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("date")
                        .HasDefaultValueSql("CURRENT_DATE");

                    b.Property<float>("height")
                        .HasColumnType("real");

                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.Property<float>("weight")
                        .HasColumnType("real");

                    b.HasKey("data_id");

                    b.HasIndex("activity_id");

                    b.HasIndex("user_id");

                    b.ToTable("UserData");
                });

            modelBuilder.Entity("Generator.Domain.Users", b =>
                {
                    b.Property<int>("user_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("user_id"));

                    b.Property<string>("birthday")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("user_role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("user_id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserCall", b =>
                {
                    b.Property<int>("UserCallId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserCallId"));

                    b.Property<DateTime>("AssignedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CallId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("UserCallId");

                    b.HasIndex("CallId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCalls");
                });

            modelBuilder.Entity("Challenge", b =>
                {
                    b.HasOne("Generator.Domain.Calls", "Call")
                        .WithMany()
                        .HasForeignKey("CallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Generator.Domain.Users", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Generator.Domain.Users", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Call");

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Generator.Domain.Calls", b =>
                {
                    b.HasOne("Generator.Domain.Users", null)
                        .WithMany("Calls")
                        .HasForeignKey("Usersuser_id");

                    b.HasOne("Generator.Domain.Friendship", "Friendship")
                        .WithMany("Calls")
                        .HasForeignKey("friend_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Friendship");
                });

            modelBuilder.Entity("Generator.Domain.DailyActivity", b =>
                {
                    b.HasOne("Generator.Domain.Users", null)
                        .WithMany("DailyActivities")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Generator.Domain.Users", "User")
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Generator.Domain.Friendship", b =>
                {
                    b.HasOne("Generator.Domain.Users", "User1")
                        .WithMany("Friendships1")
                        .HasForeignKey("user1_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Generator.Domain.Users", "User2")
                        .WithMany("Friendships2")
                        .HasForeignKey("user2_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("Generator.Domain.UserData", b =>
                {
                    b.HasOne("Generator.Domain.Activities", "Activity")
                        .WithMany("UserData")
                        .HasForeignKey("activity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Generator.Domain.Users", "User")
                        .WithMany("UserData")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserCall", b =>
                {
                    b.HasOne("Generator.Domain.Calls", "Call")
                        .WithMany()
                        .HasForeignKey("CallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Generator.Domain.Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Call");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Generator.Domain.Activities", b =>
                {
                    b.Navigation("UserData");
                });

            modelBuilder.Entity("Generator.Domain.Friendship", b =>
                {
                    b.Navigation("Calls");
                });

            modelBuilder.Entity("Generator.Domain.Users", b =>
                {
                    b.Navigation("Calls");

                    b.Navigation("DailyActivities");

                    b.Navigation("Friendships1");

                    b.Navigation("Friendships2");

                    b.Navigation("UserData");
                });
#pragma warning restore 612, 618
        }
    }
}
