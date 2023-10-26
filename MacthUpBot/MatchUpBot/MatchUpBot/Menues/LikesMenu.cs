using MatchUpBot.Repositories;
using Microsoft.Extensions.Logging;

namespace ConsoleApplication1.Menues;

public class LikesMenu
{
    private static ILogger<LikesMenu> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LikesMenu>();

    public LikesMenu(ILogger<LikesMenu> logger)
    {
        _logger = logger;
    }

    public static long GetLikerId(long userId)
    {
        var likerId = ViewProfilesMenuRepository.GetLikerId(userId);
        _logger.LogInformation($"GetLikerId ({userId}): {likerId}");
        return likerId;
    }
}