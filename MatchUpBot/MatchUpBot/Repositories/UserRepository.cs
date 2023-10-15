using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class UserRepository
{
    private readonly Context _context = new();


    public void UpdateUserStage(long tgId, int newStage)
    {
        var user = _context.Users
            .Where(u => u.TgId == tgId)
            .FirstOrDefault();

        if (user != null)
        {
            user.Stage = newStage;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

    public int GetUserStage(long tgId)
    {
        var user = _context.Users
            .Where(u => u.TgId == tgId)
            .FirstOrDefault();
        return user.Stage;
    }

    public UserEntity CreateUser(long tgId)
    {
        var newUser = new UserEntity
        {
            TgId = tgId,
            Name = "N/A",
            Age = 0,
            Country = "N/A",
            City = "N/A",
            Gender = "N/A",
            TgUsername = "N/A",
            TgChatId = "N/A",
            Stage = -1,
            About = "N/A"
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        return newUser;
    }

    public bool IsUserExists(long tgId)
    {
        return _context.Users.Any(u => u.TgId == tgId);
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
}