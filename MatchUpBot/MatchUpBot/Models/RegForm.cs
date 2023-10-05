namespace TelegramBotExperiments.Models;

public class RegForm
{
    public string TgId { get; set; }
    public string Name { get; set; }
    public string Age { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Gender { get; set; }
    public string Photo { get; set; }
    public string TgUsername { get; set; }
    public string TgChatId { get; set; }

    public int Stage;

    public RegForm(string id, string chatId, string username)
    {
        Stage = 1;
        TgId = id;
        TgUsername = username;
    }

    public (string, int) StageText(string id)
    {
        if (Stage == 1)
            return ("Введите отображаемое имя:", Stage);
        if (Stage == 2)
            return ("Введите возраст:", Stage);
        if (Stage == 3)
            return ("Введите Вашу страну:", Stage);
        if (Stage == 4)
            return ("Введите Ваш город:", Stage);
        if (Stage == 5)
            return ("Введите Ваш пол:", Stage);
        else
            return ("Отправьте боту Ваше фото:", Stage);
    }

    public bool SetParam(string param)
    {
        if (Stage == 1)
            Name = param;
        if (Stage == 2)
            Age = param;
        if (Stage == 3)
            Country = param;
        if (Stage == 4)
            City = param;
        if (Stage == 5)
            Gender = param;
        if (Stage == 6)
            Photo = param;
        Stage++;
        return true;
    }
}
