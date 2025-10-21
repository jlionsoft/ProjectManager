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
            // USER → PROJECT (Responsibility)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Responsibility)
                .WithMany() // Optional: .WithMany(u => u.ResponsibleProjects)
                .HasForeignKey(p => p.ResponsibilityId)
                .OnDelete(DeleteBehavior.Restrict); // Verhindert Kaskadenschleifen

            // PROJECT → ASSIGNMENTS (1:n)
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Assignments)
                .WithOne(a => a.Project)
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Projektlöschen → zugehörige Aufgaben löschen

            // USER → ASSIGNMENTS (Zuweisung)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.User)
                .WithMany() // Optional: .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Kein automatisches Löschen bei Benutzerlöschung

            // ENUM: Priority als int speichern
            modelBuilder.Entity<Assignment>()
                .Property(a => a.Priority)
                .HasConversion<int>();

            // Optional: Indexe für Performance
            modelBuilder.Entity<Assignment>()
                .HasIndex(a => a.ProjectId);

            modelBuilder.Entity<Assignment>()
                .HasIndex(a => a.UserId);

            // Optional: Konfiguration für string-Felder
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Project>()
                .Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Assignment>()
                .Property(a => a.Title)
                .HasMaxLength(200);

            // Ignoriere ImageSource in User (bereits durch [NotMapped] abgesichert, hier redundant)
            modelBuilder.Entity<User>().Ignore(u => u.Image);
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