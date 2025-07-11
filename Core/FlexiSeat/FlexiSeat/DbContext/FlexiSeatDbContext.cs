using FlexiSeat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;

namespace FlexiSeat.DbContext
{
    public class FlexiSeatDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }

        public FlexiSeatDbContext(DbContextOptions<FlexiSeatDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
              .HasIndex(u => u.BadgeId)
              .IsUnique();

            // Optional: configure self-referencing relationships for clarity
            modelBuilder.Entity<User>()
                .HasOne(u => u.Lead)
                .WithMany()
                .HasForeignKey(u => u.LeadADID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany()
                .HasForeignKey(u => u.ManagerADID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zone>()
              .HasIndex(d => d.Name)
              .IsUnique();

            modelBuilder.Entity<Seat>()
             .HasIndex(c => c.Number)
             .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}
