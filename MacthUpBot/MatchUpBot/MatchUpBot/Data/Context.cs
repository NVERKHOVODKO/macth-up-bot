using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class Context : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<InterestEntity> Interests { get; set; }
    public DbSet<LikesEntity> Likes { get; set; }
    public DbSet<UserInterestsEntity> UserInterestsEntities { get; set; }
    public DbSet<InterestWeightEntity> InterestWeightEntities { get; set; } 
    public DbSet<CardEntity> CreditCards { get; set; }
    public DbSet<BlanksShowingHistory> BlanksShowingHistory { get; set; }
    
    public Context() { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LikesEntity>()
            .HasOne(like => like.LikedUser)
            .WithMany(user => user.LikedByUsers)
            .HasForeignKey(like => like.LikedUserId)
            .OnDelete(DeleteBehavior.Restrict); // При необходимости укажите поведение при удалении

        modelBuilder.Entity<LikesEntity>()
            .HasOne(like => like.Liker)
            .WithMany(user => user.LikedUsers)
            .HasForeignKey(like => like.LikedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        /*optionsBuilder.UseNpgsql(
            @"host=localhost;port=5432;database=TelegramBot;username=postgres;password=postgres");*/
        // optionsBuilder.UseNpgsql(
        //     @"host=localhost;port=5432;database=Messenger;username=postgres;password=root");
        
        optionsBuilder.UseNpgsql(
           @"Host=postgres_db;Port=5432;Database=TelegramBot;Username=postgres;Password=root"); 
    }
}