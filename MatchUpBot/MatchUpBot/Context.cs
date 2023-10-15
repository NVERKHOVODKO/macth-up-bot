using Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkLesson
{
    class Context : DbContext
    {
        public Context()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                @"host=localhost;port=5432;database=TelegramBot;username=postgres;password=postgres");
        }
        
        public DbSet<UserEntity> UserEntity { get; set; }
        //public DbSet<LikesEntity> LikesEntities { get; set; }
    }
}