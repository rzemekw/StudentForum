using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLibrary.DataAccess
{
    public class ForumContext : DbContext
    {
        public ForumContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<TopicAnswer> Answers { get; set; }
        public DbSet<Attachement> Attachements { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserUniversity> UserUniversities { get; set; }
        public DbSet<UserTopicDate> UserTopicDates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => new { ug.UserId , ug.GroupId });
            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserUniversity>()
                .HasKey(uu => new { uu.UserId, uu.UniversityId });
            modelBuilder.Entity<UserUniversity>()
                .HasOne(uu => uu.User)
                .WithMany(u => u.UserUniversities)
                .HasForeignKey(uu => uu.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserUniversity>()
                .HasOne(uu => uu.University)
                .WithMany(ur => ur.UserUniversities)
                .HasForeignKey(uu => uu.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Admin)
                .WithMany(u => u.AdministratedGroups)
                .HasForeignKey(g => g.AdminId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<University>()
                .HasIndex(u => u.Website);

            modelBuilder.Entity<TopicAnswer>()
                .HasOne(a => a.Topic)
                .WithMany(t => t.Answers)
                .HasForeignKey(a => a.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attachement>()
                .HasOne(a => a.TopicAnswer)
                .WithMany(a => a.Attachements)
                .HasForeignKey(a => a.TopicAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTopicDate>()
                .HasKey(utd => new { utd.TopicId, utd.UserId });

            modelBuilder.Entity<UserTopicDate>()
                .HasOne(utd => utd.User)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
