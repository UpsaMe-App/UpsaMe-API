using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Models;

namespace UpsaMe_API.Data
{
    public class UpsaMeDbContext : DbContext
    {
        public UpsaMeDbContext(DbContextOptions<UpsaMeDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Faculty> Faculties => Set<Faculty>();
        public DbSet<Career> Careers => Set<Career>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostReply> PostReplies => Set<PostReply>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Career>()
                .HasOne(c => c.Faculty)
                .WithMany(f => f.Careers)
                .HasForeignKey(c => c.FacultyId);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Career)
                .WithMany(c => c.Subjects)
                .HasForeignKey(s => s.CareerId);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Subject)
                .WithMany()
                .HasForeignKey(p => p.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PostReply>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Replies)
                .HasForeignKey(r => r.PostId);

            modelBuilder.Entity<PostReply>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

