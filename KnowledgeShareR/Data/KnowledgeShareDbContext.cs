using KnowledgeShareR.Models;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeShareR.Data
{
    public class KnowledgeShareDbContext : DbContext
    {
        public KnowledgeShareDbContext (DbContextOptions<KnowledgeShareDbContext> options)
            : base(options)
        {
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>()
            .HasMany(b => b.Answers)
            .WithOne(b => b.Question);
        }
    }
}