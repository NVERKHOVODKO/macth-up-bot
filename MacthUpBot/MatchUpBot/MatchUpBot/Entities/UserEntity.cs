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
    public bool IsNotified { get; set; }
    public bool IsVip { get; set;}
    public List<UserInterestsEntity> UserInterests { get; set; }
    public List<LikesEntity> LikedByUsers { get; set; }
    public List<LikesEntity> LikedUsers { get; set; }
    
    public InterestWeightEntity InterestWeight { get; set; }
}