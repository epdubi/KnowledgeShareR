using KnowledgeShareR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KnowledgeShareR.Data
{
    public class KnowledgeShareDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ConnectedUser> ConnectedUsers { get; set; }
        public DbSet<Vote> Votes { get; set; }

        public KnowledgeShareDbContext ()
            : base()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Question>()
            .HasMany(b => b.Answers)
            .WithOne(b => b.Question);

            modelBuilder.Entity<Vote>()
            .ToTable("Votes");

            modelBuilder.Entity<ConnectedUser>()
            .ToTable("ConnectedUsers");
        }
    }
}