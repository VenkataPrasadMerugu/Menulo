using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Menulo.Infrastructure.Persistence;

public sealed class MenuloDbContextFactory : IDesignTimeDbContextFactory<MenuloDbContext>
{
    public MenuloDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MenuloDbContext>();
        var connectionString =
            "Host=localhost;Port=5432;Database=menulo_db;Username=admin;Password=admin123";
        optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("tbl_migrations_history"));
        return new MenuloDbContext(optionsBuilder.Options);
    }
}
