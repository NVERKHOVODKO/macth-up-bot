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
    
    public static async Task ShowBlank(Message message, ITelegramBotClient botClient)
    {
        var user = UserRepository.GetUser(message.From.Id);
        _logger.LogInformation($"user({message.From.Id}): getting a blank");
        var userSearched = GetMatchingProfile(user.Age, user.City, message.From.Id);
        if (userSearched == null)
        {
            await botClient.SendTextMessageAsync(message.From.Id, "Не получилось найти кого-то подходящего для тебя(");
        }
        else
        {
            _logger.LogInformation($"user({userSearched.TgId}): getted to user({message.From.Id})");
            await PhotoRepository.SendBlank(message, botClient, userSearched.TgId);
            _logger.LogInformation($"user({userSearched.TgId}): sended to user({message.From.Id})");
            UserRepository.SetLastShowedBlankTgId(message.From.Id, userSearched.TgId);
        }
    }
}