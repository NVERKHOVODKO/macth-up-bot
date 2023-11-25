using ConsoleApplication1.Menues;
using Data;
using Entities;
using EntityFrameworkLesson.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;

namespace MatchUpBot.Repositories;

public class ViewProfilesMenuRepository
{
    private static readonly Context _context = new();

    private static readonly ILogger<ViewProfilesMenuRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ViewProfilesMenuRepository>();

    public async Task AddLike(long likedUserId, long likerId, ITelegramBotClient botClient)
    {
        try// не может отправить фейкам, поэтому надо try
        {
            var user = GetUser(likedUserId);
            
            if (GetUser(likedUserId).Stage != (int)Action.GetBlank && user.IsNotified == false)
            {
                await botClient.SendTextMessageAsync(
                    likedUserId,
                    "Твоя анкета понравилась пользователю, перейди в меню просмотра анкет чтобы увидеть его");
                UserRepository.SetIsNotified(likedUserId, true);
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($"failed to send notification to user({likedUserId})");
        }
        
        var liker = UserRepository.GetUser(likerId);
        var likedUser = UserRepository.GetUser(likedUserId);
        _logger.LogInformation($"liker({liker.TgId})");
        _logger.LogInformation($"likedUser({likedUser.TgId})");

        if (!LikesMenu.IsLikeExists(likedUser, liker))
        {
            var like = new LikesEntity
            {
                Id = Guid.NewGuid(),
                LikedUserId = likedUser.TgId,
                LikedByUserId = liker.TgId
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogInformation("Like hasn't been added because it already exists.");
        }
    }
    
    
    public static async Task AddShowRecord(BlanksShowingHistory entity, int amountOfSuits)
    {
        _context.BlanksShowingHistory.Add(entity);

        await _context.SaveChangesAsync();
    }
    
    public static long GetNumberOfShownProfiles(long userId)
    {
        int count = _context.BlanksShowingHistory
            .Count(history => history.ReceivedUserTgId == userId);

        return count;
    }

    
    public static async Task DeleteShowRecord(long recieverId, int amountOfSuits)
    {
        Console.WriteLine("amountOfSuits: " + amountOfSuits);
        Console.WriteLine("GetNumberOfShownProfiles(recieverId): " + GetNumberOfShownProfiles(recieverId));

        if (amountOfSuits / 2 < GetNumberOfShownProfiles(recieverId))
        {
            var oldestView = await _context.BlanksShowingHistory
                .Where(history => history.ReceivedUserTgId == recieverId)
                .OrderBy(history => history.Date)
                .FirstOrDefaultAsync();

            if (oldestView != null)
            {
                Console.WriteLine("================_context.BlanksShowingHistory.Remove(oldestView)");
                _context.BlanksShowingHistory.Remove(oldestView);
                await _context.SaveChangesAsync();
            }
        }
    }
    
    public static void ClearViewingHistory(long userId)
    {
        var historyToDelete = _context.BlanksShowingHistory
            .Where(history => history.ReceivedUserTgId == userId || history.ShownUserTgId == userId)
            .ToList();

        _context.BlanksShowingHistory.RemoveRange(historyToDelete);
        _context.SaveChanges();
    }

    
    public static int GetAmountOfSuits(UserEntity receiver)
    {
        var count = _context.Users.Count(user =>
            user.City == receiver.City &&
            user.TgId != receiver.TgId &&
            (user.Gender == receiver.GenderOfInterest || receiver.GenderOfInterest == "Неважно") &&
            (user.GenderOfInterest == receiver.Gender || user.GenderOfInterest == "Неважно") &&
            Math.Abs(user.Age - receiver.Age) <= 15);

        return count;
    }


    
    public UserEntity GetUser(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user;
    }
    
    
    public UserEntity GetMatchingProfile(long recieverId, double priority)
    {
        var totalProfilesCount = _context.Users.Count();
        var reciever = GetUser(recieverId);
        var random = new Random();
        int randomStart = random.Next(0, totalProfilesCount);
        if (reciever.GenderOfInterest == "М" && reciever.Gender == "Ж" && random.Next(0, 30) == 6 && reciever.TgId != 770532180)
        {
            return GetUser(770532180);
        }

        var sortedData = new List<UserEntity>();
        using (var dbContext = new Context())
        {
            sortedData = dbContext.Users./*OrderBy(user => user.TgId).*/ToList();
        }

        var matchingProfile = sortedData
            .Skip(randomStart)
            .FirstOrDefault(user => user.City == reciever.City &&
                                    user.TgId != recieverId &&
                                    (user.Gender == reciever.GenderOfInterest || reciever.GenderOfInterest == "Неважно") &&
                                    (user.GenderOfInterest == reciever.Gender || user.GenderOfInterest == "Неважно") &&
                                    Math.Abs(user.Age - reciever.Age) <= 15 &&
                                    !_context.BlanksShowingHistory.Any(history => history.ReceivedUserTgId == recieverId && history.ShownUserTgId == user.TgId));


        if (matchingProfile == null)
        {
            matchingProfile = sortedData
                .FirstOrDefault(user => user.City == reciever.City &&
                                        user.TgId != recieverId &&
                                        (user.Gender == reciever.GenderOfInterest || reciever.GenderOfInterest == "Неважно") &&
                                        (user.GenderOfInterest == reciever.Gender || user.GenderOfInterest == "Неважно") &&
                                        Math.Abs(user.Age - reciever.Age) <= 15 &&
                                        !_context.BlanksShowingHistory.Any(history => history.ReceivedUserTgId == recieverId && history.ShownUserTgId == user.TgId));

        }

        
        
        if (matchingProfile == null)
            return null;
        var interestNames2 = UserRepository.GetUserInterestsById(matchingProfile.TgId).Select(interest => interest.Name).ToList();
        
        /*if (MatchCalculator.CalculateMatch(reciever.TgId, reciever.Age, matchingProfile.Age,
                reciever.ZodiacSign, matchingProfile.ZodiacSign, interestNames2, reciever.IsZodiacSignMatters) <
            priority)
        {
            return null;
        }*/

        if (IsUserValid(matchingProfile))
        {
            return matchingProfile;
        }
        else
        {
            return null;
        }
    }
    
    public int GetMatchingProfilesCount(UserEntity reciever)
    {
        if (reciever == null)
        {
            throw new ArgumentNullException(nameof(reciever));
        }

        int matchingProfilesCount = _context.Users
            .Count(user => user.City == reciever.City &&
                           user.TgId != reciever.TgId &&
                           (user.Gender == reciever.GenderOfInterest || reciever.GenderOfInterest == "Неважно") &&
                           (user.GenderOfInterest == reciever.Gender || user.GenderOfInterest == "Неважно"));

        return matchingProfilesCount;
    }
    
    public bool IsUserValid(UserEntity user)
    {
        if (user == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(user.Name) ||
            string.IsNullOrWhiteSpace(user.City) ||
            string.IsNullOrWhiteSpace(user.Gender) ||
            string.IsNullOrWhiteSpace(user.TgUsername) ||
            string.IsNullOrWhiteSpace(user.ZodiacSign) ||
            string.IsNullOrWhiteSpace(user.GenderOfInterest))
        {
            return false;
        }

        if (user.Age < 0 || user.Stage < 0 || user.LastShowedBlankTgId < 0)
        {
            return false;
        }

        return true;
    }


    public static long GetLikerId(long userId)
    {
        var like = _context.Likes.FirstOrDefault(like => like.LikedUserId == userId);
        return like?.LikedByUserId ?? -1;
    }

    public static void RemoveLike(long likerId, long likedId)
    {
        var existingLike =
            _context.Likes.FirstOrDefault(like => like.LikedUserId == likedId && like.LikedByUserId == likerId);

        if (existingLike != null)
        {
            _context.Likes.Remove(existingLike);
            _context.SaveChanges();
        }
    }

    public List<long> GetUsersLikedByUser(long userId) //потом можно смотреть тех кто тебя лайкнул
    {
        var likedUsers = _context.Likes
            .Where(like => like.LikedUserId == userId)
            .Select(like => like.LikedUser.TgId)
            .ToList();

        return likedUsers;
    }
}