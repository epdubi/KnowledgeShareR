using KnowledgeShareR.Models;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeShareR.Data
{
    public class PresentationContext : DbContext
    {
        public PresentationContext (DbContextOptions<PresentationContext> options)
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