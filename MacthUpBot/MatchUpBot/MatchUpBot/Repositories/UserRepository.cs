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


    public UserEntity GetUser(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user;
    }

    public bool GetUserZodiacSignMatter(long tgId)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(e => e.TgId == tgId);
        return user.IsZodiacSignMatters;
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
            GenderOfInterest = "N/A",
            LastShowedBlankTgId = 0
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

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
            user.City = city;
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


    public void CreateRandomFemaleUsers()
    {
        var random = new Random();
        var zodiacSigns = new[]
            { "Рак", "Овен", "Телец", "Близнцы", "Рыбы", "Дева", "Лев", "Скорпион", "Стрелец", "Водолей", "Козерог" };
        var names = new[]
        {
            "Анна", "Екатерина", "Мария", "Ольга", "Татьяна",
            "Елена", "Наталья", "Ирина", "Светлана", "Алиса",
            "Виктория", "Евгения", "Дарья", "Ксения", "Милена",
            "Надежда", "Полина", "Регина", "Юлия", "Ангелина"
        };
        var descriptions = new[]
        {
            "Привет, я новая здесь и ищу интересных собеседников. Обожаю музыку и кино.",
            "Я студентка, увлекаюсь спортом и мечтаю посетить много стран.",
            "Люблю природу и путешествия. Ищу кого-то, кто разделит мои интересы.",
            "Мне нравятся вечера в уютной компании. Давай встретимся в кафе?",
            "Если ты тоже любишь животных, мы найдем общие темы для разговора.",
            "Увлекаюсь искусством и живописью. Готова делиться своими впечатлениями.",
            "Люблю активный образ жизни. Давай вместе заниматься спортом!",
            "Мечтаю научиться играть на гитаре. Ищу учителя и вдохновение.",
            "Обожаю готовить разные блюда. Может, устроим кулинарный поединок?",
            "Интересуюсь наукой и новейшими технологиями. Обсудим актуальные темы?",
            "Мечтаю о кругосветном путешествии. Присоединись к моим приключениям!",
            "Люблю читать книги и обсуждать их с кем-то. Есть любимая книга?",
            "Ищу собеседника, чтобы вместе учиться новому. Готов присоединиться?",
            "Хочу стать лучшей версией себя. Поддержи меня в этом!",
            "Мечтаю о большой семье. Ищу надежного партнера для серьезных отношений.",
            "Путешественник и фотограф. Покажу тебе лучшие места для фотосессии.",
            "Учусь на программиста. Расскажу тебе о мире IT и компьютерах.",
            "Люблю музыку разных жанров. Поделишься своими музыкальными вкусами?",
            "Считаю себя экологически грамотным человеком. Давай обсудим природу и экологию.",
            "Увлекаюсь экстримальными видами спорта. Готов к адреналину?",
            "Люблю искусство и культуру разных стран. Давай обсудим искусство мира.",
            "Ищу интересных собеседников. Готов обсудить любые темы.",
            "Увлекаюсь космосом и астрономией. Поделишься своими знаниями?",
            "Ищу спутника для походов в кино и театр. Какие фильмы ты предпочитаешь?",
            "Люблю животных и всегда готова помочь им. У тебя есть домашние питомцы?",
            "Мечтаю научиться танцевать. Готов показать тебе свои танцевальные движения.",
            "Интересуюсь модой и стилем. Расскажешь о своем стиле одежды?",
            "Увлекаюсь историей и археологией. Давай поговорим о древних цивилизациях.",
            "Ищу спутника для походов на природу. Готов провести время на свежем воздухе?"
        };

        for (var i = 1; i <= 20; i++)
            if (!_context.Users.Any(u => u.TgId == i))
            {
                var user = new UserEntity
                {
                    TgId = i,
                    Name = names[i - 1],
                    Age = random.Next(17, 26),
                    City = "Минск",
                    Gender = "Женский",
                    TgUsername = $"user{i}_telegram",
                    Stage = 0,
                    About = descriptions[random.Next(descriptions.Length)],
                    ZodiacSign = zodiacSigns[random.Next(zodiacSigns.Length)],
                    IsZodiacSignMatters = random.Next(2) == 0,
                    GenderOfInterest = "Мужской",
                    LastShowedBlankTgId = 0
                };

                _context.Users.Add(user);
            }

        _context.SaveChanges();
    }


    /*public void SetUserPhoto(long tgId, string photoPath)
    {
        var user = _context.Users.FirstOrDefault(u => u.TgId == tgId);
        if (user != null)
        {
            user.Photo = photoPath;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }*/


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


    public void DeleteUserById(long userId)
    {
        var user = _context.Users.Find(userId);

        if (user != null)
        {
            _context.Users.Remove(user);
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
}