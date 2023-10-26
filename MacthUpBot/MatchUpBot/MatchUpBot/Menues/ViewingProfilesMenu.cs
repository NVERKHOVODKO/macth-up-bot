using Data;
using Entities;
using MatchUpBot.Repositories;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

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

    public static UserEntity GetMatchingProfile(int age, string city, long recieverId)
    {
        UserEntity user = null;
        var ageDifference = 10;
        while (user == null && ageDifference < 20)
        {
            _logger.LogInformation($"ageDifference = {ageDifference}");
            user = vpmr.GetMatchingProfile(age, city, recieverId, ageDifference);
            ageDifference++;
        }

        return user;
    }
    
    public static async Task ShowBlank(long userId, ITelegramBotClient botClient)
    {
        var user = UserRepository.GetUser(userId);
        _logger.LogInformation($"user({userId}): getting a blank");
        var userSearched = GetMatchingProfile(user.Age, user.City, userId);
        if (userSearched == null)
        {
            await botClient.SendTextMessageAsync(userId, "Не получилось найти кого-то подходящего для тебя(");
        }
        else
        {
            _logger.LogInformation($"user({userSearched.TgId}): getted to user({userId})");
            await PhotoRepository.SendBlank(userId, botClient, userSearched.TgId);
            _logger.LogInformation($"user({userSearched.TgId}): sended to user({userId})");
            UserRepository.SetLastShowedBlankTgId(userId, userSearched.TgId);
        }
    }
}