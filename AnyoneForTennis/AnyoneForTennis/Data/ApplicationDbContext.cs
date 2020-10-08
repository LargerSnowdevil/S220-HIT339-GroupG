using System;
using System.Collections.Generic;
using System.Text;
using AnyoneForTennis.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnyoneForTennis.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Member> Members { get; set; }

        public DbSet<Coach> Coaches { get; set; }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventMember>()
                .HasKey(em => new { em.EventId, em.MemberId });
            modelBuilder.Entity<EventMember>()
                .HasOne(em => em.Event)
                .WithMany(e => e.EventMembers)
                .HasForeignKey(em => em.EventId);
            modelBuilder.Entity<EventMember>()
                .HasOne(em => em.Member)
                .WithMany(m => m.EventMembers)
                .HasForeignKey(em => em.MemberId);
        }
    }
}
