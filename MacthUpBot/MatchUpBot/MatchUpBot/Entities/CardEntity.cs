using System.ComponentModel.DataAnnotations;

namespace Entities;

public class CardEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public long UserId { get; set; }
    public string CardNumber { get; set;}
    public string HolderName { get; set; }
    public string ExpirationTime { get; set; }
    public string CVV { get; set; }
}