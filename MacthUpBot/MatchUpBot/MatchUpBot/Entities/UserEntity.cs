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
    public int Stage { get; set; }
    public string About { get; set; }
    public string ZodiacSign { get; set; }
    public bool IsZodiacSignMatters { get; set; }
    public string GenderOfInterest { get; set; }
    public long LastShowedBlankTgId { get; set; }
    public List<UserInterestsEntity> UserInterests { get; set; }
    public List<LikesEntity> LikedByUsers { get; set; }
    public List<LikesEntity> LikedUsers { get; set; }

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