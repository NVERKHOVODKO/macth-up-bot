using System.ComponentModel.DataAnnotations;

namespace Entities;

public class LikesEntity
{
    [Required] public int LikedByUserId { get; set; }
    [Required] public int LikedUserId { get; set; }
    [Required] public DateTime Data { get; set; }
    [Required] public bool IsActive { get; set; }
}