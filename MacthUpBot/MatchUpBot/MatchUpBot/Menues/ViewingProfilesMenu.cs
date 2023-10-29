using Data;
using Entities;
using MatchUpBot.Repositories;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;

namespace ConsoleApplication1.Menues;

public class ViewingProfilesMenu
{
    public static readonly UserRepository UserRepository = new();

    private static ILogger<BlankMenu> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BlankMenu>();

    private static readonly ViewProfilesMenuRepository vpmr = new();


    public ViewingProfilesMenu(ILogger<BlankMenu> logger)
    {
        _logger = logger;
    }

    public static UserEntity GetMatchingProfile(long recieverId)
    {
        double priority = 70;
        UserEntity userEntity = null;
        while (userEntity == null)
        {
            userEntity = vpmr.GetMatchingProfile(recieverId, priority);
            if (priority < 20)
                return null;
            priority -= 5;
        }
        return userEntity;
    }

    public static async Task ShowBlank(long userId, ITelegramBotClient botClient)
    {
        var userSearched = GetMatchingProfile(userId);
        _logger.LogInformation($"user({userId}): getting a blank({userSearched.TgId})");
        if (userSearched == null)
        {
            await botClient.SendTextMessageAsync(userId, "Не получилось найти кого-то подходящего для тебя(");
            return;
        }
        else
        {
            _logger.LogInformation($"user({userSearched.TgId}): getted to user({userId})");
            await PhotoRepository.SendBlank(userId, botClient, userSearched.TgId);
            _logger.LogInformation($"user({userSearched.TgId}): sended to user({userId})");
            UserRepository.SetLastShowedBlankTgId(userId, userSearched.TgId);
            return;
        }
    }
}