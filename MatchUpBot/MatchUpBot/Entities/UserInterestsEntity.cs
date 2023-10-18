using System.ComponentModel.DataAnnotations;

namespace Entities;

public class UserInterestsEntity
{
    [Key] public Guid Id { get; set; }
    public long UserId { get; set; }
    public UserEntity User { get; set; }
    public Guid InterestId { get; set; }
    public InterestEntity Interest { get; set; }
}