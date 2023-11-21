using System.ComponentModel.DataAnnotations;

namespace Entities;

public class BlanksShowingHistory
{
    [Key] public Guid Id { get; set; }
    [Required] public long ReceivedUserTgId { get; set; }
    [Required] public long ShownUserTgId { get; set; }
    [Required] public DateTime Date { get; set; }
    public UserEntity ReceivedUser { get; set; }
    public UserEntity ShownUser { get; set; }
}