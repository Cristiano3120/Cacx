using CacxServer.Data.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CacxServer.Data.PostgreSQL;

public sealed class CacxDbContext(DbContextOptions<CacxDbContext> contextOptions) : DbContext(contextOptions)
{
    public DbSet<DatabaseUser> Users { get; set; }
}
