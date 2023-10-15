using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

internal class Context : DbContext
{
    
    public Context()
    {
        /*Database.EnsureDeleted();
        Database.EnsureCreated();*/
    }
    
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        /*optionsBuilder.UseNpgsql(
            @"host=localhost;port=5432;database=TelegramBot;username=postgres;password=postgres");*/
        optionsBuilder.UseNpgsql(
            @"host=localhost;port=5432;database=EntityFrameworkLesson;username=postgres;password=root");
    }
}