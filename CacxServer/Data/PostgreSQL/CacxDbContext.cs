using CacxServer.Data.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CacxServer.Data.PostgreSQL;

public sealed class CacxDbContext : DbContext
{
    public DbSet<DatabaseUser> Users { get; set; }
}
