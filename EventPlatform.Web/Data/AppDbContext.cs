using EventPlatform.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EventPlatform.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ModerationHistory> ModerationHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.OrganizerId);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Status);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.StartDateTime);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.EventType);

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.EventId);

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.UserId);

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.TicketNumber)
                .IsUnique();

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.Status);

            modelBuilder.Entity<ModerationHistory>()
                .HasIndex(m => m.EventId);

            modelBuilder.Entity<ModerationHistory>()
                .HasIndex(m => m.ModeratorId);
        }
    }
}