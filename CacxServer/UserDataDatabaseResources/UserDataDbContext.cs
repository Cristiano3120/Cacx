using Microsoft.EntityFrameworkCore;

namespace CacxServer.UserDataDatabaseResources;

public class UserDataDbContext : DbContext
{
    public DbSet<DbUser> Users { get; set; }

    public UserDataDbContext(DbContextOptions<UserDataDbContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.Entity<DbUser>()
            .HasKey(u => u.Id);

        _ = modelBuilder.Entity<DbUser>()
            .HasIndex(u => u.EmailHash)
            .IsUnique();

        _ = modelBuilder.Entity<DbUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        _ = modelBuilder.Entity<DbUser>()
            .HasIndex(u => u.CreatedAt);

        _ = modelBuilder.Entity<DbUser>()
            .HasIndex(u => u.Verified);
    }
}

