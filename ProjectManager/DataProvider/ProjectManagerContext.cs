using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProjectManager.Model;

namespace ProjectManager.DataProvider
{
    public class ProjectManagerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public ProjectManagerContext(DbContextOptions<ProjectManagerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
    public class ProjectManagerContextFactory : IDesignTimeDbContextFactory<ProjectManagerContext>
    {
        public ProjectManagerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectManagerContext>();
            optionsBuilder.UseSqlServer(App.DBConnectionString);

            return new ProjectManagerContext(optionsBuilder.Options);
        }
    }
    

}