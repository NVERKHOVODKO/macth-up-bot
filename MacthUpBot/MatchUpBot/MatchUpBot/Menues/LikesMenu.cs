using Data;
using Entities;
using MatchUpBot.Repositories;
using Microsoft.Extensions.Logging;

namespace ConsoleApplication1.Menues;

public class LikesMenu
{
    private static ILogger<LikesMenu> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LikesMenu>();

    private static readonly Context _context = new();

    
    public LikesMenu(ILogger<LikesMenu> logger)
    {
        _logger = logger;
    }


    public static bool IsLikeExists(UserEntity likedUser, UserEntity liker)
    {
        var existingLike = _context.Likes
            .FirstOrDefault(like => like.LikedUserId == likedUser.TgId && like.LikedByUserId == liker.TgId);

        return existingLike != null;
    }

    
    
    public static long GetLikerId(long userId)
    {
        var likerId = ViewProfilesMenuRepository.GetLikerId(userId);
        _logger.LogInformation($"GetLikerId ({userId}): {likerId}");
        return likerId;
    }
}