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
        
        
        var ur = new UserRepository();
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
            _logger.LogInformation("Like hasn't been added because it already exists.");
        }
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
        var randomStart = new Random().Next(totalProfilesCount);

        var matchingProfile = _context.Users
            .Skip(randomStart)
            .FirstOrDefault(user => user.City == reciever.City && user.TgId != recieverId &&
                                    (user.Gender == reciever.GenderOfInterest ||
                                     reciever.GenderOfInterest == "Неважно") &&
                                    (user.GenderOfInterest == reciever.Gender || user.GenderOfInterest == "Неважно"));

        if (matchingProfile == null)
            matchingProfile = _context.Users
                .FirstOrDefault(user => user.City == reciever.City && user.TgId != recieverId &&
                                        (user.Gender == reciever.GenderOfInterest ||
                                         reciever.GenderOfInterest == "Неважно") &&
                                        (user.GenderOfInterest == reciever.Gender ||
                                         user.GenderOfInterest == "Неважно"));

        var interestsEntities1 = UserRepository.GetUserInterestsById(reciever.TgId);
        var interestNames1 = interestsEntities1.Select(interest => interest.Name).ToList();
        if (matchingProfile == null)
            return null;
        var interestsEntities2 = UserRepository.GetUserInterestsById(matchingProfile.TgId);
        var interestNames2 = interestsEntities2.Select(interest => interest.Name).ToList();

        if (MatchCalculator.CalculateMatch(reciever.Age, matchingProfile.Age,
                reciever.ZodiacSign, matchingProfile.ZodiacSign,
                interestNames1, interestNames2, reciever.IsZodiacSignMatters) < priority)
            return null;

        return matchingProfile;
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