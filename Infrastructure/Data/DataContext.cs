using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<DummyValue> DummyValues { get; set; }
        public DbSet<TechniqueChart> TechniqueCharts { get; set; }
        public DbSet<Technique> Techniques { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogSection> BlogSections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TechniqueChart>().HasMany(t => t.Techniques).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Blog>().HasMany(b => b.Sections).WithOne().OnDelete(DeleteBehavior.Cascade);
        }

    }
}