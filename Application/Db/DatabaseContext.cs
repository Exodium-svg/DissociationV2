using Application.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Db;
public class DatabaseContext : DbContext
{
    private const string DEFAULT_DATA_SOURCE = "Data Source=database.db";
    public DbSet<StarboardEntry> StarboardEntries { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(DEFAULT_DATA_SOURCE);
    
}

