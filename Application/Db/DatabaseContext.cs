using Application.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Db;
public class DatabaseContext : DbContext
{
    public DbSet<StarboardEntry> StarboardEntries { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=database.db");
    
}

