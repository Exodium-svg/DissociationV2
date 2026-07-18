using Application.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Db;
/// <summary>
/// 
/// </summary>
public class DatabaseContext : DbContext
{
    /// <summary>
    /// 
    /// </summary>
    private const string DEFAULT_DATA_SOURCE = "Data Source=database.db";

    /// <summary>
    /// 
    /// </summary>
    public DbSet<StarboardEntry> StarboardEntries { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DbSet<Post> PostEntries { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DbSet<GuildMember> GuildMembersEntries { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DbSet<Guild> GuildEntries { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DbSet<Channel> ChannelEntries { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(DEFAULT_DATA_SOURCE);
}

