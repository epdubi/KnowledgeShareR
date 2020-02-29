using KnowledgeShareR.Models;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeShareR.Data
{
    public class KnowledgeShareDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ConnectedUser> ConnectedUsers { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public KnowledgeShareDbContext (DbContextOptions<KnowledgeShareDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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