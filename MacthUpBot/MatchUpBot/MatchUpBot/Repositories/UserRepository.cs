using ConsoleApplication1.Menues;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Data;

public class UserRepository
{
    private static readonly Context _context = new();

    private static readonly ILogger<UserRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UserRepository>();

    public static void UpdateUserStage(long tgId, int newStage)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.TgId == tgId);

        if (user != null)
        {
            user.Stage = newStage;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
    
    public static void DeleteUser(long tgId)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);

        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
    
    public static void DeleteUserAndRelatedEntities(long userId)
    {
        var user = _context.Users.Include(u => u.UserInterests).Include(u => u.LikedByUsers).Include(u => u.LikedUsers)
            .SingleOrDefault(u => u.TgId == userId);

        if (user != null)
        {
            _context.Interests.RemoveRange(GetUserInterestsById(userId));
            var likedByUsers = _context.Likes.Where(like => like.Liker.TgId == userId);
            var likedUsers = _context.Likes.Where(like => like.LikedUser.TgId == userId);
            _context.Likes.RemoveRange(likedByUsers);
            _context.Likes.RemoveRange(likedUsers);
            _context.CreditCards.RemoveRange(_context.CreditCards.Where(card => card.UserId == userId));

            _context.Users.Remove(user);

            _context.SaveChanges();
        }
    }


    public int GetUserStage(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user.Stage;
    }

    public string GetUserGender(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user.Gender;
    }


    public static List<InterestEntity> GetUserInterestsById(long userId)
    {
        var userInterests = _context.UserInterestsEntities
            .Include(ui => ui.Interest)
            .Where(ui => ui.UserId == userId)
            .Select(ui => ui.Interest)
            .ToList();

        return userInterests;
    }
    
    public static UserEntity GetUser(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user;
    }

    public string GetUserAbout(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user.About;
    }


    public static UserEntity CreateUser(long tgId)
    {
        var newUser = new UserEntity
        {
            TgId = tgId,
            Name = "N/A",
            Age = 0,
            City = "N/A",
            Gender = "N/A",
            TgUsername = "None",
            Stage = -1,
            About = "N/A",
            ZodiacSign = "N/A",
            IsNotified = false,
            GenderOfInterest = "N/A",
            LastShowedBlankTgId = 0,
            IsVip = false
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();
        _logger.LogInformation($"user{tgId} created");
        return newUser;
    }

    public static bool IsUserExists(long tgId)
    {
        return _context.Users.Any(u => u.TgId == tgId);
    }

    public async Task AddInterestToUserAsync(long userId, Guid interestId)
    {
        var user = await _context.Users
            .Include(u => u.UserInterests)
            .FirstOrDefaultAsync(u => u.TgId == userId);
        if (user == null) return;

        if (user.UserInterests.Any(ui => ui.InterestId == interestId)) return;

        user.UserInterests.Add(new UserInterestsEntity
        {
            InterestId = interestId
        });

        await _context.SaveChangesAsync();
    }

    public void SetUserName(long tgId, string name)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.Name = name;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
    
    public static void SetIsNotified(long tgId, bool status)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.IsNotified = status;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
    public static void SetIsVip(long tgId, bool status)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.IsVip = status;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }


    public void SetUserAge(long tgId, int age)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.Age = age;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

    public static void SetLastShowedBlankTgId(long tgId, long blankId)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.LastShowedBlankTgId = blankId;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

    public void SetUserCity(long tgId, string city)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.City = city.ToLower();
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

    public void SetUserAbout(long tgId, string about)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.About = about;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

    public void SetUserGender(long tgId, string gender)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.Gender = gender;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

    public void SetUserInterestedGender(long tgId, string gender)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.GenderOfInterest = gender;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }


    public static void SetUserTgUsername(long tgId, string tgUsername)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        
        if (user != null)
        {
            user.TgUsername = tgUsername;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }


    public void SetUserZodiacSign(long tgId, string zodiacSign)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);

        if (user != null)
        {
            user.ZodiacSign = zodiacSign;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

    public void SetUserIsZodiacSignMatters(long tgId, bool isZodiacSignMatters)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.IsZodiacSignMatters = isZodiacSignMatters;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
    
    public async void AddInterestToUser(long userId, int interestNumber, ITelegramBotClient botClient)
    {
        if (interestNumber == 16)
        {
            await BlankMenu.EnterAction(botClient, userId);
            UpdateUserStage(userId, (int)Action.EnterAction);
            _logger.LogInformation($"Пользователь {userId} выбрал завершение добавления интересов.");
            return;
        }

        if (interestNumber >= 1 && interestNumber <= 15)
        {
            var user = _context.Users.Include(u => u.UserInterests).FirstOrDefault(u => u.TgId == userId);
            if (user != null)
            {
                var interests = Interests.GetInterests();
                if (interestNumber >= 0 && interestNumber < interests.Length)
                {
                    var interestName = interests[interestNumber - 1];

                    var existingInterest = user.UserInterests.FirstOrDefault(ui => ui.Interest.Name == interestName);

                    if (existingInterest == null)
                    {
                        var interest = _context.Interests.FirstOrDefault(i => i.Name == interestName);
                        if (interest == null)
                        {
                            interest = new InterestEntity
                            {
                                Id = Guid.NewGuid(),
                                Name = interestName
                            };
                            _context.Interests.Add(interest);
                        }

                        user.UserInterests.Add(new UserInterestsEntity
                        {
                            Interest = interest
                        });
                        _context.SaveChanges();

                        var backToMenuKeyboard = new ReplyKeyboardMarkup(
                            new List<KeyboardButton[]>
                            {
                                new KeyboardButton[]
                                {
                                    new("Вернуться в меню")
                                }
                            })
                        {
                            ResizeKeyboard = true
                        };
                        await botClient.SendTextMessageAsync(userId, "Интерес добавлен",
                            replyMarkup: backToMenuKeyboard);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(userId, "Такой интерес уже добавлен");
                        _logger.LogInformation("Интерес не был добавлен, так как он уже существует.");
                    }
                }
                else
                {
                    _logger.LogError($"Недопустимый номер интереса: {interestNumber}");
                    await botClient.SendTextMessageAsync(userId, "Недопустимый номер интереса");
                }
            }
            else
            {
                _logger.LogError($"Не удалось найти пользователя с TgId: {userId}");
                await botClient.SendTextMessageAsync(userId, "Не удалось найти пользователя");
            }
        }
        else
        {
            _logger.LogError($"Недопустимый номер интереса: {interestNumber}");
            await botClient.SendTextMessageAsync(userId, "Недопустимый номер интереса");
        }
    }
    
    public void DeleteCard(Guid cardId)
    {
        // Находим карту по идентификатору
        var cardToDelete = _context.CreditCards.FirstOrDefault(c => c.Id == cardId);

        if (cardToDelete != null)
        {
            // Удаляем карту из контекста и сохраняем изменения в базе данных
            _context.CreditCards.Remove(cardToDelete);
            _context.SaveChanges();
        }
    }
    public void AddCard(CardEntity card)
    {
        _context.CreditCards.Add(card);
        _context.SaveChanges();
    }
    public int GetCardCountForUser(long userId)
    {
        return _context.CreditCards.Count(c => c.UserId == userId);
    }
    public List<CardEntity> GetCardsForUser(long userId)
    {
        return _context.CreditCards.Where(c => c.UserId == userId).ToList();
    }
    public CardEntity CreateEmptyCard(long userId)
    {
        // Создаем новую пустую карту с установленным UserId
        BlankMenu.currentCardId = Guid.NewGuid();
        var newCard = new CardEntity
        {
            Id = BlankMenu.currentCardId,
            UserId = userId,
            CardNumber = "N/A",
            HolderName = "N/A",
            ExpirationTime = "N/A",
            CVV = "N/A"
        };
       
        // Добавляем новую карту в контекст и сохраняем изменения в базе данных
        _context.CreditCards.Add(newCard);
        _context.SaveChanges();

        // Возвращаем созданную карту
        return newCard;
    }

    public void AddCardNumber(Guid cardId, string cardNumber)
    {
        var cardToUpdate = _context.CreditCards.FirstOrDefault(c => c.Id == cardId);
        if (cardToUpdate != null)
        {
            cardToUpdate.CardNumber = cardNumber;
            _context.SaveChanges();
        }
    }

    public void AddHolderName(Guid cardId, string holderName)
    {
        var cardToUpdate = _context.CreditCards.FirstOrDefault(c => c.Id == cardId);
        if (cardToUpdate != null)
        {
            cardToUpdate.HolderName = holderName;
            _context.SaveChanges();
        }
    }

    public void AddExpirationTime(Guid cardId, string expirationTime)
    {
        var cardToUpdate = _context.CreditCards.FirstOrDefault(c => c.Id == cardId);
        if (cardToUpdate != null)
        {
            cardToUpdate.ExpirationTime = expirationTime;
            _context.SaveChanges();
        }
    }

    public void AddCVV(Guid cardId, string cvv)
    {
        var cardToUpdate = _context.CreditCards.FirstOrDefault(c => c.Id == cardId);
        if (cardToUpdate != null)
        {
            cardToUpdate.CVV = cvv;
            _context.SaveChanges();
        }
    }
    public bool GetUserVip(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user.IsVip;
    }
        
    public void SetVipStatus(long tgId, bool status)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.IsVip = status;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}