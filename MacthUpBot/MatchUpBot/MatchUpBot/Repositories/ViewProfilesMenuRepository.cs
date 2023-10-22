using Data;
using Entities;
using EntityFrameworkLesson.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MatchUpBot.Repositories;

public class ViewProfilesMenuRepository
{
    private static readonly Context _context = new();
    
    private static readonly ILogger<ViewProfilesMenuRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ViewProfilesMenuRepository>();

    public void AddLike(long likedUserId, long likerId)
    {
        UserRepository ur = new UserRepository();
        var liker = ur.GetUser(likerId);
        var likedUser = ur.GetUser(likedUserId);
        _logger.LogInformation($"liker({liker.TgId})");
        _logger.LogInformation($"likedUser({likedUser.TgId})");

        var existingLike = _context.Likes
            .FirstOrDefault(like => like.LikedUserId == likedUser.TgId && like.LikedByUserId == liker.TgId);
        
        
        if (existingLike == null)
        {
            var like = new LikesEntity
            {
                Id = Guid.NewGuid(),
                LikedUserId = likedUser.TgId,
                LikedByUserId = liker.TgId
            };

            _context.Likes.Add(like);
            _context.SaveChanges();
        }
        else
        {
            _logger.LogInformation($"Like hasn't been added because it already exists.");
        }
    }
    
    public UserEntity GetMatchingProfile(int age, string city, long recieverId, int ageDifference)
    {
        var totalProfilesCount = _context.Users.Count();

        var randomStart = new Random().Next(totalProfilesCount);

        var matchingProfile = _context.Users
            .Where(user => (user.Age >= age - ageDifference && user.Age <= age + ageDifference) && user.City == city && user.TgId != recieverId)
            .OrderBy(user => user.TgId)
            .Skip(randomStart)
            .FirstOrDefault();

        if (matchingProfile == null)
        {
            matchingProfile = _context.Users
                .Where(user => (user.Age >= age - ageDifference && user.Age <= age + ageDifference) && user.City == city && user.TgId != recieverId)
                .OrderBy(user => user.TgId)
                .FirstOrDefault();
        }

        return matchingProfile;
    }
    
    public static long GetLikerId(long userId)
    {
        var like = _context.Likes.FirstOrDefault(like => like.LikedUserId == userId);
        return like.LikedByUserId;
    }
}