﻿using Generator.Domain;
using Microsoft.EntityFrameworkCore;

namespace Generator.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    public DbSet<Domain.Users> Users { get; set; }
    public DbSet<Domain.Activities> Activities { get; set; }    
    public DbSet<Domain.Friendship> Friendships { get; set; }
    public DbSet<Domain.UserData> UserData { get; set; }
    public DbSet<Domain.Calls> Calls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Activities>()
        .HasKey(a => a.activity_id);
        modelBuilder.Entity<Calls>()
        .HasKey(a => a.call_id);
        modelBuilder.Entity<Friendship>()
        .HasKey(a => a.friend_id);
        modelBuilder.Entity<UserData>()
        .HasKey(a => a.data_id);
        modelBuilder.Entity<Users>()
        .HasKey(a => a.user_id);


        modelBuilder.Entity<UserData>()
                .HasOne(ud => ud.User)
                .WithMany(u => u.UserData)
                .HasForeignKey(ud => ud.user_id)
                .OnDelete(DeleteBehavior.Cascade);  

        modelBuilder.Entity<UserData>()
            .HasOne(ud => ud.Activity)
            .WithMany(a => a.UserData)
            .HasForeignKey(ud => ud.activity_id)
            .OnDelete(DeleteBehavior.Cascade);  

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User1)
            .WithMany(u => u.Friendships1)
            .HasForeignKey(f => f.user1_id)
            .OnDelete(DeleteBehavior.Cascade);  

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User2)
            .WithMany(u => u.Friendships2)
            .HasForeignKey(f => f.user2_id)
            .OnDelete(DeleteBehavior.Cascade);  

        modelBuilder.Entity<Calls>()
            .HasOne(c => c.Friendship)
            .WithMany(f => f.Calls)
            .HasForeignKey(c => c.friend_id)
            .OnDelete(DeleteBehavior.Cascade);  

        modelBuilder.Entity<Friendship>()
            .HasIndex(f => new { f.user1_id, f.user2_id })
            .IsUnique();
    }
}
