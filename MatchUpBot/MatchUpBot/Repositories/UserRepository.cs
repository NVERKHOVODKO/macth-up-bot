using Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Data;

public class UserRepository
{
    private readonly Context _context = new();


    public void UpdateUserStage(long tgId, int newStage)
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
    

    public UserEntity CreateUser(long tgId)
    {
        var newUser = new UserEntity
        {
            TgId = tgId,
            Name = "N/A",
            Age = 0,
            City = "N/A",
            Gender = "N/A",
            TgUsername = "N/A",
            TgChatId = "N/A",
            Stage = -1,
            About = "N/A",
            ZodiacSign = "N/A"
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        return newUser;
    }

    public bool IsUserExists(long tgId)
    {
        return _context.Users.Any(u => u.TgId == tgId);
    }

    public async Task AddInterestToUserAsync(long userId, Guid interestId)
    {
        var user = await _context.Users
            .Include(u => u.UserInterests)
            .FirstOrDefaultAsync(u => u.TgId == userId);
        if (user == null)
        {
            return;
        }

        if (user.UserInterests.Any(ui => ui.InterestId == interestId))
        {
            return;
        }

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


    public void SetUserTgUsername(long tgId, string tgUsername)
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
    
    public void SeedInterests()
    {
        if (!_context.Interests.Any())
        {
            var interests = new List<InterestEntity>
            {
                new InterestEntity { Name = "Путешествия" },
                new InterestEntity { Name = "Кино и театр" },
                new InterestEntity { Name = "Спорт" },
                new InterestEntity { Name = "Музыка" },
                new InterestEntity { Name = "Чтение" },
                new InterestEntity { Name = "Искусство" },
                new InterestEntity { Name = "Наука и образование" },
                new InterestEntity { Name = "Киберспорт" },
                new InterestEntity { Name = "Игры" },
                new InterestEntity { Name = "Природа" },
                new InterestEntity { Name = "Фотография" },
                new InterestEntity { Name = "Технологии" },
                new InterestEntity { Name = "Мода и стиль" },
                new InterestEntity { Name = "Автомобили" },
                new InterestEntity { Name = "Здоровье и фитнес" }
            };

            _context.Interests.AddRange(interests);
            _context.SaveChanges();
        }
    }
}