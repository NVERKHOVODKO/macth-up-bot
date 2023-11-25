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
    
    

    public static async Task<UserEntity> GetMatchingProfile(long recieverId, ITelegramBotClient botClient)
    {
        double priority = 70;
        UserEntity userEntity = null;
        
        int amountOfSuits = ViewProfilesMenuRepository.GetAmountOfSuits(UserRepository.GetUser(recieverId));
        await ViewProfilesMenuRepository.DeleteShowRecord(recieverId, amountOfSuits);
        while (userEntity == null)
        {
            userEntity = vpmr.GetMatchingProfile(recieverId, priority);
            if (priority < 30)
            {
                await botClient.SendTextMessageAsync(recieverId, "Не получилось найти кого-то подходящего для тебя(");
                return null;
            }
            priority -= 3;
        }
        Console.WriteLine($"priority: {priority}");
        
        
        if (amountOfSuits <= 1)
        {
            return null;
        }
        
        if (userEntity != null)
        {
            await ViewProfilesMenuRepository.AddShowRecord(new BlanksShowingHistory
            {
                Id = Guid.NewGuid(),
                ShownUserTgId = userEntity.TgId,
                ReceivedUserTgId = recieverId,
                Date = DateTime.UtcNow
            }, amountOfSuits);
        }

        return userEntity;
    }

    public static async Task ShowBlank(long userId, ITelegramBotClient botClient)
    {
        var userSearched = await GetMatchingProfile(userId, botClient);//берем подходящий профиль
        _logger.LogInformation($"user({userId}): getting a blank({userSearched.TgId})");//если нет такого то кидаем это
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