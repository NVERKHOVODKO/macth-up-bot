using System.ComponentModel.DataAnnotations;

namespace Entities;

public class LikesEntity
{
    [Key]
    public Guid Id { get; set; }
    [Required] public long LikedByUserId { get; set; }
    [Required] public long LikedUserId { get; set; }
    public UserEntity LikedUser { get; set; }
    public UserEntity Liker { get; set; }
}