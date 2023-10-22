namespace EntityFrameworkLesson.DTO;

public class BlankResponse
{
    public long TgId { get; set; }
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
}