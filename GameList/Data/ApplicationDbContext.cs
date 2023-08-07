using GameList.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameList.Data
{
    //This code is defining an ApplicationDbContext class that inherits from IdentityDbContext with AppUser as its generic parameter.
    //It has a constructor that takes a DbContextOptions object and passes it to the base constructor. The class also has three DbSet properties,
    //UserGameLists, UserFollows, and UserMessages. The OnModelCreating method is overridden to set up relationships between these entities using
    //the Entity Framework Core Fluent API, such as one-to-many relationships between UserFollow and AppUser, and UserMessage and AppUser.
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<UserGameList> UserGameLists { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Followed)
                .WithMany(u => u.Followers)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserMessage>()
                .HasOne(um => um.Sender)
                .WithMany(u => u.SentMessages)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserMessage>()
                .HasOne(um => um.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
