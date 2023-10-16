using System.ComponentModel.DataAnnotations;

namespace Entities;

public class UserEntity
{
    [Key] public long TgId { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
    public string Gender { get; set; }
    public string TgUsername { get; set; }
    public string TgChatId { get; set; } //Айди чата, куда отправлять ответ
    public int Stage { get; set; }
    public string About { get; set; }
    public string ZodiacSign { get; set; }
    public bool IsZodiacSignMatters { get; set; }
    public List<UserInterestsEntity> UserInterests { get; set; }

    

    public UserEntity()
    {
    }

    public UserEntity(long tgId, string name, int age, string city, string gender, string tgUsername, string tgChatId, int stage, string about, string zodiacSign, bool isZodiacSignMatters)
    {
        TgId = tgId;
        Name = name;
        Age = age;
        City = city;
        Gender = gender;
        TgUsername = tgUsername;
        TgChatId = tgChatId;
        Stage = stage;
        About = about;
        ZodiacSign = zodiacSign;
        IsZodiacSignMatters = isZodiacSignMatters;
    }

    public void PrintToConsole()
    {
        Console.WriteLine("User Profile:");
        Console.WriteLine($"Telegram ID: {TgId}");
        Console.WriteLine($"Profile Name: {Name ?? "N/A"}");
        Console.WriteLine($"Username: {TgUsername ?? "N/A"}");
        Console.WriteLine($"Age: {Age.ToString() ?? "N/A"}");
        Console.WriteLine($"Sex: {Gender ?? "N/A"}");
        Console.WriteLine($"About: {About ?? "N/A"}");
        Console.WriteLine($"City: {City ?? "N/A"}");
    }
}