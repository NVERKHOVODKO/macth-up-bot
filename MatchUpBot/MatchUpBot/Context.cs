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
            optionsBuilder.UseNpgsql(@"host=localhost;port=5432;database=EntityFrameworkLesson;username=postgres;password=root")
                //.LogTo(Console.WriteLine)
                ;
        }
        
        public DbSet<UserEntity> UserEntity { get; set; }
        public DbSet<LikesEntity> LikesEntity { get; set; }
    }
}
