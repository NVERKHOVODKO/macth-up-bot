using Data;
using MatchUpBot.Repositories;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApplication1.Menues;

public class LikesMenu
{
    
    public static readonly UserRepository UserRepository = new();

    private static ILogger<LikesMenu> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LikesMenu>();
    private static ViewProfilesMenuRepository vpmr = new ViewProfilesMenuRepository();
    
    
    public LikesMenu(ILogger<LikesMenu> logger)
    {
        _logger = logger;
    }
    
    
    public static async Task ShowLikeBlank(Message message, ITelegramBotClient botClient)
    {
        var user = UserRepository.GetUser(message.From.Id);
        _logger.LogInformation($"user({message.From.Id}): getting a like blank");
        var userLiker = ViewingProfilesMenu.GetMatchingProfile(user.Age, user.City, message.From.Id);
        await PhotoRepository.SendLikerBlank(message, botClient, userLiker.TgId);
        _logger.LogInformation($"user({userLiker.TgId}): sended to user({message.From.Id})");
        UserRepository.SetLastShowedBlankTgId(message.From.Id, userLiker.TgId);
    }
    
    public static long GetLikerId(long userId)
    {
        var likerId = ViewProfilesMenuRepository.GetLikerId(userId);
        return likerId;    
    }
}